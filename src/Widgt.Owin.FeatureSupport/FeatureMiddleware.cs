// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureMiddleware.cs">
//
//  The MIT License (MIT)
//
//  Copyright (c) 2014 Matt Channer
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Widgt.Features
{
    // Environment -> Task function
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;

    using IronPython.Runtime;

    using log4net;

    using Microsoft.Scripting.Hosting;

    using Widgt.Core.Factory;
    using Widgt.Core.Model;
    using Widgt.Core.Utils;
    using Widgt.Features.Model;
    using Widgt.Owin;

    using AppFunc = Func<IDictionary<string, object>, Task>;

    using MiddlewareFunc = Func<IDictionary<string, object>,  // OWIN Environment
                                IronPython.Runtime.PythonDictionary,  // Feature Parameters
                                log4net.ILog,                 // Logger
                                bool>;

    /// <summary>
    /// The middleware support for Widgt features
    /// </summary>
    public class FeatureMiddleware
    {
        /// <summary> The logger instance </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FeatureMiddleware));

        /// <summary> The cache of compiled scripts </summary>
        private readonly ScriptCache scriptCache;

        /// <summary> The next middleware in the pipeline to call </summary>
        private readonly AppFunc next;

        /// <summary> The widget model factory </summary>
        private readonly IWidgtModelFactory modelFactory;

        /// <summary> The middleware options for widgt applications </summary>
        private readonly WidgtOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureMiddleware"/> class.
        /// </summary>
        /// <param name="next"> The next middleware in the pipeline to call. </param>
        /// <param name="modelFactory"> The model factory. </param>
        /// <param name="options"> The widgt options. </param>
        public FeatureMiddleware(AppFunc next, IWidgtModelFactory modelFactory, WidgtOptions options)
        {
            this.next = next;
            this.modelFactory = modelFactory;
            this.options = options;
            this.scriptCache = new PythonScriptCache();
        }

        /// <summary>
        /// Evaluates and applies the Widgt feature
        /// </summary>
        /// <param name="environment">The environment object</param>
        /// <returns>A task instance</returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            if (environment.RequestPath().StartsWith(options.FeaturePrefix))
            {
                await this.WriteFeatureResource(environment);
            }
            else
            {
                FileInfo serverFile = FileSystem.MapRequestPathToWidgetPath(
                    modelFactory.BaseDirectory,
                    options.ServerPrefix,
                    environment.RequestPath());

                WidgetModel widgetModel;
                if (serverFile != null && modelFactory.TryGetWidgetByPath(environment.RequestPath(), out widgetModel))
                {
                    // does not map to a valid resource, maybe it is middleware?
                    await this.MaybeMapMiddlewarePath(environment, widgetModel);
                }
            }

            // Only call the next function in the chain if this one did not redirect
            await next(environment);
        }

        /// <summary>
        /// Writes a feature to the 
        /// </summary>
        /// <param name="environment">The environment instance</param>
        /// <returns>The task to await</returns>
        private async Task WriteFeatureResource(IDictionary<string, object> environment)
        {
            NameValueCollection queryString = environment.RequestQueryString();
            if (queryString.Count == 0) return;

            string featureId = queryString.Get("featureId");
            string resource = queryString.Get("file");

            if (string.IsNullOrEmpty(featureId) || string.IsNullOrEmpty(resource)) return;

            Core.Features.Feature feature;
            if (modelFactory.FeatureProcessor.TryGetFeature(featureId, out feature))
            {
                if (resource[0] == Path.DirectorySeparatorChar) resource = resource.Substring(1);

                string fullPath = Path.Combine(feature.FeatureDirectory.FullName, resource);
                FileInfo file = new FileInfo(fullPath);

                if (file.Exists)
                {
                    environment.SetContentTypeForFile(file.Name);
                    await environment.SendFile(file);
                }
            }
        }

        /// <summary>
        /// Attempts to map a widget specific path to some middleware
        /// </summary>
        /// <param name="environment">The environment containing the request</param>
        /// <param name="widget">The widget that the path is mapped to</param>
        /// <returns>A boolean value indicating whether the path maps to a middleare feature</returns>
        private async Task<bool> MaybeMapMiddlewarePath(IDictionary<string, object> environment, WidgetModel widget)
        {
            string requestPath = environment.RequestPath();

            bool didRun = false;

            foreach (var featureRequest in widget.Widget.Features)
            {
                Core.Features.Feature featureDefinition;
                if (modelFactory.FeatureProcessor.TryGetFeature(featureRequest.Name, out featureDefinition))
                {
                    List<FeatureInclude> includes =
                        featureDefinition.Includes
                            .Where(include => include.Type == FeatureInclude.IncludeType.Middleware)
                            .ToList();

                    foreach (FeatureInclude include in includes)
                    {
                        string middlewarePath = options.ServerPrefix + "/" + widget.UriPart + "/" + include.MiddlewarePath;
                        if (requestPath.StartsWith(middlewarePath))
                        {
                            string fullPath = Path.Combine(featureDefinition.FeatureDirectory.FullName, include.Src);

                            var parameterDict = featureRequest.Parameters.ToDictionary(
                                parameter => parameter.Name,
                                parameter => parameter.Value);

                            didRun = await this.CompileAndRunMiddleware(environment, parameterDict, new FileInfo(fullPath));
                            if (didRun)
                                break;
                        }
                    }
                }
            }

            return didRun;
        }

        /// <summary>
        /// Attempts to load a middleware resource and execute it
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <param name="parameters">The feature parameters passed in from the widget</param>
        /// <param name="middlewareResource">The path to the middleware resource</param>
        /// <returns>A task indicating whether the result completed</returns>
        private async Task<bool> CompileAndRunMiddleware(
            IDictionary<string, object> environment,
            IEnumerable<KeyValuePair<string, string>> parameters,
            FileInfo middlewareResource)
        {
            if (middlewareResource.Exists == false) return await Task.FromResult(false);

            try
            {
                CompiledCode code = scriptCache.GetOrCreate(middlewareResource);
                var scope = scriptCache.Engine.CreateScope();

                code.Execute(scope);

                var middleware = scope.GetVariable<MiddlewareFunc>("invoke");

                if (middleware != null)
                {
                    PythonDictionary pyDict = new PythonDictionary();

                    foreach (var pair in parameters)
                    {
                        pyDict.Add(pair.Key, pair.Value);
                    }

                    return middleware(environment, pyDict, Logger);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
        }
    }
}

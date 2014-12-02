// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtFolderLocalizrMiddleware.cs">
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
using System.IO;
using System.Threading.Tasks;

using Widgt.Core.Exceptions;
using Widgt.Core.Model;
using Widgt.Core.Utils;

namespace Widgt.Owin
{
    using Widgt.Core.Factory;
    
    // Environment -> Task function
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Provides folder based localization support for deployed widgets
    /// </summary>
    public class WidgtFolderLocalizrMiddleware
    {
        /// <summary> The next function to call in the pipeline </summary>
        private readonly AppFunc next;

        /// <summary> The widgt options to use </summary>
        private readonly WidgtOptions options;

        /// <summary> The model factory to use </summary>
        private readonly IWidgtModelFactory modelFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgtFolderLocalizrMiddleware"/> class.
        /// </summary>
        /// <param name="next"> The next function to call in the pipeline </param>
        /// <param name="modelFactory">The model factory</param>
        /// <param name="options"> The widgt options. </param>
        public WidgtFolderLocalizrMiddleware(AppFunc next, IWidgtModelFactory modelFactory, WidgtOptions options)
        {
            Throwable.ThrowIfNull(next, "next");
            Throwable.ThrowIfNull(options, "options");

            this.next = next;
            this.options = options;
            this.modelFactory = modelFactory;
        }

        /// <summary>
        /// Evaluates and applies the Widgt localization owin rules
        /// </summary>
        /// <param name="environment">The environment object</param>
        /// <returns>A task instance</returns>
        public async Task Invoke(IDictionary<string, object> environment)
        {
            // Is the path a sub directory of the widget deployment folder?
            if (environment.RequestPath().StartsWith(options.ServerPrefix))
            {
                await this.WriteWidgetResource(environment);
            }
            
            // Only call the next function in the chain if this one did not redirect
            await next(environment);
        }

        /// <summary>
        /// Based on the incoming request, this method is responsible for locating the resource within the
        /// deployed widget directory. Consideration is made for folder based localization based on the accept-language
        /// header.
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <returns>A task to wait on</returns>
        private async Task WriteWidgetResource(IDictionary<string, object> environment)
        {
            string acceptLangString = environment.RequestHeader("Accept-Language");

            FileInfo serverFile = FileSystem.MapRequestPathToWidgetPath(
                modelFactory.BaseDirectory,
                options.ServerPrefix,
                environment.RequestPath());

            WidgetModel widgetModel;
            if (serverFile != null && modelFactory.TryGetWidgetByPath(environment.RequestPath(), out widgetModel))
            {
                // Attempt to map the given path to a locale file or failing that, the original file
                IEnumerable<LocaleName> acceptLocales = LocaleName.ParseLanguageHeader(acceptLangString);

                FileInfo mappedFile = Localizr.FolderLookup(acceptLocales)
                                              .FindBestMatchingFolder(widgetModel.RootDirectory, serverFile);

                if (mappedFile.Exists)
                {
                    environment.SetContentTypeForFile(mappedFile.Name);
                    await environment.SendFile(mappedFile);
                }
            }
        }
    }
}

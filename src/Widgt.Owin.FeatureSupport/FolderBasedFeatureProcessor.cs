// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderBasedFeatureProcessor.cs">
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

namespace Widgt.Core.Features
{
    using System;
    using System.IO;
    using System.Linq;

    using log4net;

    using Widgt.Core.Exceptions;
    using Widgt.Core.Factory;
    using Widgt.Core.Model;
    using Widgt.Features.Model;

    /// <summary>
    /// Represents a folder based feature manager, where each feature is contained within its
    /// own folder located on the server.
    /// </summary>
    public class FolderBasedFeatureProcessor : IFeatureProcessor
    {
        /// <summary> The logger instance </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FolderBasedFeatureProcessor));

        /// <summary> The prefix to use when building the full URI to the resource </summary>
        private readonly string featureUriPrefix;

        /// <summary> The directory containing the feature folders </summary>
        private FeatureParser parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderBasedFeatureProcessor"/> class. 
        /// </summary>
        /// <param name="featureDirectory">The top level directory containing feature folders </param>
        /// <param name="featureUriPrefix">The URI prefix for features</param>
        public FolderBasedFeatureProcessor(DirectoryInfo featureDirectory, string featureUriPrefix)
        {
            this.featureUriPrefix = featureUriPrefix;
            this.parser = new FeatureParser(featureDirectory);
        }

        /// <inheritdoc />
        public bool TryGetFeature(string featureId, out Feature feature)
        {
            return parser.TryGetFeature(featureId, out feature);
        }

        /// <inheritdoc />
        public void Processes(WidgetModel widget, IStartFileFactory startFileFactory)
        {
            Throwable.ThrowIfNull(widget, "widget");
            Throwable.ThrowIfNull(startFileFactory, "startFileFactory");

            // No features, nothing to do
            if (widget.Widget.Features.Count == 0) return;

            Logger.Info("Processing start file data for widget " + widget.Widget.WidgetId);

            // Process each start file referenced in the manifest
            foreach (Content startFile in widget.Widget.Contents)
            {
                string filePath = startFile.Src;

                // source should never be empty
                if (string.IsNullOrEmpty(filePath)) continue;

                // remove any leading path root otherwise Path.Combine is going to produce a rooted file
                if (filePath[0] == Path.DirectorySeparatorChar)
                    filePath = filePath.Substring(1);
                
                FileInfo file = new FileInfo(Path.Combine(widget.RootDirectory.FullName, filePath));
                if (file.Exists)
                {
                    ProcessFeatureRequests(widget.Widget, file, startFileFactory);
                }
            }
        }

        /// <summary>
        /// Processes each feature request to inject the associated sources into the page head
        /// </summary>
        /// <param name="widget">The widget to process</param>
        /// <param name="file">The path to the start file</param>
        /// <param name="startFileFactory">The start file factory to use for editing the file</param>
        private void ProcessFeatureRequests(Widget widget, FileInfo file, IStartFileFactory startFileFactory)
        {
            try
            {
                IStartFileInjector injector;

                Logger.Info("About to inject requested features into start file " + file.FullName);

                using (Stream startFileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    injector = startFileFactory.LoadStartFile(startFileStream);
                }

                bool wasChanged = false;

                foreach (Model.FeatureRequest featureRequest in widget.Features)
                {
                    Feature foundFeature;
                    if (parser.TryGetFeature(featureRequest.Name, out foundFeature))
                    {
                        // Important - the script injector prepends files, so they need to be added in reverse
                        // in order to honour how they are defined in the feature file
                        foreach (FeatureInclude include in foundFeature.Includes.Reverse())
                        {
                            string queryStringParams = string.Format(
                                "?featureId={0}&file={1}",
                                foundFeature.Id,
                                include.Src);

                            string path = this.featureUriPrefix + queryStringParams;

                            switch (include.Type)
                            {
                                case FeatureInclude.IncludeType.Script:
                                    injector.InjectScript(path);
                                    Logger.Info("Script injected with source " + path);
                                    break;
                                case FeatureInclude.IncludeType.Stylesheet:
                                    injector.InjectStyleSheet(path);
                                    Logger.Info("Stylesheet injected with source " + path);
                                    break;
                            }

                            wasChanged = true;
                        }
                    }
                    else if (featureRequest.Required)
                    {
                        // TODO: The feature is required, but it isn't there.
                        Logger.Warn("Required feature " + featureRequest.Name + " is not present");
                    }
                }

                if (wasChanged)
                {
                    using (Stream outputStream = file.OpenWrite())
                    {
                        Logger.Info("Persisting changes to start file back to disk");
                        injector.WriteTo(outputStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception caught during start file processing", ex);
            }
        }
    }
}

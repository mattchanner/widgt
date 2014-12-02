// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetModelFactory.cs">
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

namespace Widgt.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Ionic.Zip;

    using Widgt.Core.Exceptions;
    using Widgt.Core.Model;
    using Widgt.Core.Utils;

    /// <summary>
    /// An oracle providing a single point of contact for managing one or more widget resources
    /// </summary>
    public class WidgetModelFactory
    {
        /// <summary> The name of the manifest file (case-insensitive) </summary>
        private const string ManifestFileName = "config.xml";

        /// <summary> A case insensitive string comparer </summary>
        private static readonly Func<string, string, bool> CaseInsensitiveComparer =
            (s1, s2) => string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) == 0;

        /// <summary> The working directory </summary>
        private DirectoryInfo workingDirectory;

        /// <summary> A map of loaded widget models </summary>
        private IDictionary<string, WidgetModel> loadedModels; 

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetModelFactory"/> class.
        /// </summary>
        /// <param name="baseDirectory"> The base working directory. </param>
        /// <exception cref="ArgumentNullException"> Thrown when the given directory is null </exception>
        /// <exception cref="ArgumentException">Thrown when the given directory does not exist </exception>
        public WidgetModelFactory(DirectoryInfo baseDirectory)
        {
            Throwable.ThrowIfNull(baseDirectory, "baseDirectory");
            Throwable.ThrowIf(() => baseDirectory.Exists, "directory does not exist", "baseDirectory");

            this.workingDirectory = baseDirectory;
            this.loadedModels = new Dictionary<string, WidgetModel>();
        }

        /// <summary>
        /// Gets the base working directory for the widget factory to work from
        /// </summary>
        public DirectoryInfo BaseDirectory
        {
            get
            {
                return workingDirectory;
            }
        }

        /// <summary>
        /// Un-deploys a widget from the system
        /// </summary>
        /// <param name="widgetId">The ID of the widget to remove</param>
        public void Undeploy(string widgetId)
        {
            KeyValuePair<string, WidgetModel> match = default(KeyValuePair<string, WidgetModel>);
            bool found = false;

            foreach (var kv in this.loadedModels)
            {
                if (kv.Value.Widget.Id == widgetId)
                {
                    match = kv;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                this.loadedModels.Remove(match.Key);
                try
                {
                    match.Value.RootDirectory.Delete(true);
                }
                catch (IOException ioe)
                {
                    Console.Error.WriteLine(ioe);
                }
            }
        }

        /// <summary>
        /// Unzips and parses the widget package, returning the result as an instance of a <see cref="WidgetModel"/>
        /// </summary>
        /// <param name="sourceFile">The file to read from</param>
        /// <param name="deleteOnDeploy">An optional argument stating whether the source widget should be removed once 
        /// it is deployed</param>
        /// <returns>The created widget model</returns>
        /// <exception cref="ArgumentNullException">Thrown when the zip stream is null</exception>
        /// <exception cref="WidgetArchiveException">Thrown when there is an error reading from the zip file</exception>
        /// <exception cref="InvalidManifestFileException">Thrown when the zip file does not contain a valid manifest</exception>
        /// <exception cref="InvalidManifestFileException">Thrown when the manifest contains no content sections</exception>
        public WidgetModel Deploy(FileInfo sourceFile, bool deleteOnDeploy = true)
        {
            WidgetModel deployedWidget = Deploy(sourceFile.OpenRead());
            
            if (deployedWidget != null && deleteOnDeploy)
            {
                sourceFile.Delete();
            }

            return deployedWidget;
        }

        /// <summary>
        /// Unzips and parses the widget package, returning the result as an instance of a <see cref="WidgetModel"/>
        /// </summary>
        /// <param name="zipStream">The zip stream to read from</param>
        /// <returns>The created widget model</returns>
        /// <exception cref="ArgumentNullException">Thrown when the zip stream is null</exception>
        /// <exception cref="WidgetArchiveException">Thrown when there is an error reading from the zip file</exception>
        /// <exception cref="InvalidManifestFileException">Thrown when the zip file does not contain a valid manifest</exception>
        /// <exception cref="InvalidManifestFileException">Thrown when the manifest contains no content sections</exception>
        public WidgetModel Deploy(Stream zipStream)
        {
            if (zipStream == null) throw new ArgumentNullException("zipStream");

            ZipFile zipFile;
            try
            {
                zipFile = ZipFile.Read(zipStream);
            }
            catch (Exception ex)
            {
                // Input stream is not a valid zip archive
                throw new WidgetArchiveException("An error occurred processing the supplied widget file", ex);
            }

            ZipEntry manifestFile = zipFile.FirstOrDefault(entry => CaseInsensitiveComparer(entry.FileName, ManifestFileName));
            if (manifestFile == null) throw new InvalidWidgetArchiveException("Could not locate the config.xml file in the widget archive file");

            Widget widget = new ConfigFileParser().Parse(manifestFile.OpenReader());
            if (widget.Contents.Count == 0) throw new InvalidManifestFileException("Content section missing from config file");

            WidgetModel model = new WidgetModel(widget);

            try
            {
                Unzip(zipFile, model);

                lock (loadedModels)
                {
                    // Cache the model
                    loadedModels[model.UriPart] = model;
                }

                return model;
            }
            catch (IOException ioe)
            {
                throw new WidgetArchiveException("An IO error occurred unzipping the widget file", ioe);
            }
        }

        /// <summary>
        /// Attempts to find a loaded widget based on a path request
        /// </summary>
        /// <param name="pathRequest">The path to match on</param>
        /// <param name="matchingModel">The maching model if found, or null if not</param>
        /// <returns>A value indicating whether a model was found</returns>
        public bool TryGetWidgetByPath(string pathRequest, out WidgetModel matchingModel)
        {
            matchingModel = null;

            foreach (var kv in this.loadedModels)
            {
                if (pathRequest.Contains(kv.Key))
                {
                    matchingModel = kv.Value;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Unzips the given zip file to a directory based on the widget Id.  The root path
        /// for the widget will then be set on the model as the RootDirectory property.
        /// </summary>
        /// <param name="zipFile">The zip file to extract</param>
        /// <param name="model">The widget model</param>
        private void Unzip(IEnumerable<ZipEntry> zipFile, WidgetModel model)
        {
            string widgetId = FileSystem.GetDirectoryNameForId(model.Widget.Id);

            string widgetRootPathString = Path.Combine(workingDirectory.FullName, widgetId);
            DirectoryInfo rootWidgetDir = new DirectoryInfo(widgetRootPathString);
            if (rootWidgetDir.Exists) rootWidgetDir.Delete(true);

            rootWidgetDir.Create();

            model.RootDirectory = rootWidgetDir;

            foreach (ZipEntry zipEntry in zipFile)
            {
                zipEntry.Extract(rootWidgetDir.FullName);
            }
        }
    }
}

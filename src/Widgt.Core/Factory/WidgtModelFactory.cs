// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtModelFactory.cs">
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

namespace Widgt.Core.Factory
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;

    using Ionic.Zip;

    using log4net;

    using Widgt.Core.Db;
    using Widgt.Core.Exceptions;
    using Widgt.Core.Features;
    using Widgt.Core.Model;
    using Widgt.Core.Utils;

    /// <summary>
    /// An oracle providing a single point of contact for managing one or more widget resources
    /// </summary>
    public class WidgtModelFactory : IWidgtModelFactory
    {
        /// <summary> The name of the manifest file (case-insensitive) </summary>
        private const string ManifestFileName = "config.xml";

        /// <summary> The logger instance to use </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WidgtModelFactory));

        /// <summary> The default start files to look for when no content is provided in the manifest </summary>
        private static readonly string[] DefaultStartFiles = { "index.html", "index.htm", "index.svg", "index.xhtml", "index.xht" };

        /// <summary> A case insensitive string comparer </summary>
        private static readonly Func<string, string, bool> CaseInsensitiveComparer =
            (s1, s2) => string.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) == 0;

        /// <summary> The working directory </summary>
        private readonly DirectoryInfo workingDirectory;

        /// <summary> The repository implementation </summary>
        private readonly IWidgtRepository repository;

        /// <summary> The feature processor </summary>
        private readonly IFeatureProcessor featureProcessor;

        /// <summary> The start file editor factory </summary>
        private readonly IStartFileFactory startFileFactory;

        /// <summary> The subject to be used when pushing deployment notifications </summary>
        private readonly Subject<WidgetModel> deployerSubject = new Subject<WidgetModel>();

        /// <summary> The subject to be used when pushing un-deployment notifications </summary>
        private readonly Subject<WidgetModel> undeploySubject = new Subject<WidgetModel>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgtModelFactory"/> class.
        /// </summary>
        /// <param name="baseDirectory"> The base working directory.  </param>
        /// <param name="repository"> The repository implementation </param>
        /// <param name="featureProcessor">The feature processor</param>
        /// <param name="startFileFactory">The start file factory</param>
        /// <exception cref="ArgumentNullException"> Thrown when the given directory is null  </exception>
        /// <exception cref="ArgumentNullException"> Thrown when the given repository is null  </exception>
        /// <exception cref="ArgumentException"> Thrown when the given directory does not exist  </exception>
        public WidgtModelFactory(
            DirectoryInfo baseDirectory, 
            IWidgtRepository repository,
            IFeatureProcessor featureProcessor,
            IStartFileFactory startFileFactory)
        {
            Throwable.ThrowIfNull(baseDirectory, "baseDirectory");
            Throwable.ThrowIfNull(repository, "repository");
            Throwable.ThrowIfNull(featureProcessor, "featureProcessor");
            Throwable.ThrowIfNull(startFileFactory, "startFileFactory");
            Throwable.ThrowIf(() => baseDirectory.Exists, "directory does not exist", "baseDirectory");

            this.workingDirectory = baseDirectory;
            this.repository = repository;
            this.featureProcessor = featureProcessor;
            this.startFileFactory = startFileFactory;
        }

        /// <inheritdoc />
        public DirectoryInfo BaseDirectory
        {
            get { return workingDirectory; }
        }
         
        /// <inheritdoc />
        public IObservable<WidgetModel> WidgetDeployed
        {
            get { return deployerSubject.AsObservable(); }
        }

        /// <inheritdoc />
        public IObservable<WidgetModel> WidgetUndeployed
        {
            get { return undeploySubject.AsObservable(); }
        }

        /// <inheritdoc />
        public IFeatureProcessor FeatureProcessor
        {
            get { return featureProcessor; }
        }

        /// <inheritdoc />
        public bool Undeploy(string widgetId)
        {
            bool found = false;

            Logger.Info("Request received to undeploy widget with id of " + widgetId);

            Widget existing = repository.GetWidget(widgetId);

            if (existing != null)
            {
                WidgetModel model = new WidgetModel(existing);
                DirectoryInfo rootWidgetDir = this.DetermineRootWidgetDir(model.Widget);

                try
                {
                    if (rootWidgetDir.Exists) rootWidgetDir.Delete(true);
                }
                catch (IOException ioe)
                {
                    Logger.Error("Unable to undeploy widget, an IOException occurred when trying to remove the directory", ioe);
                    throw;
                }

                repository.DeleteWidget(model.Widget);

                Logger.Info("Widget with id '" + widgetId + "' has been un-deployed");
                
                // push an undeploy notification
                undeploySubject.OnNext(model);

                found = true;
            }
            else
            {
                Logger.Info("Undeploy did nothing, the requested widget does not exist in the database: " + widgetId);
            }

            return found;
        }

        /// <inheritdoc />
        public WidgetModel Deploy(FileInfo sourceFile, bool deleteOnDeploy = true)
        {
            WidgetModel deployedWidget = Deploy(sourceFile.OpenRead());
            
            if (deployedWidget != null && deleteOnDeploy)
            {
                try
                {
                    sourceFile.Delete();
                }
                catch (IOException ioe)
                {
                    Logger.Error("Widget with id '" + deployedWidget.Widget.WidgetId + "' has been deployed, " +
                        "but the widget file could not be removed", 
                        ioe);
                }
            }

            return deployedWidget;
        }

        /// <inheritdoc />
        public WidgetModel Deploy(Stream zipStream)
        {
            Logger.Info("Deployment requested");

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
            if (manifestFile == null)
            {
                throw new InvalidWidgetArchiveException("Could not locate the config.xml file in the widget archive file");
            }

            Widget widget = new ConfigFileParser().Parse(manifestFile.OpenReader());
            WidgetModel model = new WidgetModel(widget);

            Logger.Info("Manifest file parsed without error");

            try
            {
                Logger.Info("Attempting to un-deploy any existing data for this widget");

                // Try and un-deploy first to remove and database entries and file system artifacts
                this.Undeploy(model.Widget.WidgetId);

                Logger.Info("Un-deploy complete, about to unzip file contents to disk");

                Unzip(zipFile, model);

                Logger.Info("Zip contents unpacked without error");

                if (widget.Contents.Count == 0)
                {
                    AddDefaultStartFileIfNotPresent(model);

                    if (widget.Contents.Count == 0)
                    {
                        string all = string.Concat(DefaultStartFiles.Select(s => s + ", "));
                        all = all.Remove(all.Length - 2);

                        Logger.Error("Widget is invalid, there is no start file defined in the manifest, and it was not possible" + 
                            " to infer one from the package contents.  Expected a file at the root with one of the following names: " + all);

                        try
                        {
                            // the widget is invalid, but this has only been determined by unzipping the wgt file
                            // so before throwing an exception, attempt to clean up the unzipped directory
                            model.RootDirectory.Delete(true);
                        }
                        catch (IOException ioe)
                        {
                            Logger.Warn("Unable to clean up working directory for invalid widget, this could leave the system" +
                                        " in an invalid state", 
                                        ioe);
                        }

                        throw new InvalidManifestFileException("Content section missing from config file");
                    }
                }

                Logger.Info("Widget is valid, attempting to process each start file");
                featureProcessor.Processes(model, startFileFactory);

                repository.AddWidget(model.Widget);

                Logger.Info("Widget added to the repository, Notifying subscribers of deployed widget");

                deployerSubject.OnNext(model);

                return model;
            }
            catch (IOException ioe)
            {
                Logger.Error("Exception caught during widget deployment", ioe);
                throw new WidgetArchiveException("An IO error occurred unzipping the widget file", ioe);
            }
        }

        /// <inheritdoc />
        public bool TryGetWidgetByPath(string pathRequest, out WidgetModel matchingModel)
        {
            matchingModel = null;

            foreach (Widget w in this.repository.GetWidgets())
            {
                WidgetModel model = new WidgetModel(w);
            
                if (pathRequest.Contains(model.UriPart))
                {
                    model.RootDirectory = this.DetermineRootWidgetDir(model.Widget);
                    matchingModel = model;
                    break;
                }
            }

            return matchingModel != null;
        }

        /// <summary>
        /// Unzips the given zip file to a directory based on the widget Id.  The root path
        /// for the widget will then be set on the model as the RootDirectory property.
        /// </summary>
        /// <param name="zipFile">The zip file to extract</param>
        /// <param name="model">The widget model</param>
        private void Unzip(IEnumerable<ZipEntry> zipFile, WidgetModel model)
        {
            DirectoryInfo rootWidgetDir = this.DetermineRootWidgetDir(model.Widget);
            if (rootWidgetDir.Exists) rootWidgetDir.Delete(true);

            rootWidgetDir.Create();

            model.RootDirectory = rootWidgetDir;

            foreach (ZipEntry zipEntry in zipFile)
            {
                zipEntry.Extract(rootWidgetDir.FullName);
            }
        }

        /// <summary>
        /// Determines the root directory for a given widget model based on its identifier
        /// </summary>
        /// <param name="widget">The model to determine the directory for</param>
        /// <returns>The determined directory</returns>
        private DirectoryInfo DetermineRootWidgetDir(Widget widget)
        {
            string widgetId = FileSystem.GetDirectoryNameForId(widget.WidgetId);

            string widgetRootPathString = Path.Combine(workingDirectory.FullName, widgetId);
            DirectoryInfo rootWidgetDir = new DirectoryInfo(widgetRootPathString);

            return rootWidgetDir;
        }

        /// <summary>
        /// If no content is defined in the manifest, the unzipped directory should be inspected to auto configure this
        /// based on a predefined list of supported start file names. Note that this does not dig into the locales folder
        /// so a default start file must be present in the root folder for this inference to function correctly. Also
        /// note that this does not prevent auto detection of any localized files at run time
        /// </summary>
        /// <see cref="http://www.w3.org/TR/widgets/#reserved-file-and-folder-names"/>
        /// <param name="model">The model to inspect</param>
        private void AddDefaultStartFileIfNotPresent(WidgetModel model)
        {
            if (model.Widget.Contents.Count > 0) return;

            Logger.Warn("Manifest file does not contain a start file, attempting to infer once from the package contents");

            foreach (string startFile in DefaultStartFiles)
            {
                if (model.RootDirectory.GetFiles(startFile).Length > 0)
                {
                    Logger.Info("Auto-detected manifest file with name " + startFile + 
                        ", assigning as the default start file for the widget");

                    model.Widget.Contents.Add(new Content { Src = startFile });
                    break;
                }
            }
        }
    }
}

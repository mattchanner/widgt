// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureParser.cs">
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

namespace Widgt.Features.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using log4net;

    using Widgt.Core.Exceptions;
    using Widgt.Core.Features;
    using Widgt.Core.Utils;

    /// <summary>
    /// Represents a parser for feature files
    /// </summary>
    internal class FeatureParser
    {
        /// <summary> The logger instance to use </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(FeatureParser));

        /// <summary> Cached features </summary>
        private readonly IDictionary<string, Feature> features;

        /// <summary> The top level feature directory </summary>
        private readonly DirectoryInfo featureDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureParser"/> class.
        /// </summary>
        /// <param name="featureDir">The feature directory to scan</param>
        public FeatureParser(DirectoryInfo featureDir)
        {
            Throwable.ThrowIfNull(featureDir, "featureDir");

            this.featureDir = featureDir;
            this.features = new Dictionary<string, Feature>();
            this.Scan();
        }

        /// <summary>
        /// Attempts to load a feature with a given id
        /// </summary>
        /// <param name="featureId">The id of the feature to load</param>
        /// <param name="feature">The found feature</param>
        /// <returns>True if the feature was found, false if not</returns>
        public bool TryGetFeature(string featureId, out Feature feature)
        {
            Throwable.ThrowIfNullOrEmpty(featureId, "featureId");

            Logger.Info("Attempting to lookup feature with id of " + featureId);

            featureId = featureId.ToLowerInvariant();
            feature = null;

            try
            {
                if (features.TryGetValue(featureId, out feature))
                {
                    Logger.Info("Cached feature found with id of " + featureId);

                    if (feature.FeatureFile.Exists == false)
                    {
                        Logger.Warn("Cached feature points to a file that no longer exists! Feature file was: " + 
                            feature.FeatureFile.FullName + ", this feature will be removed");

                        // file is no longer present on disk, so remove from cache
                        features.Remove(featureId);
                        feature = null;
                    }
                    else if (FileSystem.Checksum(feature.FeatureFile) != feature.Checksum)
                    {
                        Logger.Info("Cached feature is out of date (checksum mismatch). Will reload from disk");

                        // file is stale, mark for reload
                        feature = this.Load(feature.FeatureFile);
                    }
                }
            }
            catch (FeatureLoadException fle)
            {
                Logger.Error("Exception caught trying to load feature", fle);
            }

            Logger.Info("Feature lookup for " + featureId + " result = " + (feature != null ? "FOUND" : "NOT FOUND"));

            return feature != null;
        }

        /// <summary>
        /// Scans the entire directory tree looking for new features to process
        /// </summary>
        private void Scan()
        {
            Logger.Info("Performing a directory scan for features");

            var featureDirectories = this.featureDir.GetDirectories();
            
            foreach (DirectoryInfo featureDirectory in featureDirectories)
            {
                try
                {
                    FileInfo[] featureFiles = featureDirectory.GetFiles("feature.xml", SearchOption.TopDirectoryOnly);

                    if (featureFiles.Length == 1)
                    {
                        this.Load(featureFiles[0]);
                    }
                    else
                    {
                        Logger.Info("Feature directory " + featureDirectory.Name + " does not contain a feature.xml file");
                    }
                }
                catch (FeatureLoadException fle)
                {
                    Console.Error.WriteLine(fle);
                }
            }
        }

        /// <summary>
        /// Loads and returns a feature based on a feature file
        /// </summary>
        /// <param name="featureFile">The feature file to load from</param>
        /// <returns>The loaded feature</returns>
        private Feature Load(FileInfo featureFile)
        {
            Throwable.ThrowIfNull(featureFile, "featureFile");
            Feature feature;

            Logger.Info("Attempting to load feature file from path " + featureFile.FullName);
            try
            {
                XDocument featureDocument = XDocument.Load(featureFile.FullName);
                XElement root = featureDocument.Root;
                if (root == null)
                    throw new FeatureLoadException("Data at the root element is missing for file: " + featureFile.FullName);

                if (root.Name != "feature")
                    throw new FeatureLoadException("Invalid root element in feature file, expected 'feature', got '" + root.Name + "'");

                XElement nameElement = root.Element("name");
                if (nameElement == null || string.IsNullOrEmpty(nameElement.Value))
                    throw new FeatureLoadException("Invalid feature file, no name element present in file " + featureFile.FullName);

                feature = new Feature(featureFile, FileSystem.Checksum(featureFile));
                feature.Id = nameElement.Value;

                foreach (var include in from el in root.Elements()
                                        where el.Name == "script" || el.Name == "stylesheet" || el.Name == "middleware"
                                        select el)
                {
                    FeatureInclude.IncludeType type;
                    string middleWarePath = string.Empty;

                    if (include.Name == "script")
                    {
                        type = FeatureInclude.IncludeType.Script;
                    }
                    else if (include.Name == "stylesheet")
                    {
                        type = FeatureInclude.IncludeType.Stylesheet;
                    }
                    else
                    {
                        type = FeatureInclude.IncludeType.Middleware;
                        XAttribute middleWareAttr = include.Attribute("path");
                        if (middleWareAttr != null)
                        {
                            middleWarePath = middleWareAttr.Value;
                        }
                    }
                    
                    XAttribute attr = include.Attribute("src");
                    if (attr != null)
                    {
                        if (type == FeatureInclude.IncludeType.Middleware && middleWarePath.Length == 0)
                        {
                            Logger.Warn("Invalid middleware include found - no path set!");
                        }
                        else
                        {
                            feature.Includes.Add(new FeatureInclude(type, attr.Value, middleWarePath));
                            Logger.Info("Feature resource found - type is " + type + ", source is " + attr.Value);
                        }
                    }
                }

                features[feature.Id.ToLowerInvariant()] = feature;
            }
            catch (FeatureLoadException fle)
            {
                Logger.Error("Load exception caught parsing file " + featureFile.FullName, fle);
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("Load exception caught parsing file " + featureFile.FullName, ex);
                throw new FeatureLoadException("An exception occurred loading feature file: " + featureFile.FullName, ex);
            }

            return feature;
        }
    }
}

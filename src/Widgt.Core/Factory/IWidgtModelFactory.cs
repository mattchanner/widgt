// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWidgtModelFactory.cs">
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
    using System.IO;

    using Widgt.Core.Exceptions;
    using Widgt.Core.Features;
    using Widgt.Core.Model;

    /// <summary>
    /// Represents the primary interface for deploying, un-deploying and enumerating widgets
    /// </summary>
    public interface IWidgtModelFactory
    {
        /// <summary>
        /// Gets the base working directory for the widget factory to work from
        /// </summary>
        DirectoryInfo BaseDirectory { get; }

        /// <summary>
        /// Gets an observable that can be subscribed to by the caller to receive push notifications 
        /// when a widget is deployed
        /// </summary>
        IObservable<WidgetModel> WidgetDeployed { get; }

        /// <summary>
        /// Gets an observable that can be subscribed to by the caller to receive push notifications 
        /// when a widget is un-deployed
        /// </summary>
        IObservable<WidgetModel> WidgetUndeployed { get; }

        /// <summary> Gets the feature processor associated to this factory </summary>
        IFeatureProcessor FeatureProcessor { get; }

        /// <summary>
        /// Un-deploys a widget from the system
        /// </summary>
        /// <param name="widgetId">The ID of the widget to remove</param>
        /// <returns>A boolean value indicating whether the widget was found and un-deployed or not</returns>
        bool Undeploy(string widgetId);

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
        WidgetModel Deploy(FileInfo sourceFile, bool deleteOnDeploy = true);

        /// <summary>
        /// Unzips and parses the widget package, returning the result as an instance of a <see cref="WidgetModel"/>
        /// </summary>
        /// <param name="zipStream">The zip stream to read from</param>
        /// <returns>The created widget model</returns>
        /// <exception cref="ArgumentNullException">Thrown when the zip stream is null</exception>
        /// <exception cref="WidgetArchiveException">Thrown when there is an error reading from the zip file</exception>
        /// <exception cref="InvalidManifestFileException">Thrown when the zip file does not contain a valid manifest</exception>
        /// <exception cref="InvalidManifestFileException">Thrown when the manifest contains no content sections</exception>
        WidgetModel Deploy(Stream zipStream);

        /// <summary>
        /// Attempts to find a loaded widget based on a path request
        /// </summary>
        /// <param name="pathRequest">The path to match on</param>
        /// <param name="matchingModel">The maching model if found, or null if not</param>
        /// <returns>A value indicating whether a model was found</returns>
        bool TryGetWidgetByPath(string pathRequest, out WidgetModel matchingModel);
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFeatureProcessor.cs">
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
    using Widgt.Core.Factory;
    using Widgt.Core.Model;

    /// <summary>
    /// A feature is a way of injecting dependencies into a widget start file for use by that widget.
    /// A folder based feature is stored in a server directory and contains its own manifest with
    /// the ID of the feature (referenced in the widget's config file), and a list of resources to
    /// inject into the page when that start file is requested.
    /// The feature processor is responsible for taking a deployed widget, and injecting these features
    /// into each of the declared start files.
    /// Note that a feature can be declared as required, so an open question is at what point would this
    /// check be made? At deployment time or when the start file is requested?
    /// </summary>
    public interface IFeatureProcessor
    {
        /// <summary>
        /// Attempts to load a feature with a given id
        /// </summary>
        /// <param name="featureId">The id of the feature to load</param>
        /// <param name="feature">The found feature</param>
        /// <returns>True if the feature was found, false if not</returns>
        bool TryGetFeature(string featureId, out Feature feature);

        /// <summary>
        /// The main method for processing all start files referenced by the contents collection of the widget. 
        /// </summary>
        /// <param name="widget">The widget to process</param>
        /// <param name="startFileFactory">The start file factory, used for parsing and injecting resources
        /// into the page</param>
        void Processes(WidgetModel widget, IStartFileFactory startFileFactory);
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetModel.cs">
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

namespace Widgt.Core.Model
{
    using System;
    using System.IO;

    /// <summary>
    /// An internal model representation of a widget
    /// </summary>
    public class WidgetModel
    {
        private const string HttpProtocol = "http://";

        private const string HttpsProtocol = "https://";

        /// <summary> The underlying widget definition </summary>
        private readonly Widget widget;

        /// <summary> The path to the root widget dir </summary>
        private readonly string webPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetModel"/> class. 
        /// </summary>
        /// <param name="widget"> The underlying widget </param>
        public WidgetModel(Widget widget)
        {
            this.widget = widget;
            this.webPath = this.DeterminePath();
        }

        /// <summary>
        /// Gets the underlying widget
        /// </summary>
        public Widget Widget
        {
            get
            {
                return this.widget;
            }
        }

        /// <summary>
        /// Gets or sets the root working directory for this widget
        /// </summary>
        public DirectoryInfo RootDirectory { get; set; }

        /// <summary>
        /// Gets the URI segment that is used to match this widget with an incoming request
        /// </summary>
        public string UriPart
        {
            get { return webPath; }
        }

        /// <summary>
        /// Computes a URI path to the widget based on the widget identifier
        /// </summary>
        /// <returns>The URI for the widget</returns>
        private string DeterminePath()
        {
            string cleanedIdentifier = this.widget.WidgetId;

            if (this.widget.WidgetId.StartsWith(HttpProtocol, StringComparison.InvariantCultureIgnoreCase))
            {
                cleanedIdentifier = cleanedIdentifier.Substring(HttpProtocol.Length);
            }
            if (this.widget.WidgetId.StartsWith(HttpsProtocol, StringComparison.InvariantCultureIgnoreCase))
            {
                cleanedIdentifier = cleanedIdentifier.Substring(HttpsProtocol.Length);
            }

            cleanedIdentifier = cleanedIdentifier.Replace(" ", "%20").Replace(".", "_");
            
            return cleanedIdentifier;
        }
    }
}

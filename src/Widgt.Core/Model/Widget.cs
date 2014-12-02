// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Widget.cs">
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

using System.Collections.Generic;

namespace Widgt.Core.Model
{
    using System;

    /// <summary>
    /// Represents a single W3C widget
    /// </summary>
    [Serializable]
    public class Widget : DbAware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Widget"/> class.
        /// </summary>
        public Widget()
        {
            this.Contents = new List<Content>();
            this.ViewModes = new List<string>();
            this.Names = new List<Name>();
            this.Descriptions = new List<Description>();
            this.Icons = new List<Icon>();
            this.Preferences = new List<Preference>();
            this.Features = new List<FeatureRequest>();
            this.Licenses = new List<License>();
            this.AccessRequests = new List<AccessRequest>();
        }

        /// <summary>
        /// Gets or sets the id Attribute.
        /// It is optional for authors to use the id attribute with a widget element.
        /// </summary>
        public string WidgetId { get; set; }

        /// <summary>
        /// Gets or sets a version attribute that specifies the version opf the widget
        /// It is optional for authors to use the version attribute with a widget element.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a numeric attribute greater than 0 that indicates the preferred viewport height of 
        /// the instantiated custom start file in CSS pixels
        /// It is optional for authors to use the version attribute with a widget element.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets a numeric attribute greater than 0 that indicates the preferred viewport width of 
        /// the instantiated custom start file in CSS pixels
        /// It is optional for authors to use the version attribute with a widget element.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Gets a keyword list attribute denoting the author's preferred view mode, 
        /// followed by the next most preferred view mode and so forth.  When the attribute
        /// is missing, or if left empty, it implies that the author expects the user agent to 
        /// select an appropriate viewmode for the widget.
        /// The concept of a viewport is defined in CSS, but is essentially a window or other viewing area on the screen.
        /// It is optional for authors to use the version attribute with a widget element.
        /// </summary>
        public IList<string> ViewModes { get; set; }

        /// <summary>
        /// Gets or sets the language attribute that specifies, through a language tag, the author's preferred locale for the widget.
        /// Its intended use is to provide a fallback in case the user agent cannot match any of the
        /// widget's localized content to the user agent locales list of in case the author has not provided unlocalized content.
        /// </summary>
        public string DefaultLocale { get; set; }

        /// <summary>
        /// Gets the list of names.
        /// The name attribute represents the full human-readable name for a widget that is used, for example, in an 
        /// application menu or in other contexts.  There can be zero to many names, where each name must represent
        /// a different language
        /// </summary>
        public IList<Name> Names { get; set; }

        /// <summary>
        /// Gets the collection of descriptions (zero to many, where each instance must represent a different language)
        /// </summary>
        public IList<Description> Descriptions { get; set; }

        /// <summary>
        /// Gets or sets an author that represents people or an organization attributed with the creation of the widget
        /// </summary>
        public Author Author { get; set; }

        /// <summary>
        /// Gets the list of custom icons for the widget
        /// </summary>
        public IList<Icon> Icons { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="Content"/> elements for the widget
        /// </summary>
        public IList<Content> Contents { get; set; }

        /// <summary>
        /// Gets the list of preferences for this widget
        /// </summary>
        public IList<Preference> Preferences { get; set; }

        /// <summary>
        /// Gets the list of features required by this widget
        /// </summary>
        public IList<FeatureRequest> Features { get; set; }

        /// <summary>
        /// Gets the license associated to the widget
        /// </summary>
        public IList<License> Licenses { get; set; }

        /// <summary>
        /// Gets the list of access requests present in the configuration file
        /// </summary>
        public IList<AccessRequest> AccessRequests { get; set; }
    }
}

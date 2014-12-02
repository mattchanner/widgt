// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Content.cs">
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

    /// <summary>
    /// The content element is used by an author to declare which custom start file the user agent is expected to use when it 
    /// instantiates the widget.
    /// </summary>
    [Serializable]
    public class Content : DbAware, ILanguageAware
    {
        /// <summary>
        /// Gets the parent widget that this request is for
        /// </summary>
        public Widget Widget { get; set; }

        /// <summary>
        /// Gets or sets the language identifier for this element
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the path attribute that allows an author to point to a file within the widget package
        /// </summary>
        public string Src { get; set; }

        /// <summary>
        /// Gets or sets the media type attribute that indicates the media type of the file references by the src attribute
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets thea keyword attribute that denotes the character encoding of the file identifier by the src attribute.
        /// The value is the "name" of "alias" of any "Character Set" listed in <a href="http://www.w3.org/TR/2012/REC-widgets-20121127/#iana-charsets">IANA-Charsets</a>.
        /// The default encoding is UTF-8, it is OPTIONAL for a user agent to support other character encodings.
        /// </summary>
        public string Encoding { get; set; }
    }
}

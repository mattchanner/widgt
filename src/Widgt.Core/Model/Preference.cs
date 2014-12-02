// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Preference.cs">
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
    /// The preference element allows authors to declare one or more preferences: a preference is a persistently stored name-value pair 
    /// that is associated with the widget the first time the widget is initiated.
    /// </summary>
    [Serializable]
    public class Preference : DbAware, ILanguageAware
    {
        /// <summary>
        /// Gets the parent widget that this request is for
        /// </summary>
        public Widget Widget { get; set; }

        /// <summary>
        /// Gets or sets the language specifier
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets a string that denotes the name of this preference.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a string that denotes the value of this preference.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets a boolean attribute indicating whether this preference can, or cannot, be overwritten at runtime (e.g., via 
        /// an author script). When set to true, it means that the preference cannot be overwritten. When set to false, it means that 
        /// the preference can be overwritten.
        /// </summary>
        public bool Readonly { get; set; }
    }
}

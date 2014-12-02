// --------------------------------------------------------------------------------------------------------------------
// <copyright file="License.cs">
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
    /// The license element represents a software license, which includes, for example, a usage agreement, 
    /// redistribution statement, and/or a copyright license terms under which the content of the widget package is provided.
    /// </summary>
    [Serializable]
    public class License : DbAware, ILanguageAware
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
        /// Gets or sets a valid IRI or a valid path that points to a representation of a 
        /// software and/or content license
        /// </summary>
        public string HRef { get; set; }

        /// <summary>
        /// Gets or sets the contents of the license block
        /// </summary>
        public string Contents { get; set; }
    }
}

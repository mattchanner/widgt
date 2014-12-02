// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessRequest.cs">
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
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgt.Core.Model
{
    using System;

    /// <summary>
    /// Represents a single Widget Access Request Policy {WARP} element in a configuration file.
    /// <see cref="http://www.w3.org/TR/widgets-access/#conformance"/>
    /// </summary>
    [Serializable]
    public class AccessRequest : DbAware
    {
        /// <summary>
        /// Gets the parent widget that this request is for
        /// </summary>
        public Widget Widget { get; set; }

        /// <summary>
        /// Gets or sets the IRI attribute that defines the specifics of the access request that is made.
        /// Only the scheme and authority components can be present in the IRI that this attribute contains. 
        /// Additionally, an author can use the specific value of ASTERISK (*).  This special value provides
        /// a means for  an author to request from the user agent unrestricted access to a network resource.
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the host component part of the access request applies 
        /// to subdomains (as defined in [RFC1034]) of domain in the origin attribute. The default value when 
        /// this attribute is absent is false, meaning that access to subdomains is not requested.
        /// </summary>
        public bool Subdomains { get; set; }
    }
}

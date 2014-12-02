// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureParameter.cs">
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
    /// The feature param element defines a parameter for a feature. A parameter is a name-value pair that is associated 
    /// with the corresponding feature for which the parameter is declared for. An author establishes the relationship 
    /// between a parameter and feature by having a param element as a direct child of a feature element in document order.
    /// </summary>
    [Serializable]
    public class FeatureParameter : DbAware
    {
        /// <summary>
        /// Gets or sets the parent feature
        /// </summary>
        public FeatureRequest Feature { get; set; }

        /// <summary>
        /// Gets or sets a string that denotes the name of this parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a string that denotes the value of this parameter.
        /// </summary>
        public string Value { get; set; }
    }
}

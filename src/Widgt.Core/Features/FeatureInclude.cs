// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureInclude.cs">
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
    /// <summary>
    /// A resource associated to a feature
    /// </summary>
    public class FeatureInclude
    {
        /// <summary> The type of resource being included </summary>
        private readonly IncludeType type;

        /// <summary> The relative path from the feature folder to the resource to load </summary>
        private readonly string src;

        /// <summary> The path to map the middleware to when applicable </summary>
        private readonly string middleWarePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureInclude"/> class.
        /// </summary>
        /// <param name="type"> The resource type </param>
        /// <param name="src"> The relative path to the resource. </param>
        /// <param name="middleWarePath"> The middle ware path to map this resource to if type is Middleware. </param>
        public FeatureInclude(IncludeType type, string src, string middleWarePath = "")
        {
            this.type = type;
            this.src = src;
            this.middleWarePath = middleWarePath;
        }

        /// <summary> Represents the type of feature this include represents </summary>
        public enum IncludeType
        {
            /// <summary> A script resources </summary>
            Script,

            /// <summary> A stylesheet resources </summary>
            Stylesheet,

            /// <summary> A server side middleware script </summary>
            Middleware
        }

        /// <summary>
        /// Gets the relative path to the file.
        /// </summary>
        public string Src
        {
            get { return src; }
        }

        /// <summary>
        /// Gets the type of resource represented by this feature include
        /// </summary>
        public IncludeType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the path to map the middleware to if applicable
        /// </summary>
        public string MiddlewarePath
        {
            get { return middleWarePath; }
        }
    }
}

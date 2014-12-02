// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feature.cs">
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
    using System.Collections.Generic;

    /// <summary>
    /// A feature is a URI identifiable runtime component (e.g. an Application Programming Interface or video decoder). The act of 
    /// an author requesting the availability of a feature through a feature element is referred to as a feature request. The 
    /// feature element serves as a standardized means to request the binding of an IRI identifiable runtime component to a widget 
    /// for use at runtime. Using a feature element denotes that, at runtime, a widget can attempt to access the feature identified 
    /// by the feature element's name attribute. How a user agent makes use of features depends on the user agent's security policy, 
    /// hence activation and authorization requirements for features are beyond the scope of this specification. A feature has zero 
    /// or more parameters associated with it.
    /// </summary>
    [Serializable]
    public class FeatureRequest : DbAware, ILanguageAware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureRequest"/> class.
        /// </summary>
        public FeatureRequest()
        {
            Parameters = new List<FeatureParameter>();
        }

        /// <summary>
        /// Gets or sets the parent widget that this request is for
        /// </summary>
        public Widget Widget { get; set; }

        /// <summary>
        /// Gets or sets the language specifier
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the IRI attribute that identifies a feature that is needed by the widget at runtime (such as an API).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a boolean attribute that indicates whether or not this feature has to be available to the widget at runtime.
        /// When set to true, the required attribute denotes that a feature is absolutely needed by the widget to function correctly, and
        /// without the availability of this feature the widget serves no useful purpose or won't execute properly.
        /// When set to false, the require attribute denotes that a widget can function correctly without the feature being supported or
        /// otherwise made available by the user agent. 
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets a list of feature parameters.
        /// A feature parameter element defines a parameter for a feature. A parameter is a name-value pair that is associated 
        /// with the corresponding feature for which the parameter is declared for. An author establishes the relationship 
        /// between a parameter and feature by having a param element as a direct child of a feature element in document order.
        /// </summary>
        public IList<FeatureParameter> Parameters { get; set; }
    }
}

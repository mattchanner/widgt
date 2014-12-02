// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtDto.cs">
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

namespace Widgt.Api.Shared
{
    using System.ComponentModel;

    /// <summary>
    /// A data transfer object representing a single, deployed widget
    /// </summary>
    public class WidgtDto
    {
        /// <summary> Gets or sets the ID of the widget </summary>
        [Description("The Primary Key for the Widget")]
        public string Id { get; set; }

        /// <summary> Gets or sets the name of the widget </summary>
        [Description("The localized name of the widget")]
        public string Name { get; set; }

        /// <summary> Gets or sets the description of the widget </summary>
        [Description("The localized description for the widget")]
        public string Description { get; set; }

        /// <summary> Gets or sets the height of the widget </summary>
        [Description("The preferred height of the widget when rendered")]
        public int? Height { get; set; }

        /// <summary> Gets or sets the width of the widget </summary>
        [Description("The preferred width of the widget when rendered")]
        public int? Width { get; set; }

        /// <summary> Gets or sets the relative path to the start file </summary>
        [Description("The localized path to the starting HTML file to be used when rendering the widget")]
        public string StartFilePath { get; set; }

        /// <summary> Gets or sets the relative path to the icon file </summary>
        [Description("The localized path to the icon for the widget")]
        public string IconPath { get; set; }
    }
}

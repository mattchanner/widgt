// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestHelper.cs">
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

namespace Widgt.Core.Tests
{
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;

    using Widgt.Core.Model;

    /// <summary>
    /// Helper class for loading test documents
    /// </summary>
    internal class TestHelper
    {
        /// <summary>
        /// Helper used to load a widget from file
        /// </summary>
        /// <param name="configName">The file to load from</param>
        /// <returns>The parsed widget</returns>
        public static Widget GetWidgetFromFile(string configName)
        {
            XDocument doc = GetConfigurationFileAsXDocument(configName);
            Widget widget = new ConfigFileParser().Parse(doc);
            return widget;
        }

        /// <summary>
        /// Gets and loads an XML document based on its file name
        /// </summary>
        /// <param name="shortName">The short file name to use</param>
        /// <returns>The loaded and parsed document</returns>
        public static XDocument GetConfigurationFileAsXDocument(string shortName)
        {
            return XDocument.Load(GetConfigurationFileAsStream(shortName));
        }

        /// <summary>
        /// Gets and loads an XML document based on its file name
        /// </summary>
        /// <param name="shortName">The short file name to use</param>
        /// <returns>The loaded and parsed document</returns>
        public static Stream GetConfigurationFileAsStream(string shortName)
        {
            string fullName = "ConfigFiles\\" + shortName + ".xml";

            return File.OpenRead(fullName);
        }
    }
}

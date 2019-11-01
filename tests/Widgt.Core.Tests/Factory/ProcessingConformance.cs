// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessingConformance.cs">
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

namespace Widgt.Core.Tests.Parser
{
    using NUnit.Framework;

    using Widgt.Core;
    using Widgt.Core.Exceptions;
    using Widgt.Core.Model;
    using Widgt.Core.Tests;

    /// <summary>
    /// Tests for conformance to the section:
    /// 9.1.2 Algorithm to Process a Configuration Document
    /// </summary>
    [TestFixture]
    public class ProcessingConformance
    {
        /// <summary>
        /// Section: 9.1.2 Step 4
        /// If the root element is not a widget element in the widget namespace, then the user agent MUST terminate 
        /// this algorithm and treat this widget package as an invalid widget package.
        /// </summary>
        [Test]
        public void If_root_element_is_not_a_widget_element_then_package_is_invalid()
        {
            ConfigFileParser parser = new ConfigFileParser();

            Assert.Throws<ConfigFileParseException>(() => parser.Parse(TestHelper.GetConfigurationFileAsXDocument("InvalidXmlRoot")));
        }

        /// <summary>
        /// Section: 9.1.2 Step 4
        /// If the root element is not a widget element in the widget namespace, then the user agent MUST terminate 
        /// this algorithm and treat this widget package as an invalid widget package.
        /// </summary>
        [Test]
        public void If_root_element_is_not_a_widget_namespaced_element_then_package_is_invalid()
        {
            ConfigFileParser parser = new ConfigFileParser();
            Assert.Throws<ConfigFileParseException>(() => parser.Parse(TestHelper.GetConfigurationFileAsXDocument("WidgetElementNotInWidgetNamespace")));
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementBasedLocalizationConformance.cs">
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

namespace Widgt.Core.Tests.Localization
{
    using NUnit.Framework;
    
    using Widgt.Core.Model;

    /// <summary>
    /// Conformance tests for section 8.4 of the specification
    /// </summary>
    [TestFixture]
    public class ElementBasedLocalizationConformance
    {
        /// <summary>
        /// Verifies that a lang attribute of en is used when the user agent is set to en-US
        /// </summary>
        [Test]
        public void Description_localized_to_first_matching_en()
        {
            Widget w = TestHelper.GetWidgetFromFile("ElementLocalizationDescription");
            var descriptions = w.LocalizedTo("en-US").Descriptions;

            Assert.That(descriptions.Count, Is.EqualTo(1));
            Assert.That(descriptions[0].Text, Is.EqualTo("This element would be used."));
        }

        /// <summary>
        /// Default locale used when no match is found
        /// </summary>
        [Test]
        public void Description_localized_to_first_matching_default()
        {
            Widget w = TestHelper.GetWidgetFromFile("ElementLocalizationDescription");
            var descriptions = w.LocalizedTo("fr-FR").Descriptions;

            Assert.That(descriptions.Count, Is.EqualTo(1));
            Assert.That(descriptions[0].Text, Is.EqualTo("This element is unlocalized, and would be used if the user agent's\n        locale does not match any localized description elements."));
        }
    }
}

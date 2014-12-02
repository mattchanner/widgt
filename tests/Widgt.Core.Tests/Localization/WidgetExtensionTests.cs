// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetLocalizationTests.cs">
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
    /// A set of tests for verifying the localization aspects of a widget instance
    /// </summary>
    [TestFixture]
    public class WidgetLocalizationTests
    {
        /// <summary>
        /// Tests for the localize method
        /// </summary>
        public class TheLocalizedToMethod
        {
            private static readonly string[] Languages = { "en-GB", "en-US", "de-DE", "en", string.Empty };

            /// <summary>
            /// Tests the description collection can be localized
            /// </summary>
            [Test]
            public void It_can_localize_descriptions()
            {
                Widget w = new Widget();

                foreach (string lang in Languages)
                    w.Descriptions.Add(new Description { Language = lang, Text = lang + " Text" });

                Widget localized = w.LocalizedTo("en-GB");
                Assert.That(localized.Descriptions.Count, Is.EqualTo(1));
                Assert.That(localized.Descriptions[0].Language, Is.EqualTo("en-GB"));
            }

            /// <summary>
            /// Tests the names collection can be localized
            /// </summary>
            [Test]
            public void It_can_localize_names()
            {
                Widget w = new Widget();

                foreach (string lang in Languages)
                    w.Names.Add(new Name { Language = lang, Value = lang + " Value" });

                Widget localized = w.LocalizedTo("en-GB");
                Assert.That(localized.Names.Count, Is.EqualTo(1));
                Assert.That(localized.Names[0].Language, Is.EqualTo("en-GB"));
            }

            /// <summary>
            /// Tests the icons collection can be localized
            /// </summary>
            [Test]
            public void It_can_localize_icons()
            {
                Widget w = new Widget();

                foreach (string lang in Languages)
                    w.Icons.Add(new Icon { Language = lang, Src = lang + " Src" });

                Widget localized = w.LocalizedTo("en-GB");
                Assert.That(localized.Icons.Count, Is.EqualTo(1));
                Assert.That(localized.Icons[0].Language, Is.EqualTo("en-GB"));
            }

            /// <summary>
            /// Tests the icons collection can be localized
            /// </summary>
            [Test]
            public void It_can_localize_preferences()
            {
                Widget w = new Widget();

                foreach (string lang in Languages)
                    w.Preferences.Add(new Preference { Language = lang, Name = lang + " Name", Value = lang + " value" });

                Widget localized = w.LocalizedTo("en-GB");
                Assert.That(localized.Preferences.Count, Is.EqualTo(1));
                Assert.That(localized.Preferences[0].Language, Is.EqualTo("en-GB"));
            }

            /// <summary>
            /// Tests the icons collection can be localized
            /// </summary>
            [Test]
            public void It_can_localize_features()
            {
                Widget w = new Widget();

                foreach (string lang in Languages)
                    w.Features.Add(new FeatureRequest { Language = lang, Name = lang + " Feature" });

                Widget localized = w.LocalizedTo("en-GB");
                Assert.That(localized.Features.Count, Is.EqualTo(1));
                Assert.That(localized.Features[0].Language, Is.EqualTo("en-GB"));
            }

            /// <summary>
            /// Tests the license collection can be localized
            /// </summary>
            [Test]
            public void It_can_localize_license()
            {
                Widget w = new Widget();

                foreach (string lang in Languages)
                    w.Licenses.Add(new License { Language = lang, Contents = lang + "contents", HRef = lang + " href" });

                Widget localized = w.LocalizedTo("en-GB");
                Assert.That(localized.Licenses.Count, Is.EqualTo(1));
                Assert.That(localized.Licenses[0].Language, Is.EqualTo("en-GB"));
            }

            /// <summary>
            /// Tests that the content collection can be localized
            /// </summary>
            [Test]
            public void It_can_localize_content()
            {
                Widget w = new Widget();

                foreach (string lang in Languages)
                    w.Contents.Add(new Content { Src = lang + "src", Language = lang });

                Widget localized = w.LocalizedTo("en-GB");
                Assert.That(localized.Contents.Count, Is.EqualTo(1));
                Assert.That(localized.Contents[0].Language, Is.EqualTo("en-GB"));
            }

            /// <summary>
            /// Verifies the non localizable content is copied across when localizing a widget
            /// </summary>
            [Test]
            public void It_copies_non_localized_content()
            {
                Widget w = new Widget
                {
                    Author = new Author { HRef = "href", Email = "email", Contents = "Contents" },
                    DefaultLocale = "cn",
                    Height = 200,
                    Width = 300,
                    WidgetId = "http://example/widget",
                    Version = "1.0",
                    ViewModes = { "fullscreen" }
                };
                w.AccessRequests.Add(new AccessRequest { Origin = "http://example.com", Subdomains = true });

                Widget localized = w.LocalizedTo("en-GB");

                Assert.That(localized.Author.Contents, Is.EqualTo("Contents"));
                Assert.That(localized.Author.Email, Is.EqualTo("email"));
                Assert.That(localized.Author.HRef, Is.EqualTo("href"));
                Assert.That(localized.DefaultLocale, Is.EqualTo("cn"));
                Assert.That(localized.Height, Is.EqualTo(200));
                Assert.That(localized.Width, Is.EqualTo(300));
                Assert.That(localized.Version, Is.EqualTo("1.0"));
                Assert.That(localized.WidgetId, Is.EqualTo("http://example/widget"));
                Assert.That(localized.ViewModes.Count, Is.EqualTo(1));
                Assert.That(localized.ViewModes[0], Is.EqualTo("fullscreen"));
                Assert.That(localized.AccessRequests.Count, Is.EqualTo(1));
                Assert.That(localized.AccessRequests[0].Subdomains, Is.True);
                Assert.That(localized.AccessRequests[0].Origin, Is.EqualTo("http://example.com"));
            }
        }
    }
}

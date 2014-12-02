// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConformanceNonLocalized.cs">
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
    using NUnit.Framework;

    using Widgt.Core.Model;

    /// <summary>
    /// Tests for basic conformance to the Widget definition.  These tests do not include
    /// any localized data
    /// </summary>
    [TestFixture]
    public class ConformanceNonLocalized
    {
        /// <summary> The widget under test </summary>
        private Widget widget;

        /// <summary>
        /// Sets up the test by parsing a new widget instance
        /// </summary>
        [SetUp]
        public void TestSetUp()
        {
            widget = TestHelper.GetWidgetFromFile("W3NonLocalizedExample");
        }

        /// <summary>
        /// Verifies the main elements of a widget ID can be parsed
        /// </summary>
        [Test]
        public void Can_parse_id()
        {
            Assert.That(widget.WidgetId, Is.EqualTo("http://example.org/exampleWidget"));
        }

        /// <summary>
        /// Verifies the main elements of a widget version can be parsed
        /// </summary>
        [Test]
        public void Can_parse_version()
        {
            Assert.That(widget.Version, Is.EqualTo("2.0 Beta"));
        }

        /// <summary>
        /// Tests that the access requests can be parsed
        /// </summary>
        [Test]
        public void Can_parse_access_requests()
        {
            Assert.That(widget.AccessRequests.Count, Is.EqualTo(2));
            Assert.That(widget.AccessRequests[0].Subdomains, Is.False);
            Assert.That(widget.AccessRequests[1].Subdomains, Is.True);

            Assert.That(widget.AccessRequests[0].Origin, Is.EqualTo("http://foo.example.com"));
            Assert.That(widget.AccessRequests[1].Origin, Is.EqualTo("http://example.com:4242"));
        }

        /// <summary>
        /// Verifies the main elements of a widget height can be parsed
        /// </summary>
        [Test]
        public void Can_parse_height()
        {
            Assert.That(widget.Height, Is.EqualTo(200));
        }

        /// <summary>
        /// Verifies the main elements of a widget height can be parsed
        /// </summary>
        [Test]
        public void Can_parse_width()
        {
            Assert.That(widget.Width, Is.EqualTo(400));
        }

        /// <summary>
        /// Verifies the main elements of a widget name can be parsed
        /// </summary>
        [Test]
        public void Can_parse_name()
        {
            Assert.That(widget.Names.Count, Is.EqualTo(1));
            Assert.That(widget.Names[0].Short, Is.EqualTo("Example 2.0"));
            Assert.That(widget.Names[0].Value, Is.EqualTo("The example Widget!"));
        }

        /// <summary>
        /// Verifies a feature can be parsed
        /// </summary>
        [Test]
        public void Can_parse_feature()
        {
            Assert.That(widget.Features.Count, Is.EqualTo(1));
            Assert.That(widget.Features[0].Name, Is.EqualTo("http://example.com/camera"));
        }

        /// <summary>
        /// Verifies a feature can be parsed
        /// </summary>
        [Test]
        public void Parsed_feature_defaults_to_required()
        {
            Assert.That(widget.Features.Count, Is.EqualTo(1));
            Assert.That(widget.Features[0].Required, Is.True);
        }

        /// <summary>
        /// Verifies a feature can be parsed
        /// </summary>
        [Test]
        public void Can_parse_feature_params()
        {
            Assert.That(widget.Features.Count, Is.EqualTo(1));
            Assert.That(widget.Features[0].Parameters.Count, Is.EqualTo(1));
            Assert.That(widget.Features[0].Parameters[0].Name, Is.EqualTo("autofocus"));
            Assert.That(widget.Features[0].Parameters[0].Value, Is.EqualTo("true"));
        }

        /// <summary>
        /// Verifies the preferences can be parsed
        /// </summary>
        [Test]
        public void Can_parse_preferences()
        {
            Assert.That(widget.Preferences.Count, Is.EqualTo(1));
            Assert.That(widget.Preferences[0].Name, Is.EqualTo("apikey"));
            Assert.That(widget.Preferences[0].Value, Is.EqualTo("ea31ad3a23fd2f"));
            Assert.That(widget.Preferences[0].Readonly, Is.True);
        }

        /// <summary>
        /// Verifies the single viewmode in the example can be parsed
        /// </summary>
        [Test]
        public void Can_parse_viewmode_single()
        {
            Assert.That(widget.ViewModes.Count, Is.EqualTo(1));
            Assert.That(widget.ViewModes[0], Is.EqualTo("fullscreen"));
        }

        /// <summary>
        /// Verifies the description can be parsed
        /// </summary>
        [Test]
        public void Can_parse_description()
        {
            Assert.That(widget.Descriptions.Count, Is.EqualTo(1));
            Assert.That(widget.Descriptions[0].Text, Is.EqualTo("A sample widget to demonstrate some of the possibilities."));
        }

        /// <summary>
        /// Verifies the author element can be parsed
        /// </summary>
        [Test]
        public void Can_parse_author()
        {
            Assert.That(widget.Author, Is.Not.Null);
            Assert.That(widget.Author.HRef, Is.EqualTo("http://foo-bar.example.org/"));
            Assert.That(widget.Author.Email, Is.EqualTo("foo-bar@example.org"));
            Assert.That(widget.Author.Contents, Is.EqualTo("Foo Bar Corp"));
        }

        /// <summary>
        /// Tests that the icon list can be parsed
        /// </summary>
        [Test]
        public void Can_parse_icons()
        {
            Assert.That(widget.Icons.Count, Is.EqualTo(2));
            Assert.That(widget.Icons[0].Src, Is.EqualTo("icons/example.png"));
            Assert.That(widget.Icons[1].Src, Is.EqualTo("icons/boo.png"));
        }

        /// <summary>
        /// Verifies the content block can be parsed
        /// </summary>
        [Test]
        public void Can_parse_content()
        {
            Assert.That(widget.Contents[0].Src, Is.EqualTo("myWidget.html"));
        }

        /// <summary>
        /// Verifies the license for the widget can be parsed
        /// </summary>
        [Test]
        public void Can_parse_license()
        {
            const string Expected = @"Example license (based on MIT License)
        Copyright (c) 2008 The Foo Bar Corp.
        THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS
        OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
        MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
        IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
        CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
        INSULT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
        SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.";

            Assert.That(widget.Licenses[0].Contents, Is.EqualTo(Expected.Replace("\r", string.Empty)));
            Assert.That(widget.Licenses[0].HRef, Is.EqualTo("http://opensource.org/licenses/MIT"));
        }
    }
}

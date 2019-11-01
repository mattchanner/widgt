// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviourTests.cs">
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
    using System;
    using System.IO;
    using System.Xml.Linq;

    using NUnit.Framework;

    using Widgt.Core;
    using Widgt.Core.Exceptions;
    using Widgt.Core.Model;

    /// <summary>
    /// A set of general behaviour tests based in inputs and output
    /// </summary>
    [TestFixture]
    public class BehaviourTests
    {
        /// <summary>
        /// Expects an <see cref="ArgumentNullException"/> to be raised when given a null document to parse
        /// </summary>
        [Test]
        public void Throws_ArgumentNullException_When_Passed_A_Null_Document()
        {
            ConfigFileParser parser = new ConfigFileParser();
            Assert.Throws<ArgumentNullException>(() => parser.Parse((XDocument)null));
        }

        /// <summary>
        /// Expects an <see cref="ArgumentNullException"/> to be raised when given a null document to parse
        /// </summary>
        [Test]
        public void Throws_ArgumentNullException_When_Passed_A_Null_InputStream()
        {
            ConfigFileParser parser = new ConfigFileParser();
            Assert.Throws<ArgumentNullException>(() => parser.Parse((Stream)null));
        }

        /// <summary>
        /// Given an empty document, the parser throws a <see cref="ConfigFileParseException"/>
        /// </summary>
        [Test]
        public void Throws_WidgetParserException_When_Passed_An_Empty_Document()
        {
            ConfigFileParser parser = new ConfigFileParser();
            Assert.Throws<ConfigFileParseException>(() => parser.Parse(new XDocument()));
        }

        /// <summary>
        /// Given a document with a different root node, the parser throws a <see cref="ConfigFileParseException"/>
        /// </summary>
        [Test]
        public void Throws_WidgetParserException_When_Passed_A_Different_Document()
        {
            ConfigFileParser parser = new ConfigFileParser();
            const string Xml = "<wrong_root xmlns=\"http://www.w3.org/ns/widgets\" " +
                                            "id=\"widget_id\"></wrong_root>";

            Assert.Throws<ArgumentNullException>(() => parser.Parse(XDocument.Parse(Xml)));
        }
    }
}

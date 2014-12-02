// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalizrTests.cs">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Widgt.Core.Model;
    using Widgt.Core.Utils;

    /// <summary>
    /// A set of tests for the <see cref="Localizr"/> class
    /// </summary>
    [TestFixture]
    public class LocalizrTests
    {
        /// <summary>
        /// Localizr tests
        /// </summary>
        public class TheLocalizrClass
        {
            /// <summary>
            /// Null locale string == argument null exception
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void It_throws_an_ArgumentNullException_when_locale_string_is_null()
            {
                Localizr.For(null);
            }

            /// <summary>
            /// Empty locale string == argument exception
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void It_throws_an_ArgumentException_when_locale_string_is_empty()
            {
                Localizr.For(string.Empty);
            }

            /// <summary>
            /// Given a locale of en-GB, a list of ["", "de-DE", "en", "en-GB", "en-US"] should return
            /// ["en-GB", "en", ""] in that order
            /// </summary>
            [Test]
            public void It_provies_3_matches_in_order_when_given_options()
            {
                var input = new List<LanguageAwareTestClass>();
                input.Add(new LanguageAwareTestClass(string.Empty)); // least specific match
                input.Add(new LanguageAwareTestClass("de-DE"));
                input.Add(new LanguageAwareTestClass("en-GB")); // exact
                input.Add(new LanguageAwareTestClass("en")); // partial
                input.Add(new LanguageAwareTestClass("en-")); // not a partial
                input.Add(new LanguageAwareTestClass("en-US"));

                var matches = Localizr.For("en-GB").BestMatches(input).ToList();
                Assert.That(matches.Count, Is.EqualTo(3));
                Assert.That(matches[0].Language, Is.EqualTo("en-GB"));
                Assert.That(matches[1].Language, Is.EqualTo("en"));
                Assert.That(matches[2].Language, Is.EqualTo(string.Empty));
            }
        }

        /// <summary>
        /// Tests for the <see cref="LocaleName"/> class
        /// </summary>
        public class TheLocaleNameClass
        {
            /// <summary>
            /// Null locale string == argument null exception
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void It_throws_an_ArgumentNullException_when_locale_string_is_null()
            {
                // ReSharper disable once UnusedVariable
                var locale = new LocaleName(null);
            }

            /// <summary>
            /// Empty locale string == argument exception
            /// </summary>
            [Test]
            [ExpectedException(typeof(ArgumentException))]
            public void It_throws_an_ArgumentException_when_locale_string_is_empty()
            {
                // ReSharper disable once UnusedVariable
                var locale = new LocaleName(string.Empty);
            }

            /// <summary>
            /// ToString override returns original string
            /// </summary>
            [Test]
            public void It_lower_cases_the_string()
            {
                Assert.That(new LocaleName("en-GB").ToString(), Is.EqualTo("en-gb"));
            }

            /// <summary>
            /// Verifies the LocaleName has structural equality, not referential equality
            /// </summary>
            [Test]
            public void It_has_structural_equality()
            {
                Assert.That(new LocaleName("en-GB"), Is.EqualTo(new LocaleName("en-GB")));
            }

            /// <summary>
            /// Verifies that equals does its job for non equal locale names
            /// </summary>
            [Test]
            public void It_returns_false_when_second_locale_is_different()
            {
                Assert.That(new LocaleName("en-GB"), Is.Not.EqualTo(new LocaleName("de-DE")));
            }

            /// <summary>
            /// Exact match is true for the same locales
            /// </summary>
            [Test]
            public void It_exactly_matches_a_locale_with_same_value()
            {
                Assert.That(new LocaleName("en-GB").ExactlyMatches(new LocaleName("en-GB")), Is.True);
            }

            /// <summary>
            /// Exact match is true for the same locales
            /// </summary>
            [Test]
            public void It_exactly_matches_a_locale_with_same_value_and_different_case()
            {
                Assert.That(new LocaleName("en-GB").ExactlyMatches(new LocaleName("en-gb")), Is.True);
            }

            /// <summary>
            /// Exact match is false for the different locales
            /// </summary>
            [Test]
            public void It_does_not_exactly_matches_a_locale_with_different_values()
            {
                Assert.That(new LocaleName("en-GB").ExactlyMatches(new LocaleName("de-DE")), Is.False);
            }

            /// <summary>
            /// Partial match works when given the language part
            /// </summary>
            [Test]
            public void It_partially_matches_a_partial_locale_name()
            {
                Assert.That(new LocaleName("en-GB").PartiallyMatches(new LocaleName("en")), Is.True);
            }

            /// <summary>
            /// Partial match works when given the language part
            /// </summary>
            [Test]
            public void It_does_not_partially_match_an_invalid_partial_locale_name_1()
            {
                Assert.That(new LocaleName("en-GB").PartiallyMatches(new LocaleName("en-")), Is.False);
            }

            /// <summary>
            /// Partial match works when given the language part
            /// </summary>
            [Test]
            public void It_does_not_partially_match_an_invalid_partial_locale_name_2()
            {
                Assert.That(new LocaleName("en-GB").PartiallyMatches(new LocaleName("en-GB")), Is.False);
            }

            /// <summary>
            /// Tests for the equality operator
            /// </summary>
            [Test]
            public void Equality_operator_works()
            {
                // ReSharper disable once EqualExpressionComparison
                Assert.IsTrue(new LocaleName("en-GB") == new LocaleName("en-GB"));
                Assert.IsFalse(new LocaleName("en-GB") == new LocaleName("en-US"));
                Assert.IsFalse(new LocaleName("en-GB") == new LocaleName("en"));
                Assert.IsFalse(new LocaleName("en-GB") == null);
                Assert.IsFalse(null == new LocaleName("en"));
            }

            /// <summary>
            /// Tests for the equality operator
            /// </summary>
            [Test]
            public void Inequality_operator_works()
            {
                Assert.IsTrue(new LocaleName("en-GB") != new LocaleName("en-US"));
                // ReSharper disable once EqualExpressionComparison
                Assert.IsFalse(new LocaleName("en-GB") != new LocaleName("en-GB"));
                Assert.IsTrue(new LocaleName("en-GB") != new LocaleName("en"));
                Assert.IsTrue(new LocaleName("en-GB") != null);
                Assert.IsTrue(null != new LocaleName("en"));
            }
        }

        /// <summary>
        /// Class used for testing languages
        /// </summary>
        private class LanguageAwareTestClass : ILanguageAware
        {
            public LanguageAwareTestClass(string lang)
            {
                this.Language = lang;
            }

            public string Language { get; set; }
        }
    }
}

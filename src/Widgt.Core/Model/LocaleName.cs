// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocaleName.cs">
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

using System;

namespace Widgt.Core.Model
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a locale name
    /// </summary>
    public class LocaleName
    {
        /// <summary> The locale delimiter </summary>
        private const char Delimiter = '-';

        /// <summary> The char array to split on when parsing a language header </summary>
        private static readonly char[] AcceptLanguageSplitChars = ",".ToCharArray();

        /// <summary> The original language string </summary>
        private readonly string originalLocale;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleName"/> class.
        /// </summary>
        /// <param name="localeString"> The locale string. </param>
        public LocaleName(string localeString)
        {
            if (localeString == null) throw new ArgumentNullException("localeString");
            if (localeString.Length == 0) throw new ArgumentException("Value cannot be empty", "localeString");

            // ensure locale strings are consistently delimited
            this.originalLocale = localeString.Replace("_", "-").ToLowerInvariant();
        }

        /// <summary>
        /// Gets the string value that this locale represents
        /// </summary>
        public string Value
        {
            get
            {
                return this.originalLocale;
            }
        }

        /// <summary>
        /// Implicit case from a string to a<see cref="LocaleName"/>
        /// </summary>
        /// <param name="localeNameString">The locale name string to convert`</param>
        /// <returns>The casted <see cref="LocaleName"/></returns>
        public static implicit operator LocaleName(string localeNameString)
        {
            return new LocaleName(localeNameString);
        }

        /// <summary>
        /// This probably naive approach is to take a string of the form:
        /// en-GB,en;q=0.8
        /// Remove the semicolon and anything after it:
        /// en-GB,en
        /// Split on the comma to give an array:
        /// [en-GB, en]
        /// Assume this is preference order (effectively and probably incorrectly, ignoring the q weighting)
        /// </summary>
        /// <param name="acceptLanguageHeader">The input string to parse</param>
        /// <returns>A sequence of LocaleName instances representing each of the preferred languages</returns>
        public static IEnumerable<LocaleName> ParseLanguageHeader(string acceptLanguageHeader)
        {
            if (string.IsNullOrEmpty(acceptLanguageHeader)) 
                return Enumerable.Empty<LocaleName>();

            int colonPosition = acceptLanguageHeader.IndexOf(";", StringComparison.Ordinal);

            if (colonPosition > 0)
            {
                acceptLanguageHeader = acceptLanguageHeader.Substring(0, colonPosition);
            }

            string[] preferences = acceptLanguageHeader.Trim().Split(AcceptLanguageSplitChars, StringSplitOptions.RemoveEmptyEntries);

            // Project the string array to a sequence of locale names
            return preferences.Select(lang => new LocaleName(lang));
        }

        /// <summary>
        /// Equality operator override
        /// </summary>
        /// <param name="left"> The left hand side of the binary expression. </param>
        /// <param name="right"> The right hand side of the binary expression. </param>
        /// <returns>True if equal, false if not </returns>
        public static bool operator ==(LocaleName left, LocaleName right)
        {
            if (ReferenceEquals(null, left)) return ReferenceEquals(null, right);
            return !ReferenceEquals(null, right) && left.ExactlyMatches(right);
        }

        /// <summary>
        /// Inequality operator override
        /// </summary>
        /// <param name="left"> The left hand side of the binary expression. </param>
        /// <param name="right"> The right hand side of the binary expression. </param>
        /// <returns>False if equal, true if not </returns>
        public static bool operator !=(LocaleName left, LocaleName right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a value indicating whether this instance exactly matches the given locale string
        /// </summary>
        /// <param name="localeName">The locale name to match against</param>
        /// <returns>True if this instance exactly matches the locale name provided</returns>
        public bool ExactlyMatches(LocaleName localeName)
        {
            return string.Compare(this.originalLocale, localeName.originalLocale, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns a value indicating whether this locale instance partially matches the given locale name, for example, 
        /// LocaleName("en-GB").PartiallyMatches("en") == true (partial match)
        /// LocaleName("en-GB").PartiallyMatches("en-GB") == false (exact match)
        /// LocaleName("en-GB").PartiallyMatches("en-US") == false (inexact match)
        /// </summary>
        /// <param name="localeName">The locale name to match against</param>
        /// <returns>True if this locale partially matches the locale name provided</returns>
        public bool PartiallyMatches(LocaleName localeName)
        {
            if (localeName == null) throw new ArgumentNullException("localeName");

            string localeString = localeName.ToString().ToLowerInvariant();

            return localeString.Length < this.ToString().Length
                   && localeString.IndexOf(Delimiter) < 0
                   && this.originalLocale.StartsWith(localeString);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return originalLocale;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is LocaleName && ExactlyMatches((LocaleName)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.originalLocale.GetHashCode();
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetExtensions.cs">
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
    using System.Linq;

    using Widgt.Core.Utils;

    /// <summary>
    /// Extensions for the Widget type and associated data
    /// </summary>
    public static class WidgetExtensions
    {
        /// <summary>
        /// Searches a sequence of language aware types for the first type with a language
        /// that matches the given language specifier.  If the search yields no matches, null will be returned
        /// </summary>
        /// <param name="instance"> The instance to extend. </param>
        /// <param name="locale"> The locale name to search by. </param>
        /// <typeparam name="T"> The type implementing the <see cref="ILanguageAware"/> interface</typeparam>
        /// <returns> The <see cref="T"/> matching the given language, or null. </returns>
        public static T FindByLanguage<T>(this IEnumerable<T> instance, string locale) where T : ILanguageAware
        {
            return instance == null
                       ? default(T)
                       : instance.FirstOrDefault(t => string.CompareOrdinal(t.Language, locale) == 0);
        }

        /// <summary>
        /// Finds the first instance in the sequence where the language attribute is null or an empty string
        /// </summary>
        /// <typeparam name="T">The type implementing the <see cref="ILanguageAware"/> interface</typeparam>
        /// <param name="instance">The instance to extend</param>
        /// <returns>The first element whose language attribute is null or an empty string, or the default
        /// for the type when no element is found (this will be null for classes)</returns>
        public static T FindDefault<T>(this IEnumerable<T> instance) where T : ILanguageAware
        {
            return instance == null ? default(T) : instance.FirstOrDefault(t => string.IsNullOrEmpty(t.Language));
        }

        /// <summary>
        /// Finds the best matching instance in the collection based on the given language requested
        /// </summary>
        /// <typeparam name="T">The type implementing the <see cref="ILanguageAware"/> interface</typeparam>
        /// <param name="instance">The instance to extend</param>
        /// <param name="locale">The language to look up</param>
        /// <returns>The best matching instance based on language specifier</returns>
        public static T FindByBestMatchingLocale<T>(this IEnumerable<T> instance, LocaleName locale) where T : ILanguageAware
        {
            return Localizr.For(locale).BestMatches(instance).FirstOrDefault();
        }

        /// <summary>
        /// Returns a localized instance of the widget, using the given locale name as a way of
        /// selecting the correct element in each of the locale aware collections
        /// </summary>
        /// <param name="inputWidget">The source widget to extend</param>
        /// <param name="localeName">The locale name to query by</param>
        /// <returns>The localized version of the widget</returns>
        public static Widget LocalizedTo(this Widget inputWidget, LocaleName localeName)
        {
            if (inputWidget == null) throw new ArgumentNullException();
            if (localeName == null) throw new ArgumentNullException();

            Widget localized = new Widget();

            localized.Author = inputWidget.Author;
            localized.DefaultLocale = inputWidget.DefaultLocale;
            localized.Height = inputWidget.Height;
            localized.WidgetId = inputWidget.WidgetId;
            localized.Version = inputWidget.Version;
            localized.Width = inputWidget.Width;

            foreach (AccessRequest ar in inputWidget.AccessRequests) localized.AccessRequests.Add(ar);

            if (inputWidget.ViewModes != null && inputWidget.ViewModes.Count > 0)
            {
                foreach (string viewmode in inputWidget.ViewModes) localized.ViewModes.Add(viewmode);
            }

            AddLocalized(inputWidget.Descriptions, localized.Descriptions, localeName);
            AddLocalized(inputWidget.Icons, localized.Icons, localeName);
            AddLocalized(inputWidget.Names, localized.Names, localeName);
            AddLocalized(inputWidget.Preferences, localized.Preferences, localeName);
            AddLocalized(inputWidget.Features, localized.Features, localeName);
            AddLocalized(inputWidget.Licenses, localized.Licenses, localeName);
            AddLocalized(inputWidget.Contents, localized.Contents, localeName);

            return localized;
        }

        private static void AddLocalized<T>(IEnumerable<T> source, ICollection<T> destination, LocaleName locale) where T : class, ILanguageAware
        {
            T match = source.FindByBestMatchingLocale(locale);
            if (match != null)
                destination.Add(match);
        }
    }
}


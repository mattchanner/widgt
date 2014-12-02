// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Localizr.cs">
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

namespace Widgt.Core.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Widgt.Core.Exceptions;
    using Widgt.Core.Model;

    /// <summary>
    /// This class contains a number of static methods used to aide in looking up localized language elements
    /// </summary>
    public static class Localizr
    {
        /// <summary>
        /// Represents the interface for folder based localizers to implement
        /// </summary>
        public interface IFolderBasedLocalizrLookup
        {
            /// <summary>
            /// Given a root directory for a widget and the original file relative to this, this method should attempt
            /// to find a best matching alternative file in a locale folder based on the supplied sequence of locales
            /// provided to the implementing component
            /// </summary>
            /// <param name="rootWidgetDirectory">The root widget directory</param>
            /// <param name="originalFile">The original file requested which should be relative to the top level 
            /// folder</param>
            /// <returns>Either the original file in the case where a best match cannot be found, or a localized
            /// variant based on the locale preferences.  Note that the original file is not guaranteed to exist, but
            /// the localized one will be</returns>
            FileInfo FindBestMatchingFolder(DirectoryInfo rootWidgetDirectory, FileInfo originalFile);
        }

        /// <summary>
        /// The lookup interface.
        /// </summary>
        public interface ILocalizrLookup
        {
            /// <summary>
            /// Given an enumeration containing language aware elements, this method returns an ordered list of
            /// matches based on most specific first, and least specific last.
            /// The most specific will be those instances who match exactly the current language that this localization 
            /// instance represents.  The least specific will be those with no locale specifier at all.
            /// </summary>
            /// <param name="items"> The items to match on. </param>
            /// <typeparam name="T"> The type of items in the sequence who implement the <see cref="ILanguageAware"/> 
            /// interface </typeparam>
            /// <returns>
            /// The <see cref="IEnumerable"/>. containing those matching elements in order of specificity
            /// </returns>
            IEnumerable<T> BestMatches<T>(IEnumerable<T> items) where T : ILanguageAware;
        }

        /// <summary>
        /// Returns an <see cref="ILocalizrLookup"/> for the given language
        /// </summary>
        /// <param name="locale">The locale name to provide localization lookup resources for</param>
        /// <returns>The localization lookup implementation for the given locale</returns>
        public static ILocalizrLookup For(string locale)
        {
            return new LocalizrLookup(new LocaleName(locale));
        }

        /// <summary>
        /// Returns an implementation of the <see cref="IFolderBasedLocalizrLookup"/> for the given sequence
        /// of locale names.  Note the given sequence should be in order of preference
        /// </summary>
        /// <param name="locales">The locales to look for</param>
        /// <returns>The <see cref="IFolderBasedLocalizrLookup"/> implementation</returns>
        public static IFolderBasedLocalizrLookup FolderLookup(IEnumerable<LocaleName> locales)
        {
            return new FolderBasedLocalizrLookup(locales);   
        }

        /// <summary>
        /// Returns an <see cref="ILocalizrLookup"/> for the given language
        /// </summary>
        /// <param name="locale">The locale name to provide localization lookup resources for</param>
        /// <returns>The localization lookup implementation for the given locale</returns>
        public static ILocalizrLookup For(LocaleName locale)
        {
            return new LocalizrLookup(locale);
        }

        private sealed class FolderBasedLocalizrLookup : IFolderBasedLocalizrLookup
        {
            private const string LocaleFolderName = "locales";

            private List<LocaleName> localeList;
 
            public FolderBasedLocalizrLookup(IEnumerable<LocaleName> locales)
            {
                Throwable.ThrowIfNull(locales, "locales");
         
                this.localeList = new List<LocaleName>(locales);
            }

            /// <inheritdoc />
            public FileInfo FindBestMatchingFolder(DirectoryInfo rootWidgetDirectory, FileInfo originalFile)
            {
                // If there is no accept language, return the original file
                if (localeList.Count == 0) return originalFile;

                DirectoryInfo localeFolder = new DirectoryInfo(Path.Combine(rootWidgetDirectory.FullName, LocaleFolderName));

                // Make sure the file is not already localized (may have already been redirected here)
                if (originalFile.FullName.StartsWith(localeFolder.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return originalFile;

                string partialPathToResource = originalFile.FullName.Substring(rootWidgetDirectory.FullName.Length);

                // Remove a leading slash as this will cause Path.Combine to return this as a rooted path
                if (partialPathToResource.StartsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
                {
                    partialPathToResource = partialPathToResource.Substring(1);
                }

                // If there is no locales folder, there is nothing we can do but return the original file
                if (!localeFolder.Exists) return originalFile;

                foreach (LocaleName language in this.localeList)
                {
                    string contender = Path.Combine(Path.Combine(localeFolder.FullName, language.Value), partialPathToResource);
                    FileInfo localeFile = new FileInfo(contender);
                    if (localeFile.Exists) return localeFile;
                }

                return originalFile;
            }
        }

        /// <summary> The standard implementation of the lookup resource</summary>
        private sealed class LocalizrLookup : ILocalizrLookup
        {
            private readonly LocaleName locale;

            internal LocalizrLookup(LocaleName locale)
            {
                this.locale = locale;
            }

            /// <inheritdoc />
            public IEnumerable<T> BestMatches<T>(IEnumerable<T> items) where T : ILanguageAware
            {
                var collector = new SortedList<int, T>();

                foreach (T item in items)
                {
                    if (string.IsNullOrEmpty(item.Language))
                    {
                        // Item has no language, so by definition will match.  Give this the lowest weighting
                        if (!collector.ContainsKey(2)) collector.Add(2, item);
                    }
                    else
                    {
                        LocaleName itemLocale = new LocaleName(item.Language);
                        if (this.locale.ExactlyMatches(itemLocale))
                        {
                            if (!collector.ContainsKey(0)) collector.Add(0, item);
                        }
                        else if (this.locale.PartiallyMatches(itemLocale))
                        {
                            if (!collector.ContainsKey(1)) collector.Add(1, item);
                        }
                    }
                }

                return collector.Values;
            }
        }
    }
}

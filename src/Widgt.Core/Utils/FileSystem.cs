// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystem.cs">
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
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    using Widgt.Core.Exceptions;

    /// <summary>
    /// A set of helper methods relating to the file system
    /// </summary>
    public static class FileSystem
    {
        /// <summary> The character to use when replacing invalid file system characters </summary>
        private const char ReplacementChar = '_';

        /// <summary>
        /// Creates and returns an MD5 checksum for the given file
        /// </summary>
        /// <param name="file">The file to checksum</param>
        /// <returns>The resulting checksum</returns>
        public static string Checksum(FileInfo file)
        {
            Throwable.ThrowIfNull(file, "file");
            
            using (Stream fileStream = file.OpenRead())
            {
                return Encoding.UTF8.GetString(MD5.Create().ComputeHash(fileStream));
            }
        }

        /// <summary>
        /// This method is responsible for mapping an incoming request to a server file.
        /// </summary>
        /// <param name="baseServerDirectory">The base widget directory where widgets are deployed to</param>
        /// <param name="servicePrefix">The service prefix for widget requests</param>
        /// <param name="serverPathToMap">The server path to map to a physical file location</param>
        /// <returns>The mapped file, or null if the request does not map to a file</returns>
        public static FileInfo MapRequestPathToWidgetPath(DirectoryInfo baseServerDirectory, string servicePrefix, string serverPathToMap)
        {
            FileInfo serverFile = null;

            try
            {
                int deployIndex = serverPathToMap.IndexOf(servicePrefix + "/", StringComparison.InvariantCultureIgnoreCase);

                if (deployIndex >= 0)
                {
                    string relativePath = serverPathToMap.Substring(deployIndex + servicePrefix.Length + 1);
                    relativePath = relativePath.Replace("%20", " ").Replace('/', Path.DirectorySeparatorChar);
                    serverFile = new FileInfo(Path.Combine(baseServerDirectory.FullName, relativePath));
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteAsync(ex.ToString());
            }

            return serverFile;
        }

        /// <summary>
        /// Returns a file system friendly version of the given identifier.
        /// </summary>
        /// <param name="identifier">The identifier to use</param>
        /// <returns>The cleaned directory name</returns>
        public static string GetDirectoryNameForId(string identifier)
        {
            if (identifier.StartsWith("http://")) identifier = identifier.Substring(7);
            if (identifier.StartsWith("https://")) identifier = identifier.Substring(8);
            identifier = identifier.Replace('/', Path.DirectorySeparatorChar);

            char[] identifierChars = identifier.ToCharArray();

            ISet<char> invalidChars = new HashSet<char>(Path.GetInvalidPathChars());

            // prevent directory traversal
            invalidChars.Add('.');

            for (int charIndex = 0; charIndex < identifierChars.Length; charIndex++)
            {
                if (invalidChars.Contains(identifierChars[charIndex]))
                {
                    identifierChars[charIndex] = ReplacementChar;
                }
            }

            return new string(identifierChars);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentExtensions.cs">
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

using System.Collections.Generic;
using System.IO;

namespace Widgt.Owin
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Widgt.Core.Utils;

    // The SendFile server provided middleware function
    using SendFileFunc = System.Func<string /* File Name and path */,
             long /* file offset */,
             long? /* Byte count, null for remainder of file */,
             System.Threading.CancellationToken,
             System.Threading.Tasks.Task>;

    /// <summary>
    /// Some convenience extension methods to make acessing the environment data easier
    /// </summary>
    public static class EnvironmentExtensions
    {
        /// <summary>
        /// Sets the content type header to something more specific than text/plain in supported cases
        /// </summary>
        /// <param name="environment">The OWIN environment to set the header on</param>
        /// <param name="name">The name of the resource to infer the content type from</param>
        public static void SetContentTypeForFile(this IDictionary<string, object> environment, string name)
        {
            string contentType = DetermineContentType(name);
            environment.ResponseHeaders().Add("Content-Type", new[] { contentType });
        }

        /// <summary>
        /// Returns the response body from the environment
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <returns>The response body</returns>
        public static Stream ResponseBody(this IDictionary<string, object> environment)
        {
            return (Stream)environment[OwinKeys.ResponseBody];
        }

        /// <summary>
        /// Returns the request body from the environment
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <returns>The request body</returns>
        public static Stream RequestBody(this IDictionary<string, object> environment)
        {
            return (Stream)environment[OwinKeys.RequestBody];
        }

        /// <summary>
        /// Sends the file to the client using either the server provided send file function, or
        /// failing that, the function provided by the Streams helper class
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <param name="serverFile">The file to send to the client</param>
        /// <returns>The task to wait on</returns>
        public static async Task SendFile(this IDictionary<string, object> environment, FileInfo serverFile)
        {
            if (environment.ContainsKey(OwinKeys.SendFileAsync))
            {
                var providedFunction = (SendFileFunc)environment[OwinKeys.SendFileAsync];
                await providedFunction(serverFile.FullName, 0, null, new CancellationToken());
            }
            else
            {
                using (Stream fileStream = serverFile.OpenRead())
                {
                    await Streams.Copy(fileStream, environment.ResponseBody());
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="NameValueCollection"/> representing the query string parameters in the request
        /// </summary>
        /// <param name="environment">The environment to extend</param>
        /// <returns>The name value collection</returns>
        public static NameValueCollection RequestQueryString(this IDictionary<string, object> environment)
        {
            string queryString = (string)environment[OwinKeys.RequestQueryString];
            string[] argPairs = queryString.Split(new[] { '&' });

            NameValueCollection nvc = new NameValueCollection();
            foreach (string argPair in argPairs)
            {
                int eqIndex = argPair.IndexOf("=", StringComparison.InvariantCultureIgnoreCase);
                if (eqIndex > 0 && eqIndex < argPair.Length)
                {
                    string left = argPair.Substring(0, eqIndex);
                    string right = argPair.Substring(eqIndex + 1);
                    nvc.Add(left, right);
                }
            }

            return nvc;
        }

        /// <summary>
        /// Returns the request headers from the environment
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <returns>The request headers</returns>
        public static IEnumerable<KeyValuePair<string, string[]>> RequestHeaders(this IDictionary<string, object> environment)
        {
            return (IDictionary<string, string[]>)environment[OwinKeys.RequestHeaders];
        }

        /// <summary>
        /// Returns the response headers from the environment
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <returns>The response headers</returns>
        public static IDictionary<string, string[]> ResponseHeaders(this IDictionary<string, object> environment)
        {
            return (IDictionary<string, string[]>)environment[OwinKeys.ResponseHeaders];
        }

        /// <summary>
        /// Returns the first header value found in the request if present, otherwise an empty string is returned.
        /// </summary>
        /// <param name="environment">The environment instance to extend</param>
        /// <param name="headerName">The name of the header to find (case-insensitive)</param>
        /// <returns>The matching header, or an empty string if not found</returns>
        public static string RequestHeader(this IDictionary<string, object> environment, string headerName)
        {
            var headers = environment.RequestHeaders();
            var headerPair = headers.FirstOrDefault(
                                    pair => string.Compare(pair.Key, headerName, StringComparison.InvariantCultureIgnoreCase) == 0);
            string value = headerPair.Value != null && headerPair.Value.Length > 0 ? headerPair.Value[0] : string.Empty;
            return value;
        }

        /// <summary>
        /// Returns the request path from the environment
        /// </summary>
        /// <param name="environment">The OWIN environment</param>
        /// <returns>The request path</returns>
        public static string RequestPath(this IDictionary<string, object> environment)
        {
            return (string)environment[OwinKeys.RequestPath];
        }

        /// <summary>
        /// Very simple content type inference from feature resources, known to only be JS or CSS files
        /// </summary>
        /// <param name="fileResource">The file resource to infer</param>
        /// <returns>The inferred content type</returns>
        private static string DetermineContentType(string fileResource)
        {
            if (fileResource.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase)) return "application/javascript";
            if (fileResource.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase)) return "text/html";
            if (fileResource.EndsWith(".css", StringComparison.InvariantCultureIgnoreCase)) return "text/css";
            return "text/plain";
        }
    }
}

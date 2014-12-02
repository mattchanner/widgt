// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtApiClient.cs">
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

namespace Embedding.Client
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Widgt.Api.Shared;

    /// <summary>
    /// A simple API client for the Widgt REST service
    /// </summary>
    public class WidgtApiClient
    {
        private readonly Uri serverEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgtApiClient"/> class. 
        /// </summary>
        /// <param name="serverEndpoint"> The endpoint address to talk to </param>
        public WidgtApiClient(string serverEndpoint = "http://localhost:9000")
        {
            this.serverEndpoint = new Uri(serverEndpoint);
        }

        /// <summary>
        /// Gets the URI to the start file for the given widget
        /// </summary>
        /// <param name="widget">The widget to address</param>
        /// <returns>The inferred URI</returns>
        public Uri GetStartFilePathFor(WidgtDto widget)
        {
            return new Uri(serverEndpoint + widget.StartFilePath);
        }

        /// <summary>
        /// Gets all deployed widgets from the service
        /// </summary>
        /// <returns>A task which when run to completion, will return a list of deployed widgets</returns>
        public async Task<List<WidgtDto>> GetWidgets()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await client.GetAsync(new Uri(this.serverEndpoint + "/api/widgt"));
            var responseBody = await response.Content.ReadAsStreamAsync();
            return await FromJson<List<WidgtDto>>(responseBody);
        }

        /// <summary>
        /// De-serializes a JSON response into a typed instance 
        /// </summary>
        /// <typeparam name="T">The type to convert to</typeparam>
        /// <param name="s">The stream to read from</param>
        /// <returns>The de-serialized task</returns>
        private async Task<T> FromJson<T>(Stream s)
        {
            JsonSerializer serializer = new JsonSerializer();

            StreamReader r = new StreamReader(s);
            string contents = await r.ReadToEndAsync();
            return serializer.Deserialize<T>(new JsonTextReader(new StringReader(contents)));
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OwinKeys.cs">
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

namespace Widgt.Owin
{
    /// <summary>
    /// A static class containing constants pertaining to the OWIN environment dictionary
    /// </summary>
    public static class OwinKeys
    {
        /// <summary> A Stream with the request body, if any. Stream.Null MAY be used as a placeholder if there is no request body </summary>
        public const string RequestBody = "owin.RequestBody";

        /// <summary> An IDictionary&lt;string, string[]/&gt; of request headers. </summary>
        public const string RequestHeaders = "owin.RequestHeaders";

        /// <summary> A string containing the HTTP request method of the request (e.g., "GET", "POST"). </summary>
        public const string RequestMethod = "owin.RequestMethod";

        /// <summary> A string containing the request path. The path MUST be relative to the "root" of the application delegate. </summary>
        public const string RequestPath = "owin.RequestPath";

        /// <summary> A string containing the portion of the request path corresponding to the "root" of the application delegate </summary>
        public const string RequestPathBase = "owin.RequestPathBase";

        /// <summary> A string containing the protocol name and version (e.g. "HTTP/1.0" or "HTTP/1.1"). </summary>
        public const string RequestProtocol = "owin.RequestProtocol";

        /// <summary> A string containing the query string component of the HTTP request URI, without the leading "?" 
        /// (e.g., "foo=bar&amp;baz=quux"). The value may be an empty string. </summary>
        public const string RequestQueryString = "owin.RequestQueryString";

        /// <summary> A string containing the URI scheme used for the request (e.g., "http", "https") </summary>
        public const string RequestScheme = "owin.RequestScheme";

        /// <summary> An optional string that uniquely identifies a request. The value is opaque and SHOULD have some level of uniqueness. 
        /// A Host MAY specify this value. If it is not specified, middleware MAY set it. Once set, it SHOULD NOT be modified. </summary>
        public const string RequestId = "owin.RequestId";

        /// <summary> A Stream used to write out the response body, if any </summary>
        public const string ResponseBody = "owin.ResponseBody";
        
        /// <summary> An IDictionary&lt;string, string[]&gt; of response headers. </summary>
        public const string ResponseHeaders = "owin.ResponseHeaders";

        /// <summary> An optional int containing the HTTP response status code as defined in RFC 2616 section 6.1.1. The default is 200. </summary>
        public const string ResponseStatusCode = "owin.ResponseStatusCode";

        /// <summary> An optional string containing the reason phrase associated the given status code. If none is provided then the server SHOULD 
        /// provide a default as described in RFC 2616 section 6.1.1 </summary>
        public const string ResponseReasonPhrase = "owin.ResponseReasonPhrase";

        /// <summary> An optional string containing the protocol name and version (e.g. "HTTP/1.0" or "HTTP/1.1"). If none is provided then the 
        /// "owin.RequestProtocol" key's value is the default. </summary>
        public const string ResponseProtocol = "owin.ResponseProtocol";

        /// <summary> Send File API: The SendFileFunc provided by the server or middleware for the application to utilize.  See Delegate Signature. </summary>
        /// <see cref="http://owin.org/spec/extensions/owin-SendFile-Extension-v0.3.0.htm"/>
        public const string SendFileAsync = "sendfile.SendAsync";
    }
}

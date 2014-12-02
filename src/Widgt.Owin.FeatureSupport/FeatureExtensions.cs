// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureExtensions.cs">
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

namespace Widgt.Features
{
    using global::Owin;

    using Widgt.Core.Exceptions;
    using Widgt.Core.Factory;
    using Widgt.Core.Model;

    /// <summary>
    /// Extension methods for adding FeatureMiddleware to an application pipeline.
    /// </summary>
    public static class FeatureExtensions
    {
        /// <summary>
        /// Adds a Widgt middleware to your web application pipeline to allow cross domain requests.
        /// </summary>
        /// <param name="app"> The IAppBuilder passed to your configuration method </param>
        /// <param name="modelFactory"> The model factory. </param>
        /// <param name="options"> An options class that controls the middleware behavior </param>
        /// <returns> The original app parameter </returns>
        public static IAppBuilder UseWidgtFeatures(this IAppBuilder app, IWidgtModelFactory modelFactory, WidgtOptions options)
        {
            Throwable.ThrowIfNull(app, "app");
            Throwable.ThrowIfNull(modelFactory, "modelFactory");
            Throwable.ThrowIfNull(options, "options");

            return app.Use(typeof(FeatureMiddleware), modelFactory, options);
        }
    }
}

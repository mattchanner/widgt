// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtMapper.cs">
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

namespace Widgt.WebApi.Models
{
    using Widgt.Api.Shared;
    using Widgt.Core.Model;

    /// <summary>
    /// A mapper class to map between backend and frontend types 
    /// </summary>
    public static class WidgtMapper
    {
        /// <summary>
        /// Maps from a <see cref="Widget"/> to a <see cref="WidgtDto"/>
        /// </summary>
        /// <param name="widget">The widget to map</param>
        /// <param name="locale">The current locale</param>
        /// <param name="options">The widgt options</param>
        /// <returns>The API model</returns>
        public static WidgtDto ToApiModel(this Widget widget, LocaleName locale, WidgtOptions options)
        {
            Widget localized = widget.LocalizedTo(locale);
            WidgetModel model = new WidgetModel(localized);
            WidgtDto widgtDto = new WidgtDto();
            
            if (localized.Descriptions.Count > 0) widgtDto.Description = localized.Descriptions[0].Text;
            if (localized.Names.Count > 0) widgtDto.Name = localized.Names[0].Short ?? localized.Names[0].Value;
            if (localized.Contents.Count > 0) widgtDto.StartFilePath = UriPath(options, model, localized.Contents[0].Src);
            if (localized.Icons.Count > 0) widgtDto.IconPath = UriPath(options, model, localized.Icons[0].Src);

            widgtDto.Width = localized.Width;
            widgtDto.Height = localized.Height;
            widgtDto.Id = localized.WidgetId;
            
            return widgtDto;
        }

        /// <summary>
        /// Creates what is assumed to be a valid server URL to a resource inside of a widget directory
        /// </summary>
        /// <param name="options">The widget options, used for determining the service prefix for the application</param>
        /// <param name="model">The widget model, used for determining the URL path to the root widget folder (relative to the
        /// application root)</param>
        /// <param name="src">The path inside of the widget folder to the resource to access</param>
        /// <returns>The created path</returns>
        private static string UriPath(WidgtOptions options, WidgetModel model, string src)
        {
            string prefix = options.ServerPrefix.EndsWith("/") ? options.ServerPrefix : options.ServerPrefix + "/";
            string uriPart = model.UriPart.EndsWith("/") ? model.UriPart : model.UriPart + "/";
            return prefix + uriPart + src;
        }
    }
}

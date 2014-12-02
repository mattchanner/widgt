// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtApiController.cs">
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

namespace Widgt.WebApi.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Web.Http;

    using Widgt.Core.Db;
    using Widgt.Core.Factory;
    using Widgt.Core.Model;
    using Widgt.WebApi.Models;

    /// <summary>
    /// The API controller for the Widgt application
    /// </summary>
    public class WidgtApiController : ApiController
    {
        /// <summary> The default locale to use when it cannot be determined </summary>
        private static readonly LocaleName CurrentLocaleName = new LocaleName(CultureInfo.CurrentCulture.Name);

        /// <summary> The repository implementation </summary>
        private readonly IWidgetRepository repository;

        /// <summary> The model factory </summary>
        private readonly WidgetModelFactory factory;

        private readonly WidgtContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="WidgtApiController"/> class. 
        /// </summary>
        /// <param name="repository"> The widget repository  </param>
        /// <param name="factory"> The model factory to use </param>
        /// <param name="context">The widgt context</param>
        public WidgtApiController(IWidgetRepository repository, WidgetModelFactory factory, WidgtContext context)
        {
            this.repository = repository;
            this.factory = factory;
            this.context = context;
        }

        /// <summary>
        /// Returns all deployed widgets
        /// </summary>
        /// <returns>Each deployed widget</returns>
        public IEnumerable<WidgtApiModel> Get()
        {
            LocaleName locale = GetLocaleName();

            return this.repository.GetWidgets().Select(w => w.ToApiModel(locale, context));
        }

        /// <summary>
        /// Gets a single widget based on its identifier
        /// </summary>
        /// <param name="widgetId"> The widget Id. </param>
        /// <returns> The found widget </returns>
        public WidgtApiModel Get(string widgetId)
        {
            if (string.IsNullOrEmpty(widgetId))
                throw new HttpResponseException(HttpStatusCode.BadRequest);

            Widget widget = this.repository.GetWidget(widgetId);
            if (widget != null) return widget.ToApiModel(GetLocaleName(), context);

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Deletes a widget by its identifier
        /// </summary>
        /// <param name="widgetId">The id of the widget to delete</param>
        public void Delete(string widgetId)
        {
            if (!factory.Undeploy(widgetId))
                throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    
        /// <summary>
        /// Extractsthe first locale name from the request header
        /// </summary>
        /// <returns>The first locale name, assumed to have the highest priority</returns>
        private LocaleName GetLocaleName()
        {
            var acceptLangHeader = this.Request.Headers.AcceptLanguage;

            LocaleName locale = null;
            if (acceptLangHeader != null && acceptLangHeader.Count > 0)
            {
                locale = LocaleName.ParseLanguageHeader(acceptLangHeader.First().Value).FirstOrDefault();
            }

            return locale ?? CurrentLocaleName;
        }
    }
}

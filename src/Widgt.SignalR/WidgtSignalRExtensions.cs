// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtSignalRExtensions.cs">
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

using global::Owin;

namespace Widgt.SignalR
{
    using System;
    using System.Reactive;
    
    using Microsoft.AspNet.SignalR;
    using Widgt.Core.Factory;
    using Widgt.Core.Model;

    /// <summary>
    /// An OWIN App builder extension for adding SignalR support to widgt applications
    /// </summary>
    public static class WidgtSignalRExtensions
    {
        /// <summary>
        /// Adds the SignalR middleware support to the Widgt application
        /// </summary>
        /// <param name="app">The app builder to configure</param>
        /// <param name="modelFactory">The widget model factory</param>
        /// <param name="options">The Signal R options</param>
        /// <returns>The app builder</returns>
        public static IAppBuilder UseWidgtSignalR(this IAppBuilder app, IWidgtModelFactory modelFactory, WidgtSignalROptions options)
        {
            HubConfiguration config = new HubConfiguration { EnableJSONP = true };
        
            IObserver<WidgetModel> deployObserver = Observer.Create<WidgetModel>(
                model => GlobalHost.ConnectionManager
                                   .GetHubContext<WidgtSignalRHub>()
                                   .Clients.All.widgetDeployed(model.Widget.WidgetId));

            IObserver<WidgetModel> undeployObserver = Observer.Create<WidgetModel>(
                model => GlobalHost.ConnectionManager
                                   .GetHubContext<WidgtSignalRHub>()
                                   .Clients.All.widgetUndeployed(model.Widget.WidgetId));

            modelFactory.WidgetDeployed.Subscribe(deployObserver);
            modelFactory.WidgetUndeployed.Subscribe(undeployObserver);

            config.Resolver.Register(typeof(WidgtSignalRHub), () => new WidgtSignalRHub());
            
            app.MapSignalR(config);

            return app;
        }
    }
}

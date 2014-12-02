// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs">
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

namespace Widgt.Owin.Hosting.Self
{
    using System.IO;
    using System.Web.Http;
    using System.Web.Http.Dispatcher;

    using Microsoft.Owin.Cors;
    using Microsoft.Practices.Unity;

    using global::Owin;

    using Unity.WebApi;

    using Widgt.Core.Db;
    using Widgt.Core.Factory;
    using Widgt.Core.Features;
    using Widgt.Core.Model;
    using Widgt.Db;
    using Widgt.Features;
    using Widgt.SignalR;

    /// <summary>
    /// The startup class is used to configure the OWIN application
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This code configures the application.  The startup class is specified as a 
        /// type parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="appBuilder">The app builder to configure</param>
        public void Configuration(IAppBuilder appBuilder)
        {
            // Set up the directory used for deploying widgets
            DirectoryInfo workingDirectory = new DirectoryInfo("deploy");
            if (workingDirectory.Exists == false)
                workingDirectory.Create();

            // The directory where features are found
            DirectoryInfo featureDirectory = new DirectoryInfo("features");
            if (featureDirectory.Exists == false)
                featureDirectory.Create();

            WidgtOptions context = new WidgtOptions
                                       {
                                           ServerPrefix = "/widgt",
                                           FeatureFolder = featureDirectory,
                                           FeaturePrefix = "/features"
                                       };

            // Features are mapped to folder based resources using the folder based feature processor
            IFeatureProcessor featureProcessor = new FolderBasedFeatureProcessor(
                context.FeatureFolder,
                context.FeaturePrefix);

            // Use the default starte file factory (internally uses HtmlAgilityPack for parsing HTML documents)
            IStartFileFactory startFileFactory = new DefaultStartFileFactory();
    
            // Entity framework repository
            IWidgtRepository repository = new WidgtRepository();

            // Create a new instance of the widget factory, using the deployment directory as the root for widgets to
            // be deployed to
            IWidgtModelFactory factory = new WidgtModelFactory(
                workingDirectory, 
                repository,
                featureProcessor,
                startFileFactory);

            this.ConfigureWidgt(appBuilder, factory, context);
            var httpConfig = this.ConfigureWebApi(appBuilder);
            this.ConfigureWidgtApi(httpConfig, repository, factory, context);
            this.ConfigureSignalRForWidgtApp(appBuilder, factory);
            this.ConfigureCors(appBuilder);
        }

        /// <summary>
        /// Sets up cross origin requests for calls made from different clients
        /// </summary>
        /// <param name="appBuilder">The app builder to configure</param>
        private void ConfigureCors(IAppBuilder appBuilder)
        {
            appBuilder.UseCors(CorsOptions.AllowAll);
        }

        /// <summary>
        /// Sets up the app builder to include SignalR in the pipeline
        /// </summary>
        /// <param name="app">The app builder to augment</param>
        /// <param name="factory">The model factory</param>
        private void ConfigureSignalRForWidgtApp(IAppBuilder app, IWidgtModelFactory factory)
        {
            app.UseWidgtSignalR(factory, new WidgtSignalROptions { Path = "/signalr" });
        }

        /// <summary>
        /// Configures the WebAPI framework for use in this container
        /// </summary>
        /// <param name="appBuilder">
        /// The app builder to add WebApi to
        /// </param>
        /// <returns>
        /// The <see cref="HttpConfiguration"/>. </returns>
        private HttpConfiguration ConfigureWebApi(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            appBuilder.UseWebApi(config);

            return config;
        }

        /// <summary>
        /// Configures the dependency resolver for WebApi and registers the API controller instance to use
        /// </summary>
        /// <param name="builder">The builder to use</param>
        /// <param name="repository">The repository</param>
        /// <param name="factory">The factory</param>
        /// <param name="context">The widgt context</param>
        private void ConfigureWidgtApi(HttpConfiguration builder,
            IWidgtRepository repository,
            IWidgtModelFactory factory,
            WidgtOptions context)
        {
            var container = new UnityContainer();

            container.RegisterInstance(typeof(IWidgtModelFactory), factory);
            container.RegisterInstance(typeof(IWidgtRepository), repository);
            container.RegisterInstance(typeof(WidgtOptions), context);

            builder.DependencyResolver = new UnityDependencyResolver(container);
            builder.Services.Replace(typeof(IAssembliesResolver), new WebApiAssembliesResolver());
        }

        /// <summary>
        /// Configures the Widgt framework for use in this container
        /// </summary>
        /// <param name="appBuilder"> The app builder to add Widgt to </param>
        /// <param name="factory"> The factory. </param>
        /// <param name="context">The widgt context</param>
        private void ConfigureWidgt(IAppBuilder appBuilder, IWidgtModelFactory factory, WidgtOptions context)
        {
            // Add Widgt to the app builder
            appBuilder.UseWidgtFolderLocalizr(factory, context);
            appBuilder.UseWidgtFeatures(factory, context);
        }
    }
}

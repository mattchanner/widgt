// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs">
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


using System;
using System.Windows.Forms;

namespace Embedding
{
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using CefSharp;

    using Embedding.Client;

    using log4net.Config;

    using Microsoft.Owin.Hosting;

    /// <summary>
    /// Program entry point
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string BaseAddress = "http://localhost:9000/";

            CefInit();

            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            StartOptions options = new StartOptions();
            options.Urls.Add(BaseAddress);

            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, errors) => true;

            using (WebApp.Start<Startup>(options))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var hubClient = new WidgtHubClient(BaseAddress);
                var form = new MainForm(new WidgtApiClient(BaseAddress), hubClient);

                // ReSharper disable CSharpWarnings::CS4014
                hubClient.Start();
                // ReSharper restore CSharpWarnings::CS4014

                SynchronizationContext.Current.Post(
                    async state => 
                        {
                            await LoadWidgets();
                            await form.LoadWidgets();
                        }, 
                        null);
                
                Application.Run(form);
            }
        }

        private static async Task LoadWidgets()
        {
            HttpClient client = new HttpClient();
            foreach (FileInfo file in new DirectoryInfo("../../../ToDeploy").GetFiles("*.wgt"))
            {
                using (var bodyContent = new MultipartFormDataContent("Widget-------" + Guid.NewGuid()))
                using (Stream source = file.OpenRead())
                {
                    bodyContent.Add(new StreamContent(source), "widget", file.Name);
                    await client.PostAsync("http://localhost:9000/api/widgt", bodyContent);
                }
            }
        }

        private static void CefInit()
        {
            var settings = new CefSettings();
            settings.RemoteDebuggingPort = 8088;
            settings.LogSeverity = LogSeverity.Verbose;

            if (!Cef.Initialize(settings))
            {
                Environment.Exit(0);
            }
        }
    }
}

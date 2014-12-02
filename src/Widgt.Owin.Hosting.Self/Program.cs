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


namespace Widgt.Owin.Hosting.Self
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using log4net.Config;

    using Microsoft.Owin.Hosting;

    /// <summary>
    /// The program entry point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args">The command line arguments provided to the application</param>
        public static void Main()
        {
            const string BaseAddress = "http://localhost:9000/";

            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            StartOptions options = new StartOptions();
            options.Urls.Add(BaseAddress);

            using (WebApp.Start<Startup>(options))
            {
                LoadWidgets().ContinueWith(task => Console.WriteLine("Widgets deployed, press any key to continue"));

                Console.ReadKey();
            }
        }

        private static async Task LoadWidgets()
        {
            HttpClient client = new HttpClient();
            foreach (FileInfo file in new DirectoryInfo("../../ToDeploy").GetFiles("*.wgt"))
            {
                using (var bodyContent = new MultipartFormDataContent("Widget-------" + Guid.NewGuid()))
                using (Stream source = file.OpenRead())
                {
                    bodyContent.Add(new StreamContent(source), "widget", file.Name);
                    await client.PostAsync("http://localhost:9000/api/widgt", bodyContent);
                }
            }
        }
    }
}

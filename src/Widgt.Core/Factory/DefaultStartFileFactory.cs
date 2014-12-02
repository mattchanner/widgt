// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultStartFileFactory.cs">
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

namespace Widgt.Core.Factory
{
    using System;
    using System.IO;

    using HtmlAgilityPack;

    using Widgt.Core.Exceptions;

    /// <summary>
    /// The default start file factory used by the widget factory for editing start files
    /// </summary>
    public class DefaultStartFileFactory : IStartFileFactory
    {
        /// <inheritdoc />
        public IStartFileInjector LoadStartFile(Stream contents)
        {
            Throwable.ThrowIfNull(contents, "contents");

            try
            {
                HtmlDocument document = new HtmlDocument();
                document.Load(contents);

                return new DefaultStartFileInjector(document);
            }
            catch (Exception ex)
            {
                throw new StartFileLoadException("Caught exception loading start file", ex);
            }
        }

        /// <summary> Default start file injector </summary>
        private class DefaultStartFileInjector : IStartFileInjector
        {
            /// <summary> The document to process </summary>
            private readonly HtmlDocument document;

            internal DefaultStartFileInjector(HtmlDocument document)
            {
                this.document = document;
            }

            /// <inheritdoc />
            public void InjectScript(string src)
            {
                HtmlNode head = GetOrCreateHead(this.document);
                HtmlNode scriptNode = document.CreateElement("script");

                scriptNode.SetAttributeValue("type", "text/javascript");
                scriptNode.SetAttributeValue("language", "javascript");
                scriptNode.SetAttributeValue("src", src);

                PrependNode(head, scriptNode);
            }

            /// <inheritdoc />
            public void InjectStyleSheet(string href)
            {
                HtmlNode head = GetOrCreateHead(this.document);
                HtmlNode linkNode = this.document.CreateElement("link");

                linkNode.SetAttributeValue("rel", "stylesheet");
                linkNode.SetAttributeValue("type", "text/css");
                linkNode.SetAttributeValue("href", href);

                PrependNode(head, linkNode);
            }

            /// <inheritdoc />
            public void WriteTo(Stream output)
            {
                document.Save(output);
            }

            private static void PrependNode(HtmlNode parent, HtmlNode child)
            {
                if (parent.HasChildNodes)
                {
                    parent.InsertBefore(child, parent.FirstChild);
                }
                else
                {
                    parent.AppendChild(child);
                }
            }

            private static HtmlNode GetOrCreateHead(HtmlDocument document)
            {
                HtmlNode htmlNode = document.DocumentNode.Element("html");

                HtmlNode head;
                if (htmlNode != null) 
                    head = htmlNode.Element("head");
                else
                    head = document.DocumentNode.Element("head");

                if (head == null)
                {
                    head = document.CreateElement("head");
                    document.DocumentNode.AppendChild(head);
                }

                return head;
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgetFactoryTests.cs">
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

namespace Widgt.Core.Tests.Parser
{
    using System;
    using System.IO;

    using NUnit.Framework;
    
    using Widgt.Core.Exceptions;
    using Widgt.Core.Factory;
    using Widgt.Core.Features;
    using Widgt.Core.Model;
    using Widgt.Db;

    /// <summary>
    /// A set of tests for basic widget oracle functionality
    /// </summary>
    [TestFixture]
    public class WidgetFactoryTests
    {
        /// <summary>
        /// Widget oracle tests
        /// </summary>
        public class TheWidgetOracle
        {
            private WidgtModelFactory widgetOracle;

            private DirectoryInfo directory;

            /// <summary>
            /// Set up method
            /// </summary>
            [SetUp]
            public void TestSetUp()
            {
                directory = new DirectoryInfo(Guid.NewGuid().ToString());
                directory.Create();

                widgetOracle = new WidgtModelFactory(
                    directory, 
                    new TransientWidgtRepository(), 
                    new NullFeatureProcessor(), 
                    new DefaultStartFileFactory());
            }

            /// <summary>
            /// Tear down method
            /// </summary>
            [TearDown]
            public void TestTearDown()
            {
                try
                {
                    directory.Delete(true);
                }
                catch (IOException)
                {
                }
            }

            /// <summary>
            /// Throws an argument null exception when the input stream is null
            /// </summary>
            [Test, ExpectedException(typeof(ArgumentNullException))]
            public void It_throws_an_exception_with_a_null_stream()
            {
                widgetOracle.Deploy(null);
            }

            /// <summary>
            /// Throws a WidgetArchiveException when the input stream does not represent a valid zip file
            /// </summary>
            [Test, ExpectedException(typeof(WidgetArchiveException))]
            public void It_throws_an_exception_when_input_stream_is_not_a_zip_archive()
            {
                MemoryStream memoryStream = new MemoryStream(new byte[] { 0, 0, 0, 0, 0 });
                widgetOracle.Deploy(memoryStream);
            }

            /// <summary>
            /// Thrown when the zip does not contain a config file
            /// </summary>
            [Test, ExpectedException(typeof(InvalidWidgetArchiveException))]
            public void It_throws_an_exception_when_zip_does_not_contain_a_manifest()
            {
                widgetOracle.Deploy(this.OpenWidgetFile("noconfigfile"));
            }

            /// <summary>
            /// Thrown when the zip does not contain a config file
            /// </summary>
            [Test, ExpectedException(typeof(InvalidWidgetArchiveException))]
            public void It_throws_an_exception_when_zip_does_contain_a_manifest_in_a_sub_dir()
            {
                widgetOracle.Deploy(this.OpenWidgetFile("ConfigInWrongDir"));
            }

            /// <summary>
            /// Tests that a valid wgt file is opened and parsed correctly
            /// </summary>
            [Test]
            public void It_returns_a_valid_widget_for_a_valid_wgt_file()
            {
                WidgetModel model = widgetOracle.Deploy(this.OpenWidgetFile("helloworld"));
                Assert.That(model.Widget.Contents.Count, Is.EqualTo(1));
            }

            /// <summary>
            /// Sub directories are unpacked
            /// </summary>
            [Test]
            public void It_unpacks_sub_directories()
            {
                WidgetModel model = widgetOracle.Deploy(this.OpenWidgetFile("subdirs"));

                FileInfo expectedFile =
                    new FileInfo(Path.Combine(Path.Combine(model.RootDirectory.FullName, "scripts"), "script.js"));

                Assert.That(expectedFile.Exists, Is.True);

                model.RootDirectory.Delete(true);
            }

            /// <summary>
            /// Tests that a start file can be inferred from the zip file contents if not explicitly set by the Contents section in the manifest
            /// </summary>
            [Test]
            public void StartFile_Is_inferred_when_not_explicitly_set()
            {
                WidgetModel model = widgetOracle.Deploy(this.OpenWidgetFile("todo"));
                Assert.That(model.Widget.Contents.Count, Is.EqualTo(1));
                Assert.That(model.Widget.Contents[0].Src, Is.EqualTo("index.html"));
            }

            private Stream OpenWidgetFile(string fileName)
            {
                return File.OpenRead("Widgets\\" + fileName + ".wgt");
            }
        }
    }
}

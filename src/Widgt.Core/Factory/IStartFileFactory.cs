// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStartFileFactory.cs">
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
    using System.IO;

    /// <summary>
    /// A factory used for creating a start file injector from a document
    /// </summary>
    public interface IStartFileFactory
    {
        /// <summary>
        /// Called to load a start file from 
        /// </summary>
        /// <param name="contents">The start file to parse</param>
        /// <returns>
        /// The <see cref="IStartFileInjector"/> reference. </returns>
        IStartFileInjector LoadStartFile(Stream contents);
    }

    /// <summary>
    /// Represents the contract for a start file injector, capable of adding resource
    /// references to the main starting file
    /// </summary>
    public interface IStartFileInjector
    {
        /// <summary>
        /// Called to inject a script into the start file
        /// </summary>
        /// <param name="src">The path of the script to inject</param>
        void InjectScript(string src);

        /// <summary>
        /// Called to inject a style sheet reference into the start file
        /// </summary>
        /// <param name="href">The path to the style sheet to inject</param>
        void InjectStyleSheet(string href);

        /// <summary>
        /// Writes the modified start file back out to the given output stream
        /// </summary>
        /// <param name="output">The output stream to write to</param>
        void WriteTo(Stream output);
    }
}

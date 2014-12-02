// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PythonScriptCache.cs">
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
    using System.Collections.Generic;

    using IronPython;
    using IronPython.Hosting;

    using Microsoft.Scripting.Hosting;

    /// <summary>
    /// A python specific scripting cache
    /// </summary>
    internal class PythonScriptCache : ScriptCache
    {
        /// <summary>
        /// The core implementation used to create the language specific engine
        /// </summary>
        /// <returns>The created engine</returns>
        protected override ScriptEngine CreateEngine()
        {
            ScriptEngine engine = Python.CreateEngine(CreateEngineOptions());
            engine.Runtime.LoadAssembly(typeof(System.Net.NetworkCredential).Assembly);
            return engine;
        }

        /// <summary>
        /// Creates the python options for the new engine
        /// </summary>
        /// <returns>The map of engine options</returns>
        private IDictionary<string, object> CreateEngineOptions()
        {
            var options = new Dictionary<string, object>();
            options["DivisionOptions"] = PythonDivisionOptions.New;
            options["Debug"] = true;
            return options;
        }
    }
}

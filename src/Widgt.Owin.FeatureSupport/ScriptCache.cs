// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptCache.cs">
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
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Scripting.Hosting;

    using Widgt.Core.Utils;

    /// <summary>
    /// A cache of pre-compiled scripts with support for reloading from disk when the file content changes
    /// </summary>
    internal abstract class ScriptCache
    {
        /// <summary> The cache of compiled scripts </summary>
        private readonly IDictionary<string, Tuple<string, CompiledCode>> compiledCodeCache;

        /// <summary> The script engine to use </summary>
        private Lazy<ScriptEngine> engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptCache"/> class.  
        /// </summary>
        protected ScriptCache()
        {
            compiledCodeCache = new Dictionary<string, Tuple<string, CompiledCode>>();
            engine = new Lazy<ScriptEngine>(CreateEngine);
        }

        /// <summary>
        /// Gets the current engine instance
        /// </summary>
        public ScriptEngine Engine
        {
            get
            {
                return engine.Value;
            }
        }

        /// <summary>
        /// The get or create.
        /// </summary>
        /// <param name="filePath"> The file path. </param>
        /// <returns> The <see cref="CompiledCode"/>. </returns>
        public CompiledCode GetOrCreate(FileInfo filePath)
        {
            if (!filePath.Exists)
                throw new FileNotFoundException("The requested file does not exist", filePath.FullName);

            CompiledCode code = null;

            string checkSum = FileSystem.Checksum(filePath);

            lock (compiledCodeCache)
            {
                Tuple<string, CompiledCode> cache;
                if (compiledCodeCache.TryGetValue(filePath.FullName, out cache))
                {
                    if (cache.Item1 == checkSum)
                    {
                        code = cache.Item2;
                    }
                }

                if (code == null)
                {
                    var scriptSource = Engine.CreateScriptSourceFromFile(filePath.FullName);
                    code = scriptSource.Compile();
                    compiledCodeCache[filePath.FullName] = Tuple.Create(checkSum, code);
                }
            }

            return code;
        }

        /// <summary>
        /// WHen overridden in a sub class, this method should return the language specific script engine
        /// </summary>
        /// <returns>The new script engine</returns>
        protected abstract ScriptEngine CreateEngine();
    }
}

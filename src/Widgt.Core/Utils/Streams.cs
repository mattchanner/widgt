// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Streams.cs">
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

namespace Widgt.Core.Utils
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// A utility class containing operations relating to streams
    /// </summary>
    public static class Streams
    {
        /// <summary>
        /// Copies data from the input stream to the output stream
        /// </summary>
        /// <param name="input">The input stream to read from</param>
        /// <param name="output">The output stream to write to</param>
        /// <returns>A task which when complete will copy all data from the input 
        /// stream to the output stream</returns>
        public static async Task Copy(Stream input, Stream output)
        {
            while (true)
            {
                if (await CopyChunk(input, output) == false) break;
            }
        }

        /// <summary>
        /// Copies a single chunk of data from the input stream to the output stream
        /// </summary>
        /// <param name="input">The input stream to read from</param>
        /// <param name="output">The output stream to write to</param>
        /// <returns>A task which when complete will copy all data from the input 
        /// stream to the output stream, the result is an indicator of whether there is any more
        /// data left to read</returns>
        private static async Task<bool> CopyChunk(Stream input, Stream output)
        {
            byte[] buffer = new byte[1024 * 1024];
            int bytesRead = await input.ReadAsync(buffer, 0, buffer.Length);
            
            if (bytesRead > 0)
            {
                await output.WriteAsync(buffer, 0, bytesRead);
            }

            return bytesRead > 0;
        }
    }
}

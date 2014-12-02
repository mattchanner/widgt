// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Throwable.cs">
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

namespace Widgt.Core.Exceptions
{
    using System;

    /// <summary>
    /// Exception factory
    /// </summary>
    public static class Throwable
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> with the given message when the given
        /// predicate returns false
        /// </summary>
        /// <param name="predicate">The predicate to test</param>
        /// <param name="message">The message to use in the exception</param>
        /// <param name="argumentName">The name of the argument</param>
        public static void ThrowIf(Func<bool> predicate, string message, string argumentName)
        {
            if (predicate() == false)
                throw new ArgumentException(message, argumentName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the first argument is null
        /// </summary>
        /// <typeparam name="T">The argument type</typeparam>
        /// <param name="arg">The argument to test</param>
        /// <param name="argumentName">The argument name</param>
        public static void ThrowIfNull<T>(T arg, string argumentName) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> when the given string is null, and
        /// a <see cref="ArgumentException"/> when the string is empty
        /// </summary>
        /// <param name="argumentValue">The string value to test</param>
        /// <param name="argumentName">The name of the argument</param>
        public static void ThrowIfNullOrEmpty(string argumentValue, string argumentName)
        {
            ThrowIfNull(argumentName, argumentValue);
            if (argumentValue.Length == 0)
                throw new ArgumentException("Value cannot be empty", argumentName);
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWidgtRepository.cs">
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
namespace Widgt.Core.Db
{
    using System.Collections.Generic;

    using Widgt.Core.Model;

    /// <summary>
    /// The repository interface used for components able to load and save widgets from a backend data store
    /// </summary>
    public interface IWidgtRepository
    {
        /// <summary>
        /// Gets all the widgets in the system
        /// </summary>
        /// <returns>All deployed widgets</returns>
        IEnumerable<Widget> GetWidgets();

        /// <summary>
        /// Gets a widget by its identifier
        /// </summary>
        /// <param name="widgetId">The Id of the widget</param>
        /// <returns>The matching widget or null if not found</returns>
        Widget GetWidget(string widgetId);

        /// <summary>
        /// Adds a new widget to the repository
        /// </summary>
        /// <param name="widget">The widget to add</param>
        void AddWidget(Widget widget);

        /// <summary>
        /// Updates a widget in the repository
        /// </summary>
        /// <param name="widget">The widget to update</param>
        void UpdateWidget(Widget widget);

        /// <summary>
        /// Removes a widget from the repository
        /// </summary>
        /// <param name="widget">The widget to delete</param>
        void DeleteWidget(Widget widget);
    }
}

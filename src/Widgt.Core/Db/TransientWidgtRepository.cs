// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransientWidgtRepository.cs">
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

namespace Widgt.Db
{
    using System.Collections.Generic;
    using System.Linq;

    using Widgt.Core.Db;
    using Widgt.Core.Model;

    /// <summary>
    /// A transient, in-memory widget repository.  This version is intended for use in test scenarios
    /// and is not thread safe.
    /// </summary>
    public class TransientWidgtRepository : IWidgtRepository
    {
        /// <summary> The backing store </summary>
        private IList<Widget> store = new List<Widget>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientWidgtRepository"/> class.
        /// </summary>
        /// <param name="existingWidgets"> Existing widgets to populate the repository with. </param>
        public TransientWidgtRepository(IEnumerable<Widget> existingWidgets = null)
        {
            if (existingWidgets != null)
            {
                foreach (Widget widget in existingWidgets)
                    store.Add(widget);
            }
        }

        /// <inheritdoc />
        public IEnumerable<Widget> GetWidgets()
        {
            return this.store.AsQueryable();
        }

        /// <inheritdoc />
        public Widget GetWidget(string widgetId)
        {
            return this.store.FirstOrDefault(w => w.WidgetId == widgetId);
        }

        /// <inheritdoc />
        public void AddWidget(Widget widget)
        {
            this.UpdateWidget(widget);
        }

        /// <inheritdoc />
        public void UpdateWidget(Widget widget)
        {
            this.DeleteWidget(widget);
            this.store.Add(widget);
        }

        /// <inheritdoc />
        public void DeleteWidget(Widget widget)
        {
            this.store.Remove(widget);
        }
    }
}

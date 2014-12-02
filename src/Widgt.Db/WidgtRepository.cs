// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtRepository.cs">
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
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using log4net;

    using Widgt.Core.Db;
    using Widgt.Core.Exceptions;
    using Widgt.Core.Model;

    /// <summary>
    /// The widgt repository using entity framework to manage the database
    /// </summary>
    public class WidgtRepository : IWidgtRepository
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WidgtRepository));

        /// <inheritdoc />
        public IEnumerable<Widget> GetWidgets()
        {
            var db = new WidgtContext();

            return db.Widgets
                     .Include("Contents")
                     .Include("Author")
                     .Include("Icons")
                     .Include("Descriptions")
                     .Include("Features")
                     .Include("Features.Parameters");
        }

        /// <inheritdoc />
        public Widget GetWidget(string widgetId)
        {
            Throwable.ThrowIfNullOrEmpty(widgetId, "widgetId");

            using (var db = new WidgtContext())
            {
                return db.Widgets.FirstOrDefault(w => w.WidgetId == widgetId);
            }
        }

        /// <inheritdoc />
        public void AddWidget(Widget widget)
        {
            Throwable.ThrowIfNull(widget, "widget");

            try
            {
                using (var db = new WidgtContext())
                {
                    db.Widgets.AddOrUpdate(widget);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error adding new widget to the database", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public void UpdateWidget(Widget widget)
        {
            Throwable.ThrowIfNull(widget, "widget");

            try
            {
                using (var db = new WidgtContext())
                {
                    db.Widgets.AddOrUpdate(widget);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating widget in the database", ex);
                throw;
            }
        }

        /// <inheritdoc />
        public void DeleteWidget(Widget widget)
        {
            Throwable.ThrowIfNull(widget, "widget");

            try
            {
                using (var db = new WidgtContext())
                {
                    // Seems to be the only way to prevent EF from throwing an exception when it has not already loaded the item to 
                    // be deleted (in fact, it throws it even when this is the case)
                    db.Configuration.ValidateOnSaveEnabled = false;
                    
                    if (this.GetWidget(widget.WidgetId) != null)
                    {
                        db.Widgets.Attach(widget);
                        db.Entry(widget).State = EntityState.Deleted;
                        db.SaveChanges();
                    }

                    db.Configuration.ValidateOnSaveEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating widget in the database", ex);
                throw;
            }
        }
    }
}

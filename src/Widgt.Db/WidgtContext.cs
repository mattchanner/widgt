// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WidgtContext.cs" company="ID Business Solutions Ltd.">
//   Copyright © 1994 - 2014
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Widgt.Db
{
    using System.Data.Entity;

    using Widgt.Core.Model;

    /// <summary>
    /// The Entity Framework database context for the Widgt schema
    /// </summary>
    public class WidgtContext : DbContext
    {
        #region Public Properties

        /// <summary> Gets or sets the access requests. </summary>
        public DbSet<AccessRequest> AccessRequests { get; set; }

        /// <summary> Gets or sets the authors. </summary>
        public DbSet<Author> Authors { get; set; }

        /// <summary> Gets or sets the contents. </summary>
        public DbSet<Content> Contents { get; set; }

        /// <summary> Gets or sets the descriptions. </summary>
        public DbSet<Description> Descriptions { get; set; }

        /// <summary> Gets or sets the feature parameters. </summary>
        public DbSet<FeatureParameter> FeatureParameters { get; set; }

        /// <summary> Gets or sets the features. </summary>
        public DbSet<FeatureRequest> Features { get; set; }

        /// <summary> Gets or sets the icons. </summary>
        public DbSet<Icon> Icons { get; set; }

        /// <summary> Gets or sets the licenses. </summary>
        public DbSet<License> Licenses { get; set; }

        /// <summary> Gets or sets the names. </summary>
        public DbSet<Name> Names { get; set; }

        /// <summary> Gets or sets the preferences. </summary>
        public DbSet<Preference> Preferences { get; set; }

        /// <summary> Gets or sets the widgets. </summary>
        public DbSet<Widget> Widgets { get; set; }

        #endregion

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder b)
        {
            b.Entity<Author>().HasRequired(a => a.Widget).WithOptional(w => w.Author).WillCascadeOnDelete(true);
            b.Entity<AccessRequest>().HasRequired(m => m.Widget).WithMany(widget => widget.AccessRequests).WillCascadeOnDelete(true);
            b.Entity<Content>().HasRequired(c => c.Widget).WithMany(w => w.Contents).WillCascadeOnDelete(true);
            b.Entity<Description>().HasRequired(m => m.Widget).WithMany(widget => widget.Descriptions).WillCascadeOnDelete(true);
            b.Entity<FeatureRequest>().HasRequired(m => m.Widget).WithMany(widget => widget.Features).WillCascadeOnDelete(true);
            b.Entity<FeatureParameter>().HasRequired(m => m.Feature).WithMany(f => f.Parameters).WillCascadeOnDelete(true);
            b.Entity<Icon>().HasRequired(m => m.Widget).WithMany(widget => widget.Icons).WillCascadeOnDelete(true);
            b.Entity<License>().HasRequired(m => m.Widget).WithMany(widget => widget.Licenses).WillCascadeOnDelete(true);
            b.Entity<Name>().HasRequired(m => m.Widget).WithMany(widget => widget.Names).WillCascadeOnDelete(true);
            b.Entity<Preference>().HasRequired(m => m.Widget).WithMany(widget => widget.Preferences).WillCascadeOnDelete(true);
        }
    }
}
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Notes_MarketPlace.Context
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class Entities3 : DbContext
    {
        public Entities3()
            : base("name=Entities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Download> Downloads { get; set; }
        public virtual DbSet<NoteCategory> NoteCategories { get; set; }
        public virtual DbSet<NoteType> NoteTypes { get; set; }
        public virtual DbSet<ReferenceData> ReferenceDatas { get; set; }
        public virtual DbSet<SellerNote> SellerNotes { get; set; }
        public virtual DbSet<SellerNotesAttachement> SellerNotesAttachements { get; set; }
        public virtual DbSet<SellerNotesReportedIssue> SellerNotesReportedIssues { get; set; }
        public virtual DbSet<SellerNotesReview> SellerNotesReviews { get; set; }
        public virtual DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<NewGetSellerNotesDetails_Result> NewGetSellerNotesDetails(Nullable<int> fK_Type, Nullable<int> fK_Category, Nullable<int> fK_Country, string fK_University, string fK_Course, Nullable<decimal> fK_Rating, Nullable<int> pageSize, Nullable<int> pageNumber, string search)
        {
            var fK_TypeParameter = fK_Type.HasValue ?
                new ObjectParameter("FK_Type", fK_Type) :
                new ObjectParameter("FK_Type", typeof(int));
    
            var fK_CategoryParameter = fK_Category.HasValue ?
                new ObjectParameter("FK_Category", fK_Category) :
                new ObjectParameter("FK_Category", typeof(int));
    
            var fK_CountryParameter = fK_Country.HasValue ?
                new ObjectParameter("FK_Country", fK_Country) :
                new ObjectParameter("FK_Country", typeof(int));
    
            var fK_UniversityParameter = fK_University != null ?
                new ObjectParameter("FK_University", fK_University) :
                new ObjectParameter("FK_University", typeof(string));
    
            var fK_CourseParameter = fK_Course != null ?
                new ObjectParameter("FK_Course", fK_Course) :
                new ObjectParameter("FK_Course", typeof(string));
    
            var fK_RatingParameter = fK_Rating.HasValue ?
                new ObjectParameter("FK_Rating", fK_Rating) :
                new ObjectParameter("FK_Rating", typeof(decimal));
    
            var pageSizeParameter = pageSize.HasValue ?
                new ObjectParameter("PageSize", pageSize) :
                new ObjectParameter("PageSize", typeof(int));
    
            var pageNumberParameter = pageNumber.HasValue ?
                new ObjectParameter("PageNumber", pageNumber) :
                new ObjectParameter("PageNumber", typeof(int));
    
            var searchParameter = search != null ?
                new ObjectParameter("Search", search) :
                new ObjectParameter("Search", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NewGetSellerNotesDetails_Result>("NewGetSellerNotesDetails", fK_TypeParameter, fK_CategoryParameter, fK_CountryParameter, fK_UniversityParameter, fK_CourseParameter, fK_RatingParameter, pageSizeParameter, pageNumberParameter, searchParameter);
        }
    }
}

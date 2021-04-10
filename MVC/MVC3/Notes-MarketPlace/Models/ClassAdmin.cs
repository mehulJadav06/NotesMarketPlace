using Notes_MarketPlace.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
    public class Manager
    {
        public User user { get; set; }

        public UserProfile userProfile { get; set; }
    }
    public class AddAdmin
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string EmailID { get; set; }
        
        public string Password { get; set; }
        [Required]
        public string CountryCode { get; set; }

        public IEnumerable<Country> Countries { get; set; }
        [Required]
        public string MobileNo { get; set; }


    }

    public class MyProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public string EmailID2 { get; set; }
        public string CountryCode { get; set; }
        public string MobileNo { get; set; }
        public string ProfilePicture { get; set; }
        public IEnumerable<Country> Countries { get; set; }
    }
    public class Members
    {
        public List<User> users { get; set; }
    }
    public class Member
    {
        public User user { get; set; }
        public UserProfile userProfile { get; set; }

        public IEnumerable<MemberDetails> member { get; set; }
    }
    public class MemberDetails
    {
        public SellerNote note { get; set; }

        public NoteCategory category { get; set; }

    }
    public class NotesUnderReview
    {
        public SellerNote note { get; set; }

        public NoteCategory category { get; set; }

        public User user { get; set; }
        public User sellers { get; set; }
    }
    public class PublishedNotes
    {
        public SellerNote note { get; set; }
        public User seller { get; set; }
        public User approvedBy { get; set; }
        public NoteCategory category { get; set; }

        
    }

    public class RejectedNotesAdmin
    {
        public SellerNote note { get; set; }
        public User seller { get; set; }
        public User RejectedBy { get; set; }
        public NoteCategory category { get; set; }
        public SellerNotesReportedIssue reports { get; set; }
    }
    public class Downloads
    {
        public Download download { get; set; }
        public User seller { get; set; }
        public User Buyer { get; set; }
    }
    public class SpamReports
    {
        public SellerNote note { get; set; }
        public User ReportedBy { get; set; }
        public NoteCategory category { get; set; }
        public SellerNotesReportedIssue reports { get; set; }
    }
    public class ManageCategory
    {
        public NoteCategory category { get; set; }
        public User admin { get; set; }
    }
    public class ManageType
    {
        public NoteType type { get; set; }
        public User admin { get; set; }
    }
    public class ManageCountry
    {
        public Country country { get; set; }
        public User admin { get; set; }
    }

    public class AddType
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
    public class AddCategory
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
    public class AddCountry
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string CountryCode { get; set; }
    }
}
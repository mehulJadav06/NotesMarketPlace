using Notes_MarketPlace.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Notes_MarketPlace.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        public string EmailID { get; set; }


        [Required(ErrorMessage = "Required")]
        [RegularExpression("^(?=.{6,24})(?=.*[a-z])(?=.*[@#$%^&+=]).*$", ErrorMessage = "Invalid Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
    public class ChangePassword
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^(?=.{6,24})(?=.*[a-z])(?=.*[@#$%^&+=]).*$", ErrorMessage = "Invalid Password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression("^(?=.{6,24})(?=.*[a-z])(?=.*[@#$%^&+=]).*$", ErrorMessage = "Invalid Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Enter same Password")]
        public string ConformPassword { get; set; }
    } 
    public class AddNotes
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public int CategoryID { get; set; }
        public IEnumerable<NoteCategory> categories { get; set; }
        public string DisplayPicture { get; set; }

        [Required]
        public string UploadNotes { get; set; }

        public int NoteTypeID { get; set; }
        public IEnumerable<NoteType> types { get; set; }
        public Nullable<int> NumberOfPages { get; set; }
        [Required]
        public string Description { get; set; }
        public string UniversityName { get; set; }
        public int CountryID { get; set; }
        public IEnumerable<Country> countries { get; set; }
        public string Course { get; set; }
        public string CourseCode { get; set; }
        public string Professor { get; set; }
        [Required]
        public bool IsPaid { get; set; }
        [Required]
        public Nullable<decimal> SellingPrice { get; set; }
        public string NotesPreview { get; set; }
    }
    public class ContactUs
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression("[a-zA-Z_]+", ErrorMessage = "Enter Alphabatic Charcters Only")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Subject { get; set; }


        [Required(ErrorMessage = "Required")]
        public string Question { get; set; }


        [Required(ErrorMessage = "Required")]
        [EmailAddress]
        public string EmailID { get; set; }
    }


    public class NotesDetail
    {
        public int ID { get; set; }
        public int sellerID { get; set; }
        public SellerNote sellerNote { get; set; }

        public string category { get; set; }
        public string country { get; set; }

        public decimal count { get; set; }

        public string userName { get; set; }
        public string sellerName { get; set; }
        public IEnumerable<reviews> review { get; set; }
        public decimal rate { get; set; }
        public int userReport { get; set; }
    }
    public class reviews
    {
        public SellerNotesReview review { get; set; }

        public User users { get; set; }

        public UserProfile userProfiles { get; set; }
    }
    public class UserDetail
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool isAdmin { get; set; }

        [EmailAddress]
        //[Remote("IsEmailExist", "User", ErrorMessage = "Email Address Already Exist")]
        public string EmailID { get; set; }


        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public int Gender { get; set; }
        public string CountryCode { get; set; }

        public string MobileNo { get; set; }

        public string ProfilePicture { get; set; }
        public string AddressLine1 { get; set; }
        public string AddreessLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string University { get; set; }
        public string College { get; set; }

        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<ReferenceData> GenderList { get; set; }


    }

    public class BuyerRequest{

        public User user { get; set; }
        public Download download { get; set; }

        public UserProfile userProfile { get; set; }

    }
    public class MyDownloads
    {
        public User users { get; set; }
        public Download downloads { get; set; }
    }
    public class RejectedNotes
    {
        public SellerNote sellerNote { get; set; }
        public NoteCategory category { get; set; }
    }
    public class Dashboard
    {
        public int numberOfNotes { get; set; }
        public decimal earnMoney { get; set; }

        public int myDownloads { get; set; }
        public int myRejectedNotes { get; set; }
        public int buyerRequest { get; set; }
        public IEnumerable<ProgressNotes> progressNotes { get; set; }
        public IEnumerable<ProgressNotes> PublishedNotes { get; set; }
    }

    public class ProgressNotes
    {
        public SellerNote sellerNote { get; set; }
        public NoteCategory category { get; set; }

        public ReferenceData referenceData { get; set; }
    }
}
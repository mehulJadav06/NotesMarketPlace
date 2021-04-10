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
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheLibrary.Models
{
    public class LibraryItem
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Category Id")]
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Required]
        public string Title { get; set; }
        [DisplayName("Author, Speaker or Director")]
        [Required(ErrorMessage = "Creator of the media is required")]
        public string Author { get; set; }
        public int? Pages { get; set; }
        [DisplayName("DVD or Audiobook Length in Minutes")]
        public int? RunTimeMinutes { get; set; }
        [DisplayName("Borrowable")]
        public bool IsBorrowable { get; set; }
        [DisplayName("Borrowed By")]
        public string Borrower { get; set; }
        [DisplayName("Date of Borrow")]
        public DateTime? Date { get; set; }
        [DisplayName("Mediatype")]
        public string Type { get; set; }
    }
}

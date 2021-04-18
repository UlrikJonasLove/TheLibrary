using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TheLibrary.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Category | ")]
        [Required(ErrorMessage = "Category Name is Required")]
        public string CategoryName { get; set; }
        [InverseProperty(nameof(LibraryItem.Category))]
        public virtual ICollection<LibraryItem> LibraryItems { get; set; } //This line of code is needed to give multiply Library Items the same Category ID 
    }
}

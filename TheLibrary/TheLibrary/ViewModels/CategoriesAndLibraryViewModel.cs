using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheLibrary.Models;

namespace TheLibrary.ViewModels
{
    public class CategoriesAndLibraryViewModel
    {
        //A viewmodel so I am able to display the categories and filter the types
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<LibraryItem> LibraryItems { get; set; }
    }
}

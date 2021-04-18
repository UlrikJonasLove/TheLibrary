using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TheLibrary.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        [Range(1, 10)]
        [Required]
        public decimal Salary { get; set; }
        [DisplayName("CEO")]
        public bool IsCEO { get; set; }
        [DisplayName("Manager")]
        public bool IsManager { get; set; }
        [DisplayName("Manager Id")]
        public int? ManagerId { get; set; }
    }
}

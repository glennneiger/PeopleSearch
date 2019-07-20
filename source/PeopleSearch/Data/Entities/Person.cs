using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeopleSearch.Data.Entities
{
    [Table("People")]
    public class Person
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(25)]
        public string PhoneNumber { get; set; }
    }
}

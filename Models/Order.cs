using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lista10.Models
{
	public class Order
	{
		public int Id { get; set; }
		[Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string HouseNumber { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Payment { get; set; }
        public List<(Article, int)> CartItems { get; set; } = new List<(Article, int)>();
        public double Price { get; set; }

    }
}

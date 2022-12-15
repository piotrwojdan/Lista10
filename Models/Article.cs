using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Lista10.Models
{
    public class Article
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Name too short")]
        public string Name { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        [NotMapped]
        public IFormFile PictureFile { get; set; }

        public string Picture { get; set; }

        [Required]
        public int Category { get; set; }

        public Article()
        {
        }

        public Article(int id, string name, double price, IFormFile picture, int category)
        {
            Id = id;
            Name = name;
            Price = price;
            PictureFile = picture;
            Category = category;
        }
    }
}
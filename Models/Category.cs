using System.ComponentModel.DataAnnotations;

namespace Lista10.Models
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Name too short")]
        public string Name { get; set; }

        public Category() {}

        public Category(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

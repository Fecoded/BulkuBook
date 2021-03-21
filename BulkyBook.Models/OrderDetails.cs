using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }

        public Product Product { get; set; }

    }
}

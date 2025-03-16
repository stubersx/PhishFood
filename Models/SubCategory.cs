﻿using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Subcategory
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "The Subcategory Name is required.")]
        [Display(Name = "Subcategory Name")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Category is required.")]
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }

        public ICollection<Testing> Testings { get; set; } = new List<Testing>();
        public ICollection<Training> Trainings { get; set; } = new List<Training>();
    }
}

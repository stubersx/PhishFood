﻿using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "The Category Name is required.")]
        [Display(Name = "Category Name")]
        public string Type { get; set; } = string.Empty;

        public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
        public ICollection<Testing> Testings { get; set; } = new List<Testing>();
        public ICollection<Training> Trainings { get; set;} = new List<Training>();
        public ICollection<Result> Results { get; set; } = new List<Result>();
    }
}

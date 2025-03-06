using System.ComponentModel.DataAnnotations;

namespace PhishFood.Models
{
    public class Result
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }  //automatically recorded current time?
        [Required]
        [Range(1, 10)]
        public int Score { get; set; }  //automatically recorded the highest score?

        public int CategoryID { get; set; }
        public Category? Category { get; set; }

        public string StudentID { get; set; }
        public Student? Student { get; set; }
    }
}

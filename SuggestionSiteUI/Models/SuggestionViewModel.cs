using System.ComponentModel.DataAnnotations;

namespace SuggestionSiteUI.Models
{
    public class SuggestionViewModel
    {
        [Required]
        [MaxLength(75)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MinLength(1)]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }
    }
}

using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace GamesAPITestTask.Models.DTOs
{
    public class GameDTO
    {
        [Required]
        public string Name { get; set; }
        public int[] GenresIds { get; set; }
        public string[] GenresNames { get; set; }
        public int DeveloperId { get; set; }
        public string DeveloperName { get; set; }

    }
}

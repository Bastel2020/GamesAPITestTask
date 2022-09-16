using GamesAPITestTask.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamesAPITestTask.Models
{
    public class DefaultGame : IGameBase
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public IDeveloperBase Developer { get; set; }
        public List<IGenreBase> Genres { get; set; }

        public DefaultGame() { }

        public DefaultGame(string name, IDeveloperBase developer, List<IGenreBase> genres)
        {
            Name = name;
            Developer = developer;
            Genres = genres;
        }

        public object ConvertToJsonCompatible()
        {
            return new
            {
                Id = Id,
                Name = Name,
                DeveloperId = Developer.Id,
                Developer = Developer.Name,
                Genres = Genres.Select(gen => new {Id = gen.Id, Name = gen.Name})
            };
        }
    }
}

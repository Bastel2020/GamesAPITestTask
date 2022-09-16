using GamesAPITestTask.Interfaces.Models;
using System.Collections.Generic;
using System.Linq;

namespace GamesAPITestTask.Models
{
    public class DefaultDeveloper : IDeveloperBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<IGameBase> Games { get; set; }
        public object ConvertToJsonCompatible()
        {
            return new
            {
                Id = Id,
                Name = Name,
                Genres = Games.Select(g => new { Id = g.Id, Name = g.Name })
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesAPITestTask.Interfaces.Models
{
    public interface IGenreBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<IGameBase> Games { get; set; }
        public object ConvertToJsonCompatible();
    }
}

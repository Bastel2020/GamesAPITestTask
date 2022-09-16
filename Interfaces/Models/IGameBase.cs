using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesAPITestTask.Interfaces.Models
{
    public interface IGameBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IDeveloperBase Developer { get; set; }
        public List<IGenreBase> Genres { get; set; }
        public object ConvertToJsonCompatible();
    }
}

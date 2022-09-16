using GamesAPITestTask.Interfaces.Models;
using GamesAPITestTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesAPITestTask.Database
{
    public class DefaultDBContext<T, T2, T3> : DbContext
        where T : class, IGameBase
        where T2 : class, IDeveloperBase
        where T3 : class, IGenreBase
    {
        private DbContextOptions _options;

        private DbSet<T> Games { get; set; }
        private DbSet<T2> Developers { get; set; }
        private DbSet<T3> Genres { get; set; }

        public DefaultDBContext(DbContextOptions options)
        {
            _options = options;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<T>()
                .HasMany(gam => (IEnumerable<T3>)gam.Genres)
                .WithMany(gen => (IEnumerable<T>)gen.Games);

            modelBuilder.Entity<T>()
                .HasOne(g => (T2)g.Developer)
                .WithMany(d => (IEnumerable<T>)d.Games);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Games.DefaultDb;Trusted_Connection=True;");
        }

        public IEnumerable<IGameBase> GetGames()
        {
            return Games.Include(g => g.Genres)
                .Include(g => g.Developer);
        }

        public async Task AddGameAsync(IGameBase newGame)
        {
            await Games.AddAsync((T)newGame);
        }

        public async Task AddDeveloperAsync(IDeveloperBase newDeveloper)
        {
            await Developers.AddAsync((T2)newDeveloper);
        }

        public async Task AddGenreAsync(IGenreBase newGenre)
        {
            await Genres.AddAsync((T3)newGenre);
        }

        public IEnumerable<IGenreBase> GetGenres()
        {
            return Genres.Include(genre => genre.Games)
                .ThenInclude(game => game.Developer)
                .Include(genre => genre.Games)
                .ThenInclude(game => game.Genres); // Нужно для загрузки остальных жанров в память
        }

        public IEnumerable<IDeveloperBase> GetDevelopers()
        {
            return Developers.Include(d => d.Games)
                .ThenInclude(g => g.Genres);
        }
    }
}

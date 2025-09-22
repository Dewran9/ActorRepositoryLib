using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorRepositoryLib
{
    public class ActorRepository
    {
        private readonly List<Actor> _actors = new();
        private int _nextId = 1;

        public IEnumerable<Actor> Get() => _actors;

        public Actor? GetById(int id) => _actors.FirstOrDefault(a => a.Id == id);

        public Actor Add(Actor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor), "Actor cannot be null");
            }
            actor.Id = _nextId++;
            _actors.Add(actor);
            return actor;
        }

        public Actor? Delete(int id)
        {
            var actor = GetById(id);
            if (actor != null)
            {
                _actors.Remove(actor);
            }
            return actor;
        }

        public Actor? Update(int id, Actor data)
        {
           if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Actor data cannot be null");
            }
           var existingActor = _actors.FirstOrDefault(a => a.Id == id);
            if (existingActor == null)
            {
                return null;
            }
            existingActor.Name = data.Name;
            existingActor.Birthyear = data.Birthyear;
            existingActor.Country = data.Country;

            return existingActor;
        }

        public IEnumerable<Actor> Get(int? birthYearBefore)
            => Get(birthYearBefore, null);
        public IEnumerable<Actor> Get(int? birthYearBefore, int? birthYearAfter)
        {
            IEnumerable<Actor> query = _actors;
            if (birthYearBefore.HasValue)
            {
                query = query.Where(a => a.Birthyear < birthYearBefore.Value);
            }
            if (birthYearAfter.HasValue)
            {
                query = query.Where(a => a.Birthyear > birthYearAfter.Value);
            }
            return query;
        }
        public IEnumerable<Actor> GetByName(string? nameContains)
        {
            if (string.IsNullOrWhiteSpace(nameContains))
            {
                return _actors;
            }
            var needle = nameContains.Trim();
            return _actors.Where(a => a.Name.Contains(needle, StringComparison.OrdinalIgnoreCase)).ToList();

        }

        public IEnumerable<Actor> Get(int? birthYearBefore, int? birthYearAfter, string? nameContains, string sortBy, bool ascending)
        {
            IEnumerable<Actor> query = _actors;
            if (birthYearBefore.HasValue)
                query = query.Where(a => a.Birthyear < birthYearBefore.Value);

            if (birthYearAfter.HasValue)
                query = query.Where(a => a.Birthyear > birthYearAfter.Value);

            if (!string.IsNullOrWhiteSpace(nameContains))
            {
                var needle = nameContains.Trim();
                query = query.Where(a => a.Name.Contains(needle, StringComparison.OrdinalIgnoreCase));
            }
            query = sortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy (a => a.Name) : query.OrderByDescending(a => a.Name),
                "birthyear" => ascending ? query.OrderBy(a => a.Birthyear) : query.OrderByDescending(a => a.Birthyear),
                _ => ascending ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id),
            };
            return query;
        }

            

    }
}

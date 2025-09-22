using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorRepositoryLib
{
    public class Actor
    {
        private int _id;
        private string _name;
        private int _birthyear;
        public string? Country { get; set; }
        public int Id
        {
            get => _id;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Id must be non-negative");
                }
                _id = value;
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be null or empty");
                }
                if (value.Length < 4)
                {
                    throw new ArgumentException("Name cannot be shorter than 4 characters");
                }
                _name = value;
            }
        }

        public int Birthyear
        {
            get => _birthyear;
            set
            {
                if (value < 1820 || value > DateTime.Now.Year)
                {
                    throw new ArgumentException("Birthyear must be between 1820 and the current year");
                }
                _birthyear = value;
            }
        }

        public Actor(int id, string name, int birthyear, string? country = null)
        {
            Id = id;
            Name = name;
            Birthyear = birthyear;
            Country = country;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Birthyear: ({Birthyear}), Country: {Country ?? "N/A"}";
        }


    }


    }

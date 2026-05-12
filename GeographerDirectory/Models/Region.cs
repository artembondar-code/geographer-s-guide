using System.Collections.Generic;

namespace GeographerDirectory.Models
{
    public class Region : GeographicObject
    {
        public string Type { get; set; } // Вид (область, штат, провінція)
        public string Capital { get; set; } // Столиця/Адмін. центр

        public List<City> Cities { get; set; } = new List<City>();

        public Region() { }

        public Region(string name, int population, string type, string capital)
            : base(name, population)
        {
            Type = type;
            Capital = capital;
        }
    }
}
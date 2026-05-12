using System.Collections.Generic;

namespace GeographerDirectory.Models
{
    public class Country : GeographicObject
    {
        public double Area { get; set; } // Площа
        public string GovernmentForm { get; set; } // Форма державного правління
        public string Capital { get; set; } // Столиця

        public List<Region> Regions { get; set; } = new List<Region>();

        public Country() { }

        public Country(string name, int population, double area, string governmentForm, string capital)
            : base(name, population)
        {
            Area = area;
            GovernmentForm = governmentForm;
            Capital = capital;
        }
    }
}
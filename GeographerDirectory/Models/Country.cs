using System.Collections.Generic;
using System.ComponentModel;

namespace GeographerDirectory.Models
{
    public class Country : GeographicObject
    {
        [DisplayName("3. Площа (кв. км)")]
        public double Area { get; set; }

        [DisplayName("4. Форма правління")]
        public string GovernmentForm { get; set; }

        [DisplayName("5. Столиця")]
        public string Capital { get; set; }

        [Browsable(false)] // Ховаємо список регіонів
        public List<Region> Regions { get; set; } = new List<Region>();

        public Country() { }

        public Country(string name, long population, double area, string governmentForm, string capital)
            : base(name, population)
        {
            Area = area;
            GovernmentForm = governmentForm;
            Capital = capital;
        }
    }
}
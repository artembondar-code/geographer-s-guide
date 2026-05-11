using System.Collections.Generic;

namespace GeographerDirectory.Models
{
    /// <summary> Клас країни. </summary>
    public class Country : GeographicObject
    {
        /// <summary> Площа території. </summary>
        public double Area { get; set; }

        /// <summary> Форма державного правління. </summary>
        public string GovernmentForm { get; set; }

        /// <summary> Столиця країни. </summary>
        public City Capital { get; set; }

        /// <summary> Список регіонів країни. </summary>
        public List<Region> Regions { get; set; } = new List<Region>();

        /// <summary> Порожній конструктор для серіалізації. </summary>
        public Country() { }

        /// <summary> Ініціалізує об'єкт країни. </summary>
        public Country(string name, int population, double area, string governmentForm)
            : base(name, population)
        {
            Area = area;
            GovernmentForm = governmentForm;
        }
    }
}
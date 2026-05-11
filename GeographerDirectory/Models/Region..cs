using System.Collections.Generic;

namespace GeographerDirectory.Models
{
    /// <summary> Клас регіону (області, штату, провінції). </summary>
    public class Region : GeographicObject
    {
        /// <summary> Вид регіону. </summary>
        public string Type { get; set; }

        /// <summary> Назва країни, до якої належить регіон. </summary>
        public string CountryName { get; set; }

        /// <summary> Адміністративний центр регіону. </summary>
        public City Capital { get; set; }

        /// <summary> Список міст, що входять до складу регіону. </summary>
        public List<City> Cities { get; set; } = new List<City>();

        /// <summary> Порожній конструктор для серіалізації. </summary>
        public Region() { }

        /// <summary> Ініціалізує об'єкт регіону. </summary>
        public Region(string name, int population, string type, string countryName)
            : base(name, population)
        {
            Type = type;
            CountryName = countryName;
        }
    }
}
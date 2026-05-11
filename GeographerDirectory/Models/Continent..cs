using System.Collections.Generic;
using System.Linq;

namespace GeographerDirectory.Models
{
    /// <summary> Клас материка. </summary>
    public class Continent : GeographicObject
    {
        /// <summary> Список країн, розташованих на материку. </summary>
        public List<Country> Countries { get; set; } = new List<Country>();

        /// <summary> Порожній конструктор для серіалізації. </summary>
        public Continent() { }

        /// <summary> Ініціалізує об'єкт материка. </summary>
        public Continent(string name) : base(name, 0)
        {
        }

        /// <summary> Динамічно обчислює загальне населення материка. </summary>
        public int GetTotalPopulation()
        {
            return Countries.Sum(country => country.Population);
        }
    }
}
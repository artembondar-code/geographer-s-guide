using System.Collections.Generic;
using System.ComponentModel;

namespace GeographerDirectory.Models
{
    public class Region : GeographicObject
    {
        [DisplayName("3. Вид (область, штат)")]
        public string Type { get; set; }

        [DisplayName("4. Адмін. центр")]
        public string Capital { get; set; }

        [Browsable(false)] // Ховаємо список міст
        public List<City> Cities { get; set; } = new List<City>();

        public Region() { }

        public Region(string name, long population, string type, string capital)
            : base(name, population)
        {
            Type = type;
            Capital = capital;
        }
    }
}
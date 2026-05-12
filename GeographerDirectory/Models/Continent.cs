using System.Collections.Generic;
using System.ComponentModel;

namespace GeographerDirectory.Models
{
    public class Continent : GeographicObject
    {
        [Browsable(false)] // МАГІЯ: Ховаємо цю страшну колекцію з таблиці!
        public List<Country> Countries { get; set; } = new List<Country>();

        public Continent() { }

        public Continent(string name) : base(name, 0) { }
    }
}
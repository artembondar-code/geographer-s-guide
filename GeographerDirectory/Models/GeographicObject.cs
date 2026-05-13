using System.ComponentModel; // Обов'язкова бібліотека для красивих назв!

namespace GeographerDirectory.Models
{
    public abstract class GeographicObject
    {
        [DisplayName("1. Назва")] // Цифри допоможуть зберегти порядок у таблиці
        public string Name { get; set; }

        [DisplayName("2. Населення (осіб)")]
        public long Population { get; set; }

        public GeographicObject() { }

        public GeographicObject(string name, long population)
        {
            Name = name;
            Population = population;
        }
    }
}
namespace GeographerDirectory.Models
{
    /// <summary> Базовий клас для всіх географічних об'єктів. </summary>
    public abstract class GeographicObject
    {
        /// <summary> Назва об'єкта. </summary>
        public string Name { get; set; }

        /// <summary> Чисельність населення. </summary>
        public int Population { get; set; }

        /// <summary> Порожній конструктор для серіалізації. </summary>
        public GeographicObject() { }

        /// <summary> Ініціалізує базові властивості об'єкта. </summary>
        public GeographicObject(string name, int population)
        {
            Name = name;
            Population = population;
        }
    }
}
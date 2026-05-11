namespace GeographerDirectory.Models
{
    /// <summary> Клас міста з географічними координатами. </summary>
    public class City : GeographicObject
    {
        /// <summary> Географічна широта. </summary>
        public double Latitude { get; set; }

        /// <summary> Географічна довгота. </summary>
        public double Longitude { get; set; }

        /// <summary> Порожній конструктор для серіалізації. </summary>
        public City() { }

        /// <summary> Ініціалізує об'єкт міста. </summary>
        public City(string name, int population, double latitude, double longitude)
            : base(name, population)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
namespace GeographerDirectory.Models
{
    public class City : GeographicObject
    {
        public string Coordinates { get; set; } // Географічні координати (наприклад: "50.00, 36.23")

        public City() { }

        public City(string name, long population, string coordinates)
            : base(name, population)
        {
            Coordinates = coordinates;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GeographerDirectory.Models;

namespace GeographerDirectory.Storage
{
    /// <summary> Клас для управління збереженням та завантаженням даних. </summary>
    public class DataManager
    {
        // Файл автоматично створиться поруч із виконуваним файлом програми
        private readonly string _filePath = "geodata.json";

        /// <summary> Зберігає список материків у файл. </summary>
        public void SaveData(List<Continent> continents)
        {
            // WriteIndented робить файл JSON красиво відформатованим
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(continents, options);
            File.WriteAllText(_filePath, jsonString);
        }

        /// <summary> Завантажує список материків з файлу. </summary>
        public List<Continent> LoadData()
        {
            // Якщо файлу ще немає (перший запуск), повертаємо порожній список
            if (!File.Exists(_filePath))
            {
                return new List<Continent>();
            }

            string jsonString = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Continent>>(jsonString) ?? new List<Continent>();
        }
    }
}
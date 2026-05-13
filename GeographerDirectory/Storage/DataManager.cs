using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GeographerDirectory.Models;

namespace GeographerDirectory.Storage
{
    public class DataManager
    {
        // Метод збереження тепер приймає шлях до файлу (filePath)
        public void SaveData(List<Continent> continents, string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true
                };
                string jsonString = JsonSerializer.Serialize(continents, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Помилка при збереженні: " + ex.Message);
            }
        }

        // Метод завантаження тепер теж приймає шлях до файлу (filePath)
        public List<Continent> LoadData(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return new List<Continent>();

                string jsonString = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<List<Continent>>(jsonString);

                return data ?? new List<Continent>();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Помилка при завантаженні: " + ex.Message);
                return new List<Continent>();
            }
        }
    }
}
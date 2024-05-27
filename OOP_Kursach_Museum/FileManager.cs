using System.Collections.Generic;
using System.IO;

namespace OOP_Kursach_Museum
{
    /// <summary>
    /// Статический класс для управления файлами, содержащими данные о музейных экспонатах.
    /// </summary>
    public static class FileManager
    {
        /// <summary>
        /// Путь к файлу с данными.
        /// </summary>
        private static string filePath = "input.txt";

        /// <summary>
        /// Считывает данные из файла и возвращает список музейных экспонатов.
        /// </summary>
        /// <returns>Список музейных экспонатов.</returns>
        public static List<Museum> ReadFromFile()
        {
            List<Museum> museums = new List<Museum>();
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 4 && int.TryParse(parts[1], out int year) && bool.TryParse(parts[3], out bool onExhibit))
                    {
                        museums.Add(new Museum(parts[0], year, parts[2], onExhibit));
                    }
                }
            }
            return museums;
        }

        /// <summary>
        /// Записывает список музейных экспонатов в файл, перезаписывая его.
        /// </summary>
        /// <param name="museums">Список музейных экспонатов для записи в файл.</param>
        public static void WriteToFile(List<Museum> museums)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (var museum in museums)
                {
                    sw.WriteLine($"{museum.Name}|{museum.Year}|{museum.ExhibitName}|{museum.OnExhibit}");
                }
            }
        }

        /// <summary>
        /// Добавляет один музейный экспонат в файл.
        /// </summary>
        /// <param name="museum">Музейный экспонат для добавления в файл.</param>
        public static void AppendToFile(Museum museum)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine($"{museum.Name}|{museum.Year}|{museum.ExhibitName}|{museum.OnExhibit}");
            }
        }

        /// <summary>
        /// Удаляет файл с данными.
        /// </summary>
        public static void DeleteFile()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}

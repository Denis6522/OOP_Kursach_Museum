using System;

namespace OOP_Kursach_Museum
{
    /// <summary>
    /// Структура, представляющая музейный экспонат.
    /// </summary>
    public struct Museum
    {
        /// <summary>
        /// Получает или задает имя автора.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Получает или задает год создания экспоната.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Получает или задает название экспоната.
        /// </summary>
        public string ExhibitName { get; set; }

        /// <summary>
        /// Получает или задает значение, указывающее, находится ли экспонат на выставке.
        /// </summary>
        public bool OnExhibit { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр структуры <see cref="Museum"/>.
        /// </summary>
        /// <param name="name">Имя автора.</param>
        /// <param name="year">Год создания экспоната.</param>
        /// <param name="exhibitName">Название экспоната.</param>
        /// <param name="onExhibit">Значение, указывающее, находится ли экспонат на выставке.</param>
        public Museum(string name, int year, string exhibitName, bool onExhibit)
        {
            Name = name;
            Year = year;
            ExhibitName = exhibitName;
            OnExhibit = onExhibit;
        }
    }
}

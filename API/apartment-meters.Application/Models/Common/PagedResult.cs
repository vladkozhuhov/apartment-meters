using System.Collections.Generic;

namespace Application.Models.Common
{
    /// <summary>
    /// Модель для возврата данных с пагинацией
    /// </summary>
    /// <typeparam name="T">Тип элементов в коллекции</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Список элементов на текущей странице
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Общее количество элементов
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Общее количество страниц
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Есть ли предыдущая страница
        /// </summary>
        public bool HasPrevious => Page > 1;

        /// <summary>
        /// Есть ли следующая страница
        /// </summary>
        public bool HasNext => Page < TotalPages;
    }
} 
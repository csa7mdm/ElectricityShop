using System.Collections.Generic;

namespace ElectricityShop.API.Models
{
    /// <summary>
    /// Generic wrapper for paginated API responses
    /// </summary>
    /// <typeparam name="T">Type of items in the collection</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// Current page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Whether there's a previous page available
        /// </summary>
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// Whether there's a next page available
        /// </summary>
        public bool HasNext => PageNumber < TotalPages;

        /// <summary>
        /// The collection of items for the current page
        /// </summary>
        public IList<T> Items { get; set; }

        public PagedResponse()
        {
            Items = new List<T>();
        }

        public PagedResponse(IList<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }
    }
}
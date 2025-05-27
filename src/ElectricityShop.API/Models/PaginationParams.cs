namespace ElectricityShop.API.Models
{
    /// <summary>
    /// Standard pagination parameters for API requests
    /// </summary>
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        private int _pageNumber = 1;

        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        /// <summary>
        /// Number of items per page (default: 10, max: 50)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
        }
    }
}
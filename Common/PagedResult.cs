namespace SchoolApp.Common
{
    public class PagedResult<T>
    {
        // The actual records for the current page.
        // Example: 10 departments from page 1.
        public IEnumerable<T> Items { get; set; } = new List<T>();

        // Total number of records in the database before pagination.
        // Example: 100 departments total.
        public int TotalCount { get; set; }

        // Current page number requested by the user.
        // Example: pageNumber = 1.
        public int PageNumber { get; set; }

        // Number of records per page.
        // Example: pageSize = 10.
        public int PageSize { get; set; }

        // Total pages calculated automatically.
        // Example: TotalCount = 100, PageSize = 10, TotalPages = 10.
        public int TotalPages =>
            (int)Math.Ceiling((double)TotalCount / PageSize);
    }

}



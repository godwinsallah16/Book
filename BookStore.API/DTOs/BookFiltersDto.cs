namespace BookStore.API.DTOs
{
    public class BookFiltersDto
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public string? Author { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
    }
}

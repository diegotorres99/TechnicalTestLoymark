namespace back_end.DTOs
{
    public class CountryApiResponse
    {
        public string? cca3 { get; set; } 
        public CountryName? name { get; set; }
    }
    public class CountryName
    {
        public string? common { get; set; } 
    }
}

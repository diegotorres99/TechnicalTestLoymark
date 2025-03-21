namespace back_end.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }  
        public string? Name { get; set; }  
        public string? LastName { get; set; }  
        public string? Email { get; set; }  
        public DateTime BirthDate { get; set; }  
        public string? Phone { get; set; }  
        public string? Country { get; set; }  
        public bool ContactQuestion { get; set; }  
    }

}

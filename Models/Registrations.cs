namespace LittleHairSalon.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
namespace WebApplication1.Models.Faculty
{
    public class SuggestionView
    {
        public int SuggestionId { get; set; }
        public string? Title { get; set; }

        public string? Message { get; set; }
        public DateTime PostedAt { get; set; }

        public string? Status { get; set; } 

        public string? Reply { get; set; }

        public string? FactName { get; set; }


    }
}

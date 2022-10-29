namespace LawyerAPI.Models
{
    public class Presentation
    {
        public int ID { get; set; }
        public string? CourtCaseNo { get; set; }
        public int LawyerId { get; set; }
        public int Available { get; set; }
    }
}
namespace LawyerAPI.Models
{
    public class User
    {
        public int? ID { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int Remember_token { get; set; }

    }
}

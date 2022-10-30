namespace LawyerAPI.Helper
{
    public class AgendaDto
    {
        public int ID { get; set; }
        public string? CourtCaseNo { get; set; }
        public string? CourtLocation { get; set; }
        public string? HearingDate { get; set; }
        public string? HearingTime { get; set; }
        public string? HearingType { get; set; }
        public string? ChamberID { get; set; }
        public string? CourtType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UploaderEmail { get; set; }
    }
}

namespace TestAPI2.Models.DTOs
{
    public class FacturaDto
    {
        public ClienteDto? Cliente { get; set; }

        public DateTime? Fecha { get; set; }
    }
}

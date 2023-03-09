namespace TestAPI2.Models.DTOs;

public class ClienteDto
{ 
    public string? NombreCliente { get; set; }

    public string? ApellidoCliente { get; set; }

    public string? EmailCliente { get; set; }

    public string? DireccionCliente { get; set; }

    public string ToCSV() 
    {
        return String.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\"", NombreCliente, ApellidoCliente, EmailCliente, DireccionCliente);
        //return String.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\"", NombreCliente, ApellidoCliente, EmailCliente, DireccionCliente);
    }
}

namespace PracticaEmpresas.Models
{
    //Objeto que se recibe de App Angular para mandar a crear la empresa a Banco
    public class ItemEmpresaPost
    {
        public string correoElectronico { get; set; }
        public string direccion { get; set; }
        public string estado { get; set; }
        public string nit { get; set; }
        public string nombreComercial { get; set; }
        public string razonSocial { get; set; }
        public string telefono { get; set; }
    }
}

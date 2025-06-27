namespace ParInpar.Models
{
    public class UsuarioRegistroDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = "usuario";
    }
}

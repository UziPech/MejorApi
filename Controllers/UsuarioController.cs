using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParInpar.Models;

namespace ParInpar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] UsuarioRegistroDto usuarioDto)
        {
            // Validar correo
            if (string.IsNullOrWhiteSpace(usuarioDto.Correo) || !System.Text.RegularExpressions.Regex.IsMatch(usuarioDto.Correo, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                return BadRequest("El correo no es válido.");

            // Validar contraseña (solo letras y números, mínimo 6 caracteres)
            if (string.IsNullOrWhiteSpace(usuarioDto.Contrasena) || !System.Text.RegularExpressions.Regex.IsMatch(usuarioDto.Contrasena, @"^[a-zA-Z0-9]{6,}$"))
                return BadRequest("La contraseña debe tener al menos 6 caracteres y solo letras y números.");

            // Validar rol
            var rolesValidos = new[] { "admin", "usuario", "cliente" };
            if (string.IsNullOrWhiteSpace(usuarioDto.Rol) || !rolesValidos.Contains(usuarioDto.Rol.ToLower()))
                return BadRequest("El rol solo puede ser admin, usuario o cliente.");

            // Evitar duplicados por correo
            if (_context.Usuarios.Any(u => u.Correo == usuarioDto.Correo))
            {
                return BadRequest("El correo ya está registrado.");
            }

            var usuario = new Usuario
            {
                Correo = usuarioDto.Correo,
                Contrasena = usuarioDto.Contrasena,
                Rol = usuarioDto.Rol
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok("Usuario registrado correctamente.");
        }

        [HttpGet("paginado")]
        [Authorize(Roles = "admin")]
        public IActionResult ObtenerUsuariosPaginado(int page = 1, int pageSize = 10)
        {
            if (!EsNumeroValido(page) || !EsNumeroValido(pageSize))
                return BadRequest(new { mensaje = "Solo se permiten números enteros positivos." });

            var total = _context.Usuarios.Count();
            var datos = _context.Usuarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new { pagina = page, tamanoPagina = pageSize, totalRegistros = total, datos });
        }

        private bool EsNumeroValido(int numero) => numero > 0;
        // Validación para solo letras y números (sin caracteres especiales)
        private bool SoloLetrasYNumeros(string input) => System.Text.RegularExpressions.Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");
    }
}


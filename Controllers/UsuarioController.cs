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
        public IActionResult Registrar([FromBody] Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Correo == usuario.Correo))
            {
                return BadRequest("El correo ya está registrado.");
            }

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
    }
}


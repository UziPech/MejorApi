using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParInpar.Models;
using System.Linq;

namespace ParInpar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CifradoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CifradoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("encriptar")]
        public IActionResult EncriptarTextoPost([FromBody] TextoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Texto))
                return BadRequest(new { mensaje = "El texto no puede estar vacío." });

            var resultado = CifrarCesar(request.Texto);
            return Ok(new { textoOriginal = request.Texto, textoCifrado = resultado });
        }

        [HttpPost("descencriptar")]
        public IActionResult DesencriptarTextoPost([FromBody] TextoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Texto))
                return BadRequest(new { mensaje = "El texto no puede estar vacío." });

            var resultado = DescifrarCesar(request.Texto);
            return Ok(new { textoCifrado = request.Texto, textoDescifrado = resultado });
        }

        [HttpGet]
        public IActionResult GetTodos()
        {
            try
            {
                return Ok(_context.Cifrados.ToList());
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los registros." });
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Guardar([FromBody] TextoCifrado nuevo)
        {
            if (string.IsNullOrWhiteSpace(nuevo.TextoOriginal) || string.IsNullOrWhiteSpace(nuevo.TextoCifradoValor))
                return BadRequest(new { mensaje = "El texto original y cifrado no pueden estar vacíos." });

            try
            {
                _context.Cifrados.Add(nuevo);
                _context.SaveChanges();
                return Ok(nuevo);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al guardar el texto cifrado." });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Editar(int id, [FromBody] TextoCifrado actualizado)
        {
            var existente = _context.Cifrados.Find(id);
            if (existente == null)
                return NotFound(new { mensaje = "Registro no encontrado." });

            if (string.IsNullOrWhiteSpace(actualizado.TextoOriginal) || string.IsNullOrWhiteSpace(actualizado.TextoCifradoValor))
                return BadRequest(new { mensaje = "Los campos de texto no pueden estar vacíos." });

            existente.TextoOriginal = actualizado.TextoOriginal;
            existente.TextoCifradoValor = actualizado.TextoCifradoValor;
            existente.Desplazamiento = actualizado.Desplazamiento;
            existente.FechaRegistro = actualizado.FechaRegistro;

            try
            {
                _context.SaveChanges();
                return Ok(existente);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el registro." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Eliminar(int id)
        {
            var encontrado = _context.Cifrados.Find(id);
            if (encontrado == null)
                return NotFound(new { mensaje = "Registro no encontrado." });

            try
            {
                _context.Cifrados.Remove(encontrado);
                _context.SaveChanges();
                return Ok(new { mensaje = "Registro eliminado correctamente." });
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el registro." });
            }
        }

        [HttpGet("paginado")]
        [Authorize(Roles = "admin")]
        public IActionResult ObtenerCifradosPaginado(int page = 1, int pageSize = 10)
        {
            if (!EsNumeroValido(page) || !EsNumeroValido(pageSize))
                return BadRequest(new { mensaje = "Solo se permiten números enteros positivos." });

            try
            {
                var total = _context.Cifrados.Count();
                var datos = _context.Cifrados
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new { pagina = page, tamanoPagina = pageSize, totalRegistros = total, datos });
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los datos paginados." });
            }
        }

        private string CifrarCesar(string texto)
        {
            return new string(texto.Select(c => (char)(c + 3)).ToArray());
        }

        private string DescifrarCesar(string texto)
        {
            return new string(texto.Select(c => (char)(c - 3)).ToArray());
        }

        private bool EsNumeroValido(int numero) => numero > 0;
    }

    public class TextoRequest
    {
        public string Texto { get; set; } = string.Empty;
    }
}







using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParInpar.Models;
using System.Text.RegularExpressions;

namespace ParInpar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PalindromoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PalindromoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("verificar/{palabra}")]
        public IActionResult VerificarPalabraUrl(string palabra)
        {
            if (!EsPalabraValida(palabra))
                return BadRequest(new { mensaje = "La palabra no debe contener números, símbolos ni espacios, y debe tener al menos 2 letras." });

            bool esPalindromo = EsPalindromo(palabra);
            return Ok(new { palabra, esPalindromo });
        }

        [HttpPost("verificar")]
        public IActionResult VerificarPalabraJson([FromBody] PalabraRequest request)
        {
            if (!EsPalabraValida(request.Palabra))
                return BadRequest(new { mensaje = "La palabra no debe contener números, símbolos ni espacios, y debe tener al menos 2 letras." });

            bool esPalindromo = EsPalindromo(request.Palabra);
            return Ok(new { palabra = request.Palabra, esPalindromo });
        }

        [HttpGet]
        public IActionResult ObtenerPalabras()
        {
            return Ok(_context.PalabrasVerificadas.ToList());
        }

        [HttpPost]
        [Authorize]
        public IActionResult GuardarPalabra([FromBody] PalabraRequest request)
        {
            if (!EsPalabraValida(request.Palabra))
                return BadRequest(new { mensaje = "La palabra no debe contener números, símbolos ni espacios, y debe tener al menos 2 letras." });
            // Validar duplicados
            if (_context.PalabrasVerificadas.Any(p => p.Palabra == request.Palabra))
                return BadRequest(new { mensaje = "La palabra ya está registrada." });

            var nueva = new PalabraVerificada
            {
                Palabra = request.Palabra,
                EsPalindromo = EsPalindromo(request.Palabra)
            };

            _context.PalabrasVerificadas.Add(nueva);
            _context.SaveChanges();

            return CreatedAtAction(nameof(ObtenerPalabras), new { id = nueva.Id }, nueva);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult ActualizarPalabra(string id, [FromBody] PalabraRequest request)
        {
            // Validar id solo números positivos
            if (!int.TryParse(id, out int idNum) || idNum <= 0)
                return BadRequest(new { mensaje = "El id debe ser un número entero positivo." });

            if (!EsPalabraValida(request.Palabra))
                return BadRequest(new { mensaje = "La palabra no debe contener números, símbolos ni espacios, y debe tener al menos 2 letras." });

            var palabraExistente = _context.PalabrasVerificadas.FirstOrDefault(p => p.Id == idNum);
            if (palabraExistente == null)
                return NotFound(new { mensaje = "Palabra no encontrada" });
            // Validar duplicados al editar
            if (_context.PalabrasVerificadas.Any(p => p.Palabra == request.Palabra && p.Id != idNum))
                return BadRequest(new { mensaje = "Ya existe un registro con esa palabra." });

            palabraExistente.Palabra = request.Palabra;
            palabraExistente.EsPalindromo = EsPalindromo(request.Palabra);

            _context.SaveChanges();
            return Ok(palabraExistente);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult EliminarPalabra(string id)
        {
            // Validar id solo números positivos
            if (!int.TryParse(id, out int idNum) || idNum <= 0)
                return BadRequest(new { mensaje = "El id debe ser un número entero positivo." });

            var palabra = _context.PalabrasVerificadas.FirstOrDefault(p => p.Id == idNum);
            if (palabra == null)
                return NotFound(new { mensaje = "No existe el registro" });

            _context.PalabrasVerificadas.Remove(palabra);
            _context.SaveChanges();

            return Ok(new { mensaje = "Eliminado correctamente" });
        }

        [HttpGet("paginado")]
        [Authorize(Roles = "admin")]
        public IActionResult ObtenerPalabrasPaginado(int page = 1, int pageSize = 10)
        {
            if (!EsNumeroValido(page) || !EsNumeroValido(pageSize))
                return BadRequest(new { mensaje = "Solo se permiten números enteros positivos." });

            var total = _context.PalabrasVerificadas.Count();
            var datos = _context.PalabrasVerificadas
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new { pagina = page, tamanoPagina = pageSize, totalRegistros = total, datos });
        }

        private bool EsPalindromo(string palabra)
        {
            var limpia = new string(
                palabra.ToLower()
                       .Replace(" ", "")
                       .Where(c => char.IsLetter(c) || "áéíóúüñÁÉÍÓÚÜÑ".Contains(c))
                       .ToArray());

            return limpia.Length >= 2 && limpia.SequenceEqual(limpia.Reverse());
        }

        private bool EsPalabraValida(string palabra)
        {
            return Regex.IsMatch(palabra, @"^[a-zA-ZáéíóúüñÁÉÍÓÚÜÑ]{2,}$");
        }

        private bool EsNumeroValido(int numero) => numero > 0;
    }

    public class PalabraRequest
    {
        public string Palabra { get; set; } = string.Empty;
    }
}



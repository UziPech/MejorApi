using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParInpar.Models;

namespace ParInpar.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NumeroController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NumeroController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTodos()
        {
            try
            {
                return Ok(_context.Numeros.ToList());
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener los datos." });
            }
        }

        [HttpGet("verificar/{numero}")]
        public IActionResult Verificar(string numero)
        {
            if (!int.TryParse(numero, out int valor) || !EsNumeroValido(valor))
            {
                return BadRequest(new { mensaje = "El número ingresado es inválido. Solo se permiten enteros positivos menores o iguales a 100000." });
            }

            var resultado = valor % 2 == 0 ? "Par" : "Impar";
            return Ok(new { numero = valor, resultado });
        }

        [HttpPost("verificar")]
        public IActionResult VerificarPost([FromBody] NumeroVerificado numero)
        {
            if (!EsNumeroValido(numero.Valor))
                return BadRequest(new { mensaje = "El número ingresado es inválido. Solo se permiten enteros positivos menores o iguales a 100000." });

            var resultado = numero.Valor % 2 == 0 ? "Par" : "Impar";
            return Ok(new { numero.Valor, resultado });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Guardar([FromBody] NumeroVerificado numero)
        {
            // Validar duplicados por valor
            if (_context.Numeros.Any(n => n.Valor == numero.Valor))
                return BadRequest(new { mensaje = "El número ya está registrado." });
            // Validar valor
            if (!EsNumeroValido(numero.Valor))
                return BadRequest(new { mensaje = "El número ingresado es inválido. Solo se permiten enteros positivos menores o iguales a 100000." });
            try
            {
                numero.EsPar = numero.Valor % 2 == 0;
                _context.Numeros.Add(numero);
                _context.SaveChanges();
                return Ok(numero);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al guardar el número." });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Editar(string id, [FromBody] NumeroVerificado numeroEditado)
        {
            // Validar id solo números positivos
            if (!int.TryParse(id, out int idNum) || idNum <= 0)
                return BadRequest(new { mensaje = "El id debe ser un número entero positivo." });
            if (!EsNumeroValido(numeroEditado.Valor))
                return BadRequest(new { mensaje = "El número ingresado es inválido. Solo se permiten enteros positivos menores o iguales a 100000." });
            var numero = _context.Numeros.FirstOrDefault(n => n.Id == idNum);
            if (numero == null)
                return NotFound(new { mensaje = "Número no encontrado" });
            // Validar duplicados al editar
            if (_context.Numeros.Any(n => n.Valor == numeroEditado.Valor && n.Id != idNum))
                return BadRequest(new { mensaje = "Ya existe un registro con ese valor." });
            numero.Valor = numeroEditado.Valor;
            numero.EsPar = numeroEditado.Valor % 2 == 0;
            try
            {
                _context.SaveChanges();
                return Ok(numero);
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el número." });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Eliminar(string id)
        {
            // Validar id solo números positivos
            if (!int.TryParse(id, out int idNum) || idNum <= 0)
                return BadRequest(new { mensaje = "El id debe ser un número entero positivo." });
            var numero = _context.Numeros.FirstOrDefault(n => n.Id == idNum);
            if (numero == null)
                return NotFound(new { mensaje = "Número no encontrado" });
            try
            {
                _context.Numeros.Remove(numero);
                _context.SaveChanges();
                return Ok(new { mensaje = "Número eliminado correctamente" });
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el número." });
            }
        }

        [HttpGet("paginado")]
        [Authorize(Roles = "admin")]
        public IActionResult ObtenerNumerosPaginado(int page = 1, int pageSize = 10)
        {
            if (!EsNumeroValido(page) || !EsNumeroValido(pageSize))
                return BadRequest(new { mensaje = "El número ingresado es inválido. Solo se permiten enteros positivos menores o iguales a 100000." });

            try
            {
                var total = _context.Numeros.Count();
                var datos = _context.Numeros
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(new { pagina = page, tamanoPagina = pageSize, totalRegistros = total, datos });
            }
            catch
            {
                return StatusCode(500, new { mensaje = "Error al obtener la paginación." });
            }
        }

        private bool EsNumeroValido(int numero)
        {
            return numero > 0 && numero <= 100000;
        }
    }
}




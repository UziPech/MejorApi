using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParInpar.Models
{
    [Table("Palindromos")] // 👈 Mapeamos el nombre real de la tabla en Railway
    public class PalabraVerificada
    {
        [Key]
        public int Id { get; set; }

        public string Palabra { get; set; } = string.Empty;

        public bool EsPalindromo { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now; // 👈 Para llenar automáticamente
    }
}


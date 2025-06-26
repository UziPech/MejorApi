using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParInpar.Models
{
    public class TextoCifrado
    {
        public int Id { get; set; }
        public string TextoOriginal { get; set; } = string.Empty;

        [Column("textoCifrado")]
        public string TextoCifradoValor { get; set; } = string.Empty;

        public int Desplazamiento { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}


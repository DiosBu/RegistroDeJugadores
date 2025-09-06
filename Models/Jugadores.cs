using System.ComponentModel.DataAnnotations;

namespace RegistroDeJugadores.Models
{
    public class Jugadores
    {
        [Key]
        public int JugadorId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Las partidas son obligatorias")]
        [Range(0, int.MaxValue, ErrorMessage = "Las partidas deben ser >= 0")]
        public int Partidas { get; set; }
    }
}


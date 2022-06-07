namespace SistemaBiblioteca.Models {
    public class Reserva {
        public int Id { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasReserva { get; set; }
        public ICollection<Material> Materiales { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}

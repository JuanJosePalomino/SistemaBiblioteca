namespace SistemaBiblioteca.Models {
    public class Prestamo {

        public int Id { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual ICollection<Material> Materiales { get; set; }
    }
}

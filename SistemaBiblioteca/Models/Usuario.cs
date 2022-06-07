namespace SistemaBiblioteca.Models {
    public class Usuario {
        public int Id { get; set; }

        public string Documento { get; set; }

        public string Nombre { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Rol { get; set; }

        public virtual ICollection<Reserva> Reservas { get; set; }

        public virtual ICollection<Prestamo> Prestamos { get; set; }
    }
}

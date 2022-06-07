namespace SistemaBiblioteca.Models{
    public class Material{

        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Paginas { get; set; }
        public string Tipo { get; set; }
        public string Categoria { get; set; }
        public bool State { get; set; }
        public int? PrestamoId { get; set; }
        public int? ReservaId { get; set; }
        public ICollection<Autor> Autores { get; set; }
        public virtual Prestamo Prestamo { get; set; }
        public virtual Reserva Reserva { get; set; }
    }
}

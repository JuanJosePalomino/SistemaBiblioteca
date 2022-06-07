using SistemaBiblioteca.Models;

namespace SistemaBiblioteca.Request {
    public class ReservaRequest {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasReserva { get; set; }
        public List<int> MaterialIds { get; set; }
        public int IdUsuario { get; set; }
        public string HTMLFactura { get; set; }
        

    }
}

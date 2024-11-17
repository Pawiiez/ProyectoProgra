using SQLite;

namespace ProyectoProgra.Models
{
    public class UserReport
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Report { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ProblemType { get; set; } = string.Empty; // Añadir esta línea
        public string Description { get; set; } = string.Empty; // Asegúrate de que la propiedad 'Description' también esté definida
    }
}

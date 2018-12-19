using System;

namespace AppDiaryHobbies
{
    class Tarea
    {
        public int IdTarea { get; set; }
        public int IdUsuario { get; set; }
        public string NombreTarea { get; set; }
        public string TipoTarea { get; set; }
        public string DescripcionTarea { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public int? contador { get; set; }

        public Tarea()
        {

        }

        public Tarea(int IdTarea, int IdUsuario, string NombreTarea, string TipoTarea, string DescripcionTarea, DateTime HoraInicio, DateTime? HoraFin, int? contador = 0)
        {
            this.IdTarea = IdTarea;
            this.IdUsuario = IdUsuario;
            this.NombreTarea = NombreTarea;
            this.TipoTarea = TipoTarea;
            this.DescripcionTarea = DescripcionTarea;
            this.HoraInicio = HoraInicio;
            this.HoraFin = HoraFin;
            this.contador = contador;
        }
    }
}
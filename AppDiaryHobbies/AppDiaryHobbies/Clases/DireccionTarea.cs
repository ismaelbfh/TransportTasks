using System;

namespace AppDiaryHobbies
{
    class DireccionTarea
    {
        public DateTime HoraInicio { get; set; }
        public DateTime? HoraFin { get; set; }
        public string DireccionActual { get; set; }

        public DireccionTarea()
        {

        }

        public DireccionTarea(string DireccionActual, DateTime HoraInicio, DateTime? HoraFin)
        {
            this.DireccionActual = DireccionActual;
            this.HoraInicio = HoraInicio;
            this.HoraFin = HoraFin;
        }

    }
}
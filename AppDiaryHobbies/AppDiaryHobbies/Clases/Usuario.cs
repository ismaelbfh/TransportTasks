using System;

namespace AppDiaryHobbies
{
    class Usuario
    {
        public int Id { get; set; }
        public int IdEmpresa { get; set; }
        public string NombreUsuario { get; set; }
        public string Profesion { get; set; }
        public string Telefono { get; set; }
        public string Nick { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }

        public Usuario()
        {

        }

        public Usuario(int Id, int IdEmpresa, string NombreUsuario, string Profesion, string Telefono, string Nick, string Password, string Rol)
        {
            this.Id = Id;
            this.IdEmpresa = IdEmpresa;
            this.NombreUsuario = NombreUsuario;
            this.Profesion = Profesion;
            this.Telefono = Telefono;
            this.Nick = Nick;
            this.Password = Password;
            this.Rol = Rol;
        }

        public Usuario(int Id, string NombreUsuario, string Profesion, string Telefono, string Nick)
        {
            this.Id = Id;
            this.NombreUsuario = NombreUsuario;
            this.Profesion = Profesion;
            this.Telefono = Telefono;
            this.Nick = Nick;
        }

    }
}
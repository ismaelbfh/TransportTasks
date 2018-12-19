using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MySql.Data.MySqlClient;
using System.Data;

namespace AppDiaryHobbies
{
    [Activity(Label = "Modifica Usuario")]
    public class ActivityModificarUsuario : Activity
    {
        private EditText txtNombre, txtProfesion, txtTelefono, txtPassword, txtUsuario;
        private Button btnModificar;
        private ImageButton btnAtrasMod;
        private Spinner spEmpresas;
        private string rol, nombre, profesion, telefono, nick, password;
        private string idActual, nombreActual, profesionActual, telefonoActual, nickActual;
        private int idEmpresa, idUsuario, idEmpresaGet;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutModificarUsuario);

            txtNombre = FindViewById<EditText>(Resource.Id.txtNombreMod);
            txtProfesion = FindViewById<EditText>(Resource.Id.txtProfesionMod);
            txtTelefono = FindViewById<EditText>(Resource.Id.txtTelefonoMod);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPasswordMod);
            txtUsuario = FindViewById<EditText>(Resource.Id.txtUsuarioMod);
            btnModificar = FindViewById<Button>(Resource.Id.btnModificar);
            btnAtrasMod = FindViewById<ImageButton>(Resource.Id.btnAtrasMod);
            spEmpresas = FindViewById<Spinner>(Resource.Id.spEmpresasMod);
            
            //Estos son los que recoge de cuando pulsa un usuario para modificarlo
            nombre = Intent.GetStringExtra("NombreUsu");
            profesion = Intent.GetStringExtra("ProfesionUsu");
            telefono = Intent.GetStringExtra("TelefonoUsu");
            nick = Intent.GetStringExtra("NickUsu");
            password = Intent.GetStringExtra("PasswordUsu");
            idUsuario = Convert.ToInt32(Intent.GetStringExtra("IdUsu"));
            idEmpresaGet = Convert.ToInt32(Intent.GetStringExtra("IdEmpresa"));

            //Coje los datos del usuario que lo gestione para poder recuperar sus datos de una pantalla a otra, si se le da atras, etc.
            idActual = Intent.GetStringExtra("IdUsuActual");
            nombreActual = Intent.GetStringExtra("NombreActual");
            profesionActual = Intent.GetStringExtra("ProfesionActual");
            telefonoActual = Intent.GetStringExtra("TelefonoActual");
            nickActual = Intent.GetStringExtra("NickActual");
            //Si el usuario seleccionado es Admin, que cargue el rol que lo gestiona -> SuperAdmin y si es Usuario que lo cargue el Admin
            rol = Intent.GetStringExtra("RolUsu").Equals("Admin") ? "SuperAdmin" : (Intent.GetStringExtra("RolUsu").Equals("Usuario") ? "Admin" : "SuperAdmin");

            //Establece lo que habrá dentro del spinner
            List<string> empresas = ConsultaEmpresasBaseDatos();
            
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this,
                    Android.Resource.Layout.SimpleSpinnerDropDownItem, empresas);
            
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spEmpresas.Adapter = dataAdapter;
            ///////////////////////////////////////////

            //Escribe todos los campos para poder modificarlo:
            txtNombre.Text = nombre;
            txtPassword.Text = password;
            txtProfesion.Text = profesion;
            txtTelefono.Text = telefono;
            txtUsuario.Text = nick;
            spEmpresas.SetSelection(dataAdapter.GetPosition(DevuelveNombreEmpresaSeleccionadaById(idEmpresaGet)));
            //////////////////////////////////////////////////

            btnModificar.Click += delegate
            {
                CompruebaModificaDatosCorrectos();
            };

            btnAtrasMod.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityAnadirUsuario));
                intent.PutExtra("Rol", rol);
                intent.PutExtra("Id", idActual);
                intent.PutExtra("Nombre", nombreActual);
                intent.PutExtra("Profesion", profesionActual);
                intent.PutExtra("Usuario", nickActual);
                intent.PutExtra("Telefono", telefonoActual);
                StartActivity(intent);
            };
        }

        private string DevuelveNombreEmpresaSeleccionadaById(int idEmpresa)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            string nombreEmpresa = string.Empty;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nombre FROM Empresa WHERE Id = " + idEmpresa, con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                nombreEmpresa = reader.GetString(0).ToString();
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Toast.MakeText(ApplicationContext, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return nombreEmpresa;
        }

        private int DevuelveIdEmpresaSeleccionadoByName(string nombre)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            int idEmpresa = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Id FROM Empresa WHERE Nombre = '" + nombre + "'", con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                idEmpresa = Convert.ToInt32(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Toast.MakeText(ApplicationContext, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return idEmpresa;
        }

        private List<string> ConsultaEmpresasBaseDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            List<string> listaEmpresas = new List<string>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nombre FROM Empresa", con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaEmpresas.Add(reader.GetString(0).ToString());
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Toast.MakeText(ApplicationContext, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }

            return listaEmpresas;
        }
        
        private void CompruebaModificaDatosCorrectos()
        {
            if (txtNombre.Text.Equals("") || txtProfesion.Text.Equals("") || txtTelefono.Text.Equals("") ||
                txtPassword.Text.Equals("") || txtUsuario.Text.Equals(""))
            {
                Toast.MakeText(ApplicationContext, "Lo siento tienes que rellenar TODOS los datos.", ToastLength.Long).Show();
            }
            else
            {
                if (txtTelefono.Text.Length == 9)
                {
                    ModificaUsuarioBaseDatos();
                    Intent intent = new Intent(this, typeof(ActivityAnadirUsuario));
                    intent.PutExtra("Rol", rol);
                    intent.PutExtra("Id", idActual);
                    intent.PutExtra("Nombre", nombreActual);
                    intent.PutExtra("Profesion", profesionActual);
                    intent.PutExtra("Usuario", nickActual);
                    intent.PutExtra("Telefono", telefonoActual);
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Lo siento el numero de telefono solo debe tener 9 numeros", ToastLength.Long).Show();
                }
            }
        }
        
        private void ModificaUsuarioBaseDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            //recoge el idEmpresa que haya introducido
            idEmpresa = DevuelveIdEmpresaSeleccionadoByName(spEmpresas.SelectedItem.ToString());

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmdsql = new MySqlCommand("UPDATE Usuario SET Nombre = @nombre, Profesion = @profesion," +
                                                                         " Telefono = @telefono, Nick = @nick," +
                                                                         " Password = @password, IdEmpresa = @idempresa" +
                                                                         " WHERE Usuario.Id = @iduser", con);

                    //Actualiza y pone en el campo de la hora fin de la tarea, la hora actual del ese momento.
                    cmdsql.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmdsql.Parameters.AddWithValue("@profesion", txtProfesion.Text);
                    cmdsql.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                    cmdsql.Parameters.AddWithValue("@nick", txtUsuario.Text);
                    cmdsql.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmdsql.Parameters.AddWithValue("@idempresa", idEmpresa);
                    cmdsql.Parameters.AddWithValue("@iduser", idUsuario);
                    cmdsql.ExecuteNonQuery();
                    Toast.MakeText(ApplicationContext, "Se ha modificado el usuario \"" + nick + "\" con éxito!", ToastLength.Long).Show();
                }
            }
            catch (MySqlException ex)
            {
                Toast.MakeText(ApplicationContext, ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }
        }
    }
}

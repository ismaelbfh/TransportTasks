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
    [Activity(Label = "Registro de Usuarios")]
    public class ActivityRegistro : Activity
    {
        private EditText txtNombre, txtProfesion, txtTelefono, txtPassword, txtUsuario;
        private Button btnRegistrar;
        private ImageButton btnAtrasRegistro;
        private Spinner spEmpresas;
        private string rol, idUsuario, nombre, telefono, nick, profesion;
        private int idEmpresa;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutRegistro);

            txtNombre = FindViewById<EditText>(Resource.Id.txtNombre);
            txtProfesion = FindViewById<EditText>(Resource.Id.txtProfesion);
            txtTelefono = FindViewById<EditText>(Resource.Id.txtTelefonoRegistro);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            txtUsuario = FindViewById<EditText>(Resource.Id.txtUsuario);
            btnRegistrar = FindViewById<Button>(Resource.Id.btnRegistrar);
            btnAtrasRegistro = FindViewById<ImageButton>(Resource.Id.btnAtrasRegistro);
            spEmpresas = FindViewById<Spinner>(Resource.Id.spEmpresas);

            rol = Intent.GetStringExtra("Rol");
            idUsuario = Intent.GetStringExtra("Id");
            nombre = Intent.GetStringExtra("Nombre");
            telefono = Intent.GetStringExtra("Telefono");
            nick = Intent.GetStringExtra("Usuario");
            profesion = Intent.GetStringExtra("Profesion");

            //Establece lo que habrá dentro del spinner
            List<string> empresas = ConsultaEmpresasBaseDatos();
            
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this,
                    Android.Resource.Layout.SimpleSpinnerDropDownItem, empresas);
            
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

            spEmpresas.Adapter = dataAdapter;
            ///////////////////////////////////////////

            btnRegistrar.Click += delegate
            {
                CompruebaInsertaDatosCorrectos();
            };

            btnAtrasRegistro.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityAnadirUsuario));
                intent.PutExtra("Rol", rol);
                intent.PutExtra("Id", idUsuario);
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
                Finish();
            };
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
                                idEmpresa = Convert.ToInt32(reader.GetString(0).ToString());
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
        
        private void CompruebaInsertaDatosCorrectos()
        {
            if (txtNombre.Text.Equals("") || txtProfesion.Text.Equals("") || txtTelefono.Text.Equals("") ||
                txtPassword.Text.Equals("") || txtUsuario.Text.Equals(""))
            {
                Toast.MakeText(ApplicationContext, "Lo siento tienes que rellenar TODOS los datos.", ToastLength.Long).Show();
            }
            else
            {
                if (CompruebaUsuarioRepetido())
                {
                    if (txtTelefono.Text.Length == 9)
                    {
                        InsertaUsuarioBaseDatos();
                        InsertaClienteDefaultBaseDatos(DevuelveIdUsuarioInsertado());
                        Intent intent = new Intent(this, typeof(ActivityAnadirUsuario));
                        intent.PutExtra("Rol", rol);
                        intent.PutExtra("Id", idUsuario);
                        intent.PutExtra("Usuario", nick);
                        intent.PutExtra("Telefono", telefono);
                        intent.PutExtra("Profesion", profesion);
                        intent.PutExtra("Nombre", nombre);
                        StartActivity(intent);
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "El numero de telefono debe tener 9 numeros.", ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "El nick introducido ya existe, prueba con otro", ToastLength.Long).Show();
                }
            }
        }

        private int DevuelveIdUsuarioInsertado()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            int Id = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Id FROM Usuario ORDER BY Id DESC LIMIT 1", con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Id = reader.GetInt32(0);  //guardamos el id del ultimo usuario insertado
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
            return Id;
        }

        private bool CompruebaUsuarioRepetido()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nick FROM Usuario WHERE Nick = '" + txtUsuario.Text + "'", con))
                    {
                        //Nos creamos una lista para añadir dentro los datos a comprobar (Nick(en la posicion 0))
                        List<string> lista = new List<string>();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(reader.GetString(0).ToString());  //esto será el primer campo del select (Nick)
                            }

                            if (lista.Count() > 0)
                            { //si hay algun elemento en la lista significa que lo ha encontrado
                                if (lista[0].Equals(txtUsuario.Text)) //comprobamos que los datos que ha introducido el usuario son los mismos que los consultados en base de datos
                                {
                                    Toast.MakeText(ApplicationContext, "El Usuario ya existe en base de datos, introduzca otro nombre distinto.", ToastLength.Long).Show();
                                    return false;
                                }
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                Toast.MakeText(ApplicationContext, ex.Message, ToastLength.Long).Show();
            }

            return true;
        }

        private void InsertaUsuarioBaseDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            //recoge el idEmpresa que haya introducido
            idEmpresa = DevuelveIdEmpresaSeleccionadoByName(spEmpresas.SelectedItem.ToString());
            var rolARegistrar = rol.Equals("SuperAdmin") ? "Admin" : "Usuario";

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO Usuario(Nombre,Profesion,Telefono,Nick,Password,Rol,IdEmpresa) VALUES(@nombre,@profesion,@telefono,@nick,@password,@rol,@idempresa)", con);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@profesion", txtProfesion.Text);
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@nick", txtUsuario.Text);
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@rol", rolARegistrar.ToString());
                    cmd.Parameters.AddWithValue("@idempresa", idEmpresa);
                    cmd.ExecuteNonQuery();
                    Toast.MakeText(ApplicationContext, "Se ha dado de alta al usuario correctamente.", ToastLength.Long).Show();
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

        private void InsertaClienteDefaultBaseDatos(int idUsu)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO Cliente(Nombre,Apellidos,FechaNacimiento,Telefono,IdUsuario) VALUES(@nombre,@apellidos,@fecha,@telefono,@idusuario)", con);
                    cmd.Parameters.AddWithValue("@nombre", "Default Client");
                    cmd.Parameters.AddWithValue("@apellidos", "Client Default");
                    cmd.Parameters.AddWithValue("@fecha", new DateTime(1990, 12, 12));
                    cmd.Parameters.AddWithValue("@telefono", "666999000");
                    cmd.Parameters.AddWithValue("@idusuario", idUsu);
                    cmd.ExecuteNonQuery();
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
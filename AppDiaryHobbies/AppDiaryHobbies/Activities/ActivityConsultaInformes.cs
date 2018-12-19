using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MySql.Data.MySqlClient;
using System.Data;
using Android.Views;

namespace AppDiaryHobbies
{
    [Activity(Label = "Informes")]
    public class ActivityConsultaInformes : Activity
    {
        private ImageButton btnExportToPdf, btnAtrasInfor;
        private Button btnBuscar;
        List<string> usuarios;
        private List<DireccionTarea> mItems;
        private ListView listaInformes;
        private Spinner spUsuariosCascada, spClientesCascada;
        private string rol, nombre, nick, telefono, profesion, idUsuario;
        private int idUserSelected, idClienteSelected;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutConsultaInformes);

            btnBuscar = FindViewById<Button>(Resource.Id.btnBuscarInforme);
            btnAtrasInfor = FindViewById<ImageButton>(Resource.Id.btnAtrasInfor);
            listaInformes = FindViewById<ListView>(Resource.Id.listaInformes);
            spClientesCascada = FindViewById<Spinner>(Resource.Id.spClientesCascada);
            spUsuariosCascada = FindViewById<Spinner>(Resource.Id.spUsuariosCascada);

            rol = Intent.GetStringExtra("Rol");
            nombre = Intent.GetStringExtra("Nombre");
            nick = Intent.GetStringExtra("Usuario");
            telefono = Intent.GetStringExtra("Telefono");
            profesion = Intent.GetStringExtra("Profesion");
            idUsuario = Intent.GetStringExtra("Id");

            //Rellena el spinner de Usuarios en base al jefe y los que tiene a su cargo, que serán los que tengan Rol='Usuario'
            usuarios = ConsultaUsuariosBaseDatos();
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this,
                    Android.Resource.Layout.SimpleSpinnerDropDownItem, usuarios);
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spUsuariosCascada.Adapter = dataAdapter;

            spUsuariosCascada.ItemSelected += spUsuariosCascada_ItemSelected;


            btnBuscar.Click += delegate
            {
                if (!string.IsNullOrEmpty(spClientesCascada.SelectedItem.ToString()))
                {
                    idClienteSelected = DevuelveIdClienteSeleccionadoByName(spClientesCascada.SelectedItem.ToString());
                    //rellena grid con datos consulta
                    mItems = ConsultaDireccionesByIdClienteAndIdUsuario(idClienteSelected, idUserSelected);
                    if (mItems.Any())
                    {
                        MyListViewAdapterInformes myAdapter = new MyListViewAdapterInformes(this, mItems);
                        listaInformes.Adapter = myAdapter;
                    }
                    else
                    {
                        Toast.MakeText(ApplicationContext, "No hay resultados para esta consulta.", ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "No has seleccionado ningun cliente, seleccione un usuario para cargar los clientes.", ToastLength.Long).Show();
                }
            };

            btnAtrasInfor.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityTareas));
                intent.PutExtra("Id", idUsuario);
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
                Finish();
            };
            
        }

        private int DevuelveIdClienteSeleccionadoByName(string nombre)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            int idCliente = 0;

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Id FROM Cliente WHERE Nombre = '" + nombre + "'", con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                idCliente = Convert.ToInt32(reader.GetString(0));
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

            return idCliente;
        }

        private List<DireccionTarea> ConsultaDireccionesByIdClienteAndIdUsuario(int idCliente, int idUsuario)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            List<DireccionTarea> listadoInforme = new List<DireccionTarea>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Direccion.DireccionActual, Tarea.HoraInicio, Tarea.HoraFin  FROM Direccion " +
                                                                                                  "INNER JOIN Tarea ON Tarea.Id = Direccion.IdTarea " +
                                                                                                  "WHERE Tarea.IdUsuario = "+ idUsuario +" AND Direccion.IdCliente = "+ idCliente, con))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string direccionActual = reader.GetString(0).ToString();
                                DateTime fechaInicio = reader.GetDateTime(1);
                                DateTime? fechaFin = reader.IsDBNull(2) ? (DateTime?)null : (DateTime?)reader.GetDateTime(2);
                                listadoInforme.Add(new DireccionTarea(direccionActual, fechaInicio, fechaFin));
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
            return listadoInforme;
        }

        private void spUsuariosCascada_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //Depende que usuario escojamos el combo de clientes se rellenará con aquellos clientes que su propietario sea ese usuario seleccionado
            var nameUser = usuarios[e.Position];
            idUserSelected = DevuelveIdUsuarioSeleccionadoByName(nameUser);

            //Se rellena en base a la seleccion del combo de Usuarios
            List<string> clientes = ConsultaClientesByIdUsuario(idUserSelected);
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this,
                    Android.Resource.Layout.SimpleSpinnerDropDownItem, clientes);
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spClientesCascada.Adapter = dataAdapter;

        }

        private List<string> ConsultaClientesByIdUsuario(int idUser)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            List<string> listaClientes = new List<string>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nombre FROM Cliente WHERE IdUsuario = " + idUser, con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaClientes.Add(reader.GetString(0).ToString());
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

            return listaClientes;
        }


        private int DevuelveIdUsuarioSeleccionadoByName(string nombre)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            int idUsuario = 0;

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Id FROM Usuario WHERE Nombre = '" + nombre + "'", con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                idUsuario = Convert.ToInt32(reader.GetString(0));
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

            return idUsuario;
        }


        private List<string> ConsultaUsuariosBaseDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            List<string> listaUsuarios = new List<string>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nick FROM Usuario WHERE Rol = 'Usuario'", con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaUsuarios.Add(reader.GetString(0).ToString());
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

            return listaUsuarios;

        }

        private List<Usuario> ConsultaUsuariosBaseDatosByRol(string rol)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            List<Usuario> listaUsuarios = new List<Usuario>();
            var rolConsulta = rol.Equals("SuperAdmin") ? "Admin" : "Usuario";
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Id, IdEmpresa, Nombre, Profesion, Telefono, Nick, Password FROM Usuario WHERE Rol = '" + rolConsulta + "'", con))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = Convert.ToInt32(reader.GetString(0).ToString());
                                int idEmpresa = Convert.ToInt32(reader.GetString(1).ToString());
                                string nombre = reader.GetString(2).ToString();
                                string profesion = reader.GetString(3).ToString();
                                string telefono = reader.GetString(4).ToString();
                                string nick = reader.GetString(5).ToString();
                                string password = reader.GetString(6).ToString();

                                listaUsuarios.Add(new Usuario(id, idEmpresa, nombre, profesion, telefono, nick, password, rolConsulta));

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
            return listaUsuarios;
        }
    } 
}
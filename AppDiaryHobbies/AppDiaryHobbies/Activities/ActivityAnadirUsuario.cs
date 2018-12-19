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
    [Activity(Label = "Añadir Usuario")]
    public class ActivityAnadirUsuario : Activity
    {
        private ImageButton btnAnadirUsuario, btnBorrarUsuario, btnAtrasUsers;
        private AlertDialog dialog;

        private List<Usuario> mItems;
        private ListView listaUsuarios;
        private Usuario usuarioSeleccionado;
        private string rol, nombre, nick, telefono, profesion, idUsuario;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutAnadirUsuario);

            btnAnadirUsuario = FindViewById<ImageButton>(Resource.Id.btnAnadirUsu);
            btnBorrarUsuario = FindViewById<ImageButton>(Resource.Id.btnBorrarUsu);
            btnAtrasUsers = FindViewById<ImageButton>(Resource.Id.btnAtrasUsers);
            listaUsuarios = FindViewById<ListView>(Resource.Id.listaUsuarios);

            rol = Intent.GetStringExtra("Rol");
            nombre = Intent.GetStringExtra("Nombre");
            nick = Intent.GetStringExtra("Usuario");
            telefono = Intent.GetStringExtra("Telefono");
            profesion = Intent.GetStringExtra("Profesion");
            idUsuario = Intent.GetStringExtra("Id");

            //Rellena el grid de usuarios
            mItems = ConsultaUsuariosBaseDatosByRol(rol);
            var usuarioActual = new Usuario(Convert.ToInt32(idUsuario), nombre, profesion, telefono, nick);
            MyListViewAdapterUsuarios myAdapter = new MyListViewAdapterUsuarios(this, mItems, usuarioActual);
            listaUsuarios.Adapter = myAdapter;

            btnAnadirUsuario.Click += delegate
            {
                //Nos lleva a la pantalla de registro
                Intent intent = new Intent(this, typeof(ActivityRegistro));
                intent.PutExtra("Rol", rol.ToString());
                intent.PutExtra("Id", idUsuario);
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
                Finish();
            };

            btnAtrasUsers.Click += delegate
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


            btnBorrarUsuario.Click += delegate
            {
                //borrar con modal
                var alert = new AlertDialog.Builder(this);
                alert.SetCancelable(true);

                View customView = LayoutInflater.Inflate(Resource.Layout.LayoutModalBorrarUsuarios, null);
                alert.SetView(customView);

                var spUsuarios = (Spinner)customView.FindViewById(Resource.Id.spUsuariosBorrar);

                //Establece los items que habrá dentro del "ComboBox", que en este caso es un "Spinner", seran los usuarios a los que este cargo ese rol con el que hemos entrado
                List<Usuario> items = ConsultaUsuariosBaseDatosByRol(rol);
                MyListViewAdapterUsuarios adaptador = new MyListViewAdapterUsuarios(this, items, null, true);
                spUsuarios.Adapter = adaptador;

                if (items.Count > 0) //si viene con usuarios
                {
                    var btnCerrarModalBorrar = (Button)customView.FindViewById(Resource.Id.btnCerrarModalBorrarUsu);
                    var btnBorrarUsuarioModal = (Button)customView.FindViewById(Resource.Id.btnBorrarUsuario);

                    dialog = alert.Show();
                    dialog.Create();
                    dialog.Window.SetLayout(750, 500);

                    btnCerrarModalBorrar.Click += delegate
                    {
                        dialog.Dismiss();
                    };

                    btnBorrarUsuarioModal.Click += delegate
                    {
                        usuarioSeleccionado = spUsuarios.SelectedItem.Cast<Usuario>();
                        BorraUsuarioById(usuarioSeleccionado.Id);

                        Toast.MakeText(ApplicationContext, "Se ha borrado el usuario con nick " + usuarioSeleccionado.Nick + " y con nombre " + usuarioSeleccionado.NombreUsuario, ToastLength.Long).Show();
                        dialog.Dismiss();

                        //RECARGAMOS EL GRID/LIST PRINCIPAL
                        mItems = ConsultaUsuariosBaseDatosByRol(rol);
                        MyListViewAdapterUsuarios adapter = new MyListViewAdapterUsuarios(this, mItems, null);
                        listaUsuarios.Adapter = adapter;

                    };
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "No hay usuarios para borrar.", ToastLength.Long).Show();
                }
            };
            
        }

        private void BorraUsuarioById(int idUser)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM Usuario WHERE Id = " + idUser, con);
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
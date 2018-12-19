using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AppDiaryHobbies.Activities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace AppDiaryHobbies
{
    [Activity(Label = "Inicio")]
    public class ActivityTareas : Activity
    {
        private ImageButton btnPerfil, btnCerrarSesion, btnBorrarTarea, btnAnadirTarea, btnAnadirUser, btnAnadirEmpresa, btnMostrarInformes;
        private TextView txtBienvenido;
        private AlertDialog dialog;

        private List<Tarea> mItems;
        private ListView listaTareas;
        private Tarea tareaSeleccionada;
        private int idUsuario;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LayoutTareasInicio);

            btnAnadirEmpresa = FindViewById<ImageButton>(Resource.Id.btnAnadirEmpresa);
            btnAnadirUser = FindViewById<ImageButton>(Resource.Id.btnAnadirUser);
            btnMostrarInformes = FindViewById<ImageButton>(Resource.Id.btnMuestraInformes);


            btnPerfil = FindViewById<ImageButton>(Resource.Id.btnPerfil);
            btnBorrarTarea = FindViewById<ImageButton>(Resource.Id.btnBorrarTarea);
            btnCerrarSesion = FindViewById<ImageButton>(Resource.Id.btnCerrarSesion);
            btnAnadirTarea = FindViewById<ImageButton>(Resource.Id.btnAnadirTarea);
            txtBienvenido = FindViewById<TextView>(Resource.Id.txtBienvenido);

            listaTareas = FindViewById<ListView>(Resource.Id.listaTareas);

            var nombre = Intent.GetStringExtra("Nombre");
            var profesion = Intent.GetStringExtra("Profesion");
            var telefono = Intent.GetStringExtra("Telefono");
            var nick = Intent.GetStringExtra("Usuario");
            var id = Intent.GetStringExtra("Id");

            idUsuario = Convert.ToInt32(id);

            mItems = ConsultaTareasBaseDatosByIdUsuario(Convert.ToInt32(id));
            MyListViewAdapter myAdapter = new MyListViewAdapter(this, mItems);
            listaTareas.Adapter = myAdapter;

            if (nombre != null)
            {
                var palabrasSeparadasNombre = nombre.Split(' ');

                //PARA QUE COJA SOLO EL NOMBRE SI VIENE CON APELLIDOS
                if (palabrasSeparadasNombre.Length > 0)
                {
                    var nombreCorto = palabrasSeparadasNombre[0];
                    txtBienvenido.Text = "Bienvenido " + nombreCorto;
                }
                else
                {
                    txtBienvenido.Text = "Bienvenido " + nombre;
                }
            }

            //Muestra o no determinados botones en funcion del rol
            var isRole = DevuelveRolUsuario();

            if (isRole.Equals("SuperAdmin"))
            {
                btnMostrarInformes.Visibility = ViewStates.Invisible;
                btnMostrarInformes.SetMaxWidth(0);
                btnMostrarInformes.SetMaxHeight(0);
            }
            else if (isRole.Equals("Admin"))
            {
                btnAnadirEmpresa.Visibility = ViewStates.Invisible;
            }
            else if (isRole.Equals("Usuario"))
            {
                btnAnadirEmpresa.Visibility = ViewStates.Invisible;
                btnMostrarInformes.Visibility = ViewStates.Invisible;
                btnAnadirUser.Visibility = ViewStates.Invisible;
                btnMostrarInformes.SetMaxWidth(0);
                btnMostrarInformes.SetMaxHeight(0);
            }

            btnMostrarInformes.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityConsultaInformes));
                intent.PutExtra("Rol", isRole.ToString());
                intent.PutExtra("Id", id.ToString());
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
                Finish();
            };


            btnPerfil.Click += delegate
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetCancelable(true);

                View customView = LayoutInflater.Inflate(Resource.Layout.LayoutModalPerfil, null);
                alert.SetView(customView);
                
                var txtNombreInfo = (TextView)customView.FindViewById(Resource.Id.txtNombreInfo);
                txtNombreInfo.Text = nombre;

                var txtProfesionInfo = (TextView)customView.FindViewById(Resource.Id.txtProfesionInfo);
                txtProfesionInfo.Text = profesion;

                var txtTelefonoInfo = (TextView)customView.FindViewById(Resource.Id.txtTelefonoPerfil);
                txtTelefonoInfo.Text = telefono;

                var txtNickInfo = (TextView)customView.FindViewById(Resource.Id.txtNickInfo);
                txtNickInfo.Text = nick;

                var txtRolInfo = (TextView)customView.FindViewById(Resource.Id.txtRolInfo);
                txtRolInfo.Text = isRole;

                var txtEmpresaInfo = (TextView)customView.FindViewById(Resource.Id.txtEmpresaInfo);
                txtEmpresaInfo.Text = DevuelveEmpresaUsuario();

                var btnCerrarModal = (Button)customView.FindViewById(Resource.Id.btnCerrarModal);
                
                dialog = alert.Show();
                dialog.Create();
                dialog.Window.SetLayout(750, 500);

                btnCerrarModal.Click += delegate
                {
                    dialog.Dismiss();
                };
            };

            btnCerrarSesion.Click += delegate
            {
                Intent intent = new Intent(this, typeof(MainActivity)).SetFlags(ActivityFlags.ReorderToFront);
                StartActivity(intent);
                Finish();
                Toast.MakeText(ApplicationContext, "Se ha cerrado la sesión correctamente.", ToastLength.Long).Show();
            };

            btnBorrarTarea.Click += delegate
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetCancelable(true);

                View customView = LayoutInflater.Inflate(Resource.Layout.LayoutModalBorrar, null);
                alert.SetView(customView);

                var spTareas = (Spinner)customView.FindViewById(Resource.Id.spTareas);

                //Establece los items que habrá dentro del "ComboBox", que en este caso es un "Spinner", seran las tareas de ese usuario
                List <Tarea> items = ConsultaTareasBaseDatosByIdUsuario(Convert.ToInt32(id));
                MyListViewAdapter adaptador = new MyListViewAdapter(this, items, true);
                spTareas.Adapter = adaptador;

                if (items.Count > 0)
                {
                    var btnCerrarModalBorrar = (Button)customView.FindViewById(Resource.Id.btnCerrarModalBorrar);
                    var btnBorrarTarea = (Button)customView.FindViewById(Resource.Id.btnBorrarTarea);

                    dialog = alert.Show();
                    dialog.Create();
                    dialog.Window.SetLayout(750, 500);

                    btnCerrarModalBorrar.Click += delegate
                    {
                        dialog.Dismiss();
                    };

                    btnBorrarTarea.Click += delegate
                    {
                        tareaSeleccionada = spTareas.SelectedItem.Cast<Tarea>();
                        BorraTareaById(tareaSeleccionada.IdTarea);
                        Toast.MakeText(ApplicationContext, "Se ha borrado la tarea #" + tareaSeleccionada.contador.ToString().PadLeft(4, '0') + " con nombre " + tareaSeleccionada.NombreTarea, ToastLength.Long).Show();
                        dialog.Dismiss();

                    //RECARGAMOS EL GRID/LIST PRINCIPAL
                    mItems = ConsultaTareasBaseDatosByIdUsuario(Convert.ToInt32(id));
                        MyListViewAdapter adapter = new MyListViewAdapter(this, mItems);
                        listaTareas.Adapter = adapter;

                    };
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "No hay tareas para borrar.", ToastLength.Long).Show();
                }
            };

            btnAnadirTarea.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityNuevaTarea));
                intent.PutExtra("IdUsuario", id.ToString());
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
                Finish();
            };

            btnAnadirEmpresa.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityNuevaEmpresa));
                intent.PutExtra("Id", id.ToString());
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
            };

            btnAnadirUser.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityAnadirUsuario));
                intent.PutExtra("Rol", isRole.ToString());
                intent.PutExtra("Id", id.ToString());
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
            };
        }

        private string DevuelveEmpresaUsuario()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            string empresa = string.Empty;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Empresa.Nombre FROM Usuario INNER JOIN Empresa ON Usuario.IdEmpresa = Empresa.Id WHERE Usuario.Id = " + idUsuario, con))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                empresa = reader.GetString(0).ToString();
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
            return empresa;
        }

        private void BorraTareaById(int idTarea)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM Tarea WHERE Id = " + idTarea, con);
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

        private string DevuelveRolUsuario()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            string rol = string.Empty;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Rol FROM Usuario WHERE Id = " + idUsuario, con))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rol = reader.GetString(0).ToString();
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
            return rol;
        }

        private List<Tarea> ConsultaTareasBaseDatosByIdUsuario(int IdUsuario)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            List<Tarea> listaTareas = new List<Tarea>();
            int contador = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Id, NombreTarea, TipoTarea, Descripcion, HoraInicio, HoraFin FROM Tarea WHERE IdUsuario = " + IdUsuario, con))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int idTarea = Convert.ToInt32(reader.GetString(0).ToString());
                                string nombreTarea = reader.GetString(1).ToString();
                                string tipoTarea = reader.GetString(2).ToString();
                                string descripcionTarea = reader.GetString(3).ToString();
                                DateTime horaInicio = reader.GetDateTime(4);
                                DateTime? horaFin = reader.IsDBNull(reader.GetOrdinal("HoraFin")) ? null : (DateTime?) reader.GetDateTime(reader.GetOrdinal("HoraFin"));
                                contador = contador + 1;
                                listaTareas.Add(new Tarea(idTarea, IdUsuario, nombreTarea, tipoTarea, descripcionTarea, horaInicio, horaFin, contador));

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
            return listaTareas;
        }
    }

    public static class ObjectTypeHelper
    {
        public static T Cast<T>(this Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }
    }
}
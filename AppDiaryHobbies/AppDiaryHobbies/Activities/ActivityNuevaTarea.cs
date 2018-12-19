using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Text;
using Android.Locations;
using MySql.Data.MySqlClient;
using System.Data;

namespace AppDiaryHobbies.Activities
{
    [Activity(Label = "Añadir Nueva Tarea")]
    public class ActivityNuevaTarea : Activity, ILocationListener
    {
        private TextView txtTituloNuevaTarea, txtTituloDatosTarea, lblNombreTarea, 
                         lblTipoTarea, lblDescripcionTarea, lblDireccionActual,
                         txtAddress;
        private EditText txtNombreTarea, txtDescripcion;
        private Spinner spTipoTarea, spClientes;
        private Button btnMap, btnAgregarTarea, btnAgregarCliente;
        private Location currentLocation;
        private LocationManager locationManager;

        private double distancia;
        private string direccion, locationProvider, guardaLatitud, guardaLongitud, idUsuario, lati, longi, profesion, telefono, nombre, nick;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutNuevaTarea);
            
            //Sacamos el parametro enviado de la pantalla anterior
            idUsuario = Intent.GetStringExtra("IdUsuario");
            telefono = Intent.GetStringExtra("Telefono");
            nombre = Intent.GetStringExtra("Nombre");
            nick = Intent.GetStringExtra("Usuario");
            profesion = Intent.GetStringExtra("Profesion");
            
            //IDENTIFICAMOS NUESTROS CONTROLES
            txtTituloNuevaTarea = FindViewById<TextView>(Resource.Id.txtTituloNuevaTarea);
            txtTituloDatosTarea = FindViewById<TextView>(Resource.Id.txtTituloDatosTarea);

            lblNombreTarea = FindViewById<TextView>(Resource.Id.lblNombreTarea);
            lblTipoTarea = FindViewById<TextView>(Resource.Id.lblTipoTarea);
            lblDescripcionTarea = FindViewById<TextView>(Resource.Id.lblDescripcionTarea);

            lblDireccionActual = FindViewById<TextView>(Resource.Id.lblDireccionActual);
            txtAddress = FindViewById<TextView>(Resource.Id.txtAddress);

            txtNombreTarea = FindViewById<EditText>(Resource.Id.txtNombreTarea);
            txtDescripcion = FindViewById<EditText>(Resource.Id.txtDescripcionTarea);

            spTipoTarea = FindViewById<Spinner>(Resource.Id.spTipoTarea);
            spClientes = FindViewById<Spinner>(Resource.Id.spClientes);

            btnMap = FindViewById<Button>(Resource.Id.btnVerMapa);
            btnAgregarTarea = FindViewById<Button>(Resource.Id.btnAgregarTarea);
            btnAgregarCliente = FindViewById<Button>(Resource.Id.btnAgregarCliente);

            //Les damos un formato en HTML manual atractivo
            EstableceFormatos();

            //Establecemos los items del combo de los Tipos de Tareas
            spTipoTarea.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spTipoTarea_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.tipos_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spTipoTarea.Adapter = adapter;

            //recupera de la pantalla anterior:
            var vieneDeCliente = Intent.GetStringExtra("AgregaCliente");
            if (vieneDeCliente != null) //si viene de la pantalla de clientes rellena lo que ya estaba seleccionado
            {
                var nombreTarea = Intent.GetStringExtra("NombreTarea");
                var tipoTarea = Intent.GetStringExtra("TipoTarea");
                var descripcionTarea = Intent.GetStringExtra("DescripcionTarea");
                txtNombreTarea.Text = nombreTarea;
                txtDescripcion.Text = descripcionTarea;
                spTipoTarea.SetSelection(adapter.GetPosition(tipoTarea));
            }

            //Ponemos los clientes en un combo:
            List<string> clientes = ConsultaClientesBaseDatos(Convert.ToInt32(idUsuario));
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this,
                    Android.Resource.Layout.SimpleSpinnerDropDownItem, clientes);
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spClientes.Adapter = dataAdapter;
            ////////////////////////////////////

            //IMPLEMENTAMOS LOGICA DE LOCALIZADORES DEL ANDROID (BIOMETRICA)
            locationManager = (LocationManager)GetSystemService(LocationService);

            Criteria locationServiceCriteria = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };

            //encuentra el gps a traves de la posicion del wifi
            IList<string> acceptableLocationProviders =
                locationManager.GetProviders(locationServiceCriteria, true);

            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }

            //Si pulsamos el boton de "Ver Mapa" nos accedera a nuestro Google Maps y se vera esa direccion donde nos encontremos
            btnMap.Click += delegate
            {
                string url = "geo:" + guardaLatitud + "," + guardaLongitud;
                var geoUri = Android.Net.Uri.Parse(url);
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                StartActivity(mapIntent);
            };
            
            btnAgregarTarea.Click += delegate
            {
                InsertaTareaBaseDatos();
                InsertaDireccionByIdUsuario(Convert.ToInt32(idUsuario));
                Toast.MakeText(ApplicationContext, "Se ha insertado correctamente la tarea y la dirección.", ToastLength.Long).Show();
                Intent intent = new Intent(this, typeof(ActivityTareas));
                intent.PutExtra("Id", idUsuario);
                intent.PutExtra("Nombre", nombre);
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                StartActivity(intent);
            };

            btnAgregarCliente.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityAnadirCliente));
                intent.PutExtra("IdUser", idUsuario);
                intent.PutExtra("NombreTarea", txtNombreTarea.Text);
                intent.PutExtra("TipoTarea", spTipoTarea.SelectedItem.ToString());
                intent.PutExtra("DescripcionTarea", txtDescripcion.Text);
                StartActivity(intent);
                Finish();
            };
        }

        private List<string> ConsultaClientesBaseDatos(int idUser)
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

        private int DevuelveIdTareaByIdUser(int idUser)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            int IdTareaInsertar = 0;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Id FROM Tarea WHERE IdUsuario = " + idUser + " ORDER BY Id DESC LIMIT 1", con))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IdTareaInsertar = reader.GetInt32(0);  //guardamos el id de la ultima tarea insertada por el usuario
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
            return IdTareaInsertar;
        }

        private void InsertaDireccionByIdUsuario(int idUser)
        {
            /*Primero consultaremos la ultima tarea que ha registrado ese usuario 
            y con el id de la ultima tarea insertaremos la nueva Direccion*/

            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                
                    //Y insertamos la direccion en base al id de la ultima tarea insertada por ese usuario
                    string dis = string.Format("{0:N2}", Math.Truncate(distancia * 100) / 100);
                    int idCliente = DevuelveIdClienteSeleccionadoByName(spClientes.SelectedItem.ToString());
                    int IdTareaInsertar = DevuelveIdTareaByIdUser(Convert.ToInt32(idUsuario));
                    MySqlCommand cmdsql = new MySqlCommand("INSERT INTO Direccion(IdTarea,DireccionActual,Latitud,Longitud,Distancia,IdCliente) VALUES(@idtarea,@direccionactual,@latitud,@longitud,@distancia,@idcliente)", con);
                    cmdsql.Parameters.AddWithValue("@idtarea", IdTareaInsertar);
                    cmdsql.Parameters.AddWithValue("@direccionactual", direccion);
                    cmdsql.Parameters.AddWithValue("@latitud", lati);
                    cmdsql.Parameters.AddWithValue("@longitud", longi);
                    cmdsql.Parameters.AddWithValue("@distancia", dis + " Km.");
                    cmdsql.Parameters.AddWithValue("@idcliente", idCliente);
                    cmdsql.ExecuteNonQuery();
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

        public int DevuelveIdClienteSeleccionadoByName(string nombre)
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

        private void InsertaTareaBaseDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO Tarea(IdUsuario,NombreTarea,TipoTarea,Descripcion,HoraInicio,HoraFin) VALUES(@idusuario,@nombretarea,@tipotarea,@descripcion,@horainicio,@horafin)", con);
                    cmd.Parameters.AddWithValue("@idusuario", Convert.ToInt32(idUsuario));
                    cmd.Parameters.AddWithValue("@nombretarea", txtNombreTarea.Text);
                    cmd.Parameters.AddWithValue("@tipotarea", spTipoTarea.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@descripcion", txtDescripcion.Text);
                    cmd.Parameters.AddWithValue("@horainicio", DateTime.Now);
                    cmd.Parameters.AddWithValue("@horafin", null);
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

        //Metodos de la interfaz añadidos por nosotros:
        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);

        }

        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }

        private void spTipoTarea_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
        }

        private void EstableceFormatos()
        {
            string htmlTituloNuevaTarea = "<b><u>NUEVA TAREA</u></b>";
            string htmlTituloDatosTarea = "<b><u>Datos de la Tarea</u></b>";
            string htmlNombreTarea = "<u>Nombre Tarea: </u>";
            string htmlTipoTarea = "<u>Tipo Tarea: </u>";
            string htmlDescripcionTarea = "<u>Descripción: </u>";
            string htmlDireccion = "<u>Dirección Actual: </u>";
            txtTituloNuevaTarea.TextFormatted = Html.FromHtml(htmlTituloNuevaTarea);
            txtTituloDatosTarea.TextFormatted = Html.FromHtml(htmlTituloDatosTarea);
            lblNombreTarea.TextFormatted = Html.FromHtml(htmlNombreTarea);
            lblTipoTarea.TextFormatted = Html.FromHtml(htmlTipoTarea);
            lblDescripcionTarea.TextFormatted = Html.FromHtml(htmlDescripcionTarea);
            lblDireccionActual.TextFormatted = Html.FromHtml(htmlDireccion);
        }

        public void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;
                if (currentLocation == null)
                {
                    Toast.MakeText(ApplicationContext, "Ubicación no encontrada. error al cargar latitud y longitud.", ToastLength.Long).Show();
                }
                else
                {
                    guardaLatitud = currentLocation.Latitude.ToString();
                    guardaLongitud = currentLocation.Longitude.ToString();
                    lati = string.Format("{0:N2}", Math.Truncate(currentLocation.Latitude * 100) / 100);
                    longi = string.Format("{0:N2}", Math.Truncate(currentLocation.Longitude * 100) / 100);
                    
                    Geocoder geocoder = new Geocoder(this);

                    IList<Address> addressList = geocoder.GetFromLocation(currentLocation.Latitude, currentLocation.Longitude, 5);

                    Address address = addressList.FirstOrDefault();

                    //Coordenadas de Trafalgar (51.50, -0.13):
                    double lat1 = currentLocation.Latitude;
                    double theta = currentLocation.Longitude - (-0.13);
                    double distance = Math.Sin(Math.PI / 180.0 * (lat1))
                                      * Math.Sin(Math.PI / 180.0 * (51.50)) +
                                      Math.Cos(Math.PI / 180.0 * (lat1)) *
                                      Math.Cos(Math.PI / 180.0 * (51.50)) *
                                      Math.Cos(Math.PI / 180.0 * (theta));
                    distance = Math.Acos(distance);
                    distance = distance / Math.PI * 180.0;
                    distance = distance * 60 * 1.15;

                    distancia = distance * 1.609d; //lo pasa a kilometros
                    
                    if (address != null)
                    {
                        //Al final lo he tenido que poner asi si no, no encontraba la direccion
                        string dir = address.GetAddressLine(0) + " " + address.GetAddressLine(1) + " " + address.GetAddressLine(2);
                        txtAddress.Text = dir;
                        direccion = dir;
                    }
                    else
                    {
                        txtAddress.Text = "Direccion no encontrada";
                        direccion = "Direccion no encontrada";
                        Toast.MakeText(ApplicationContext, "Dirección no encontrada. error al cargar la dirección.", ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                txtAddress.Text = "Direccion no encontrada y " + ex.Message;
                direccion = "Direccion no encontrada";
                Toast.MakeText(ApplicationContext, ex.ToString(), ToastLength.Long).Show();
            }
        }

        public void OnProviderDisabled(string provider)
        {
            
        }

        public void OnProviderEnabled(string provider)
        {
            
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            
        }
    }
}
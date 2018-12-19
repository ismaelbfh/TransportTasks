using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using MySql.Data.MySqlClient;
using Android.Text;
using System.Data;

namespace AppDiaryHobbies.Activities
{
    [Activity(Label = "Añadir Direccion")]
    public class ActivityAnadirDireccion : Activity, ILocationListener
    {
        private Location currentLocation;
        private LocationManager locationManager;
        private string locationProvider, guardaLatitud, guardaLongitud;
        private TextView txtAddress, lblNomTar, lblTipoTar, lblDescTarea, lblDireccionAnadir;
        private Button btnVerMapa, btnAnadirDireccion, btnTerminarTarea, btnAnadirClienteVer;
        private Spinner spClientesDireccion;
        private string idTarea, idUsuario, lati, longi, direccion, nombre, telefono, profesion, nick, compruebaPantalla, nombreTarea, tipoTarea, descripcionTarea;
        private double distancia;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutAnadirDireccion);

            //Identificamos los labeles y le damos un formato asequible:
            lblNomTar = FindViewById<TextView>(Resource.Id.lblNomTarea);
            lblTipoTar = FindViewById<TextView>(Resource.Id.lblTipoTar);
            lblDescTarea = FindViewById<TextView>(Resource.Id.lblDescTarea);
            lblDireccionAnadir = FindViewById<TextView>(Resource.Id.lblDireccionAnadir);
            EstableceFormatos();

            //Inicializamos los datos de la Tarea:
            idTarea = Intent.GetStringExtra("IdTareaAnadir");
            idUsuario = Intent.GetStringExtra("IdUsuario");
            compruebaPantalla = Intent.GetStringExtra("AnadeDireccion");
            Usuario user = GetUsuarioById(Convert.ToInt32(idUsuario));
            nombre = user.NombreUsuario;
            telefono = user.Telefono;
            profesion = user.Profesion;
            nick = user.Nick;

            nombreTarea = Intent.GetStringExtra("NombreTarea");
            tipoTarea = Intent.GetStringExtra("TipoTarea");
            descripcionTarea = Intent.GetStringExtra("DescripcionTarea");

            var txNomTarea = FindViewById<TextView>(Resource.Id.txNomTarea);
            var txTipoTarea = FindViewById<TextView>(Resource.Id.txTipoTarea);
            var txDescripcionTarea = FindViewById<TextView>(Resource.Id.txDescripcionTarea);
            txNomTarea.Text = nombreTarea;
            txTipoTarea.Text = tipoTarea;
            txDescripcionTarea.Text = descripcionTarea;

            //Ahora localizamos los elementos de la direccion actual:

            txtAddress = FindViewById<TextView>(Resource.Id.txDireccionAnadir);

            btnVerMapa = FindViewById<Button>(Resource.Id.btnVerMapaAnadirDir);
            btnAnadirDireccion = FindViewById<Button>(Resource.Id.btnAnadirDireccion);
            btnTerminarTarea = FindViewById<Button>(Resource.Id.btnTerminarTarea);
            btnAnadirClienteVer = FindViewById<Button>(Resource.Id.btnAnadirClienteDir);
            spClientesDireccion = FindViewById<Spinner>(Resource.Id.spClientesDireccion);

            //Rellena combo de clientes de ese usuario:
            List<string> clientes = ConsultaClientesBaseDatos(Convert.ToInt32(idUsuario));
            ArrayAdapter<string> dataAdapter = new ArrayAdapter<string>(this,
                    Android.Resource.Layout.SimpleSpinnerDropDownItem, clientes);
            dataAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spClientesDireccion.Adapter = dataAdapter;

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
            btnVerMapa.Click += delegate
            {
                string url = "geo:" + guardaLatitud + "," + guardaLongitud;
                var geoUri = Android.Net.Uri.Parse(url);
                var mapIntent = new Intent(Intent.ActionView, geoUri);
                StartActivity(mapIntent);
            };

            btnAnadirClienteVer.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityAnadirCliente));
                intent.PutExtra("IdUser", idUsuario);
                intent.PutExtra("AnadeDireccion", compruebaPantalla);
                intent.PutExtra("NombreTarea", nombreTarea);
                intent.PutExtra("TipoTarea", tipoTarea);
                intent.PutExtra("DescripcionTarea", descripcionTarea);
                intent.PutExtra("IdTareaAnadir", idTarea);
                StartActivity(intent);
                Finish();
            };

            btnAnadirDireccion.Click += delegate
            {
                AnadirDireccionByLocations();
                Intent intent = new Intent(this, typeof(ActivityTareas));
                intent.PutExtra("Id", idUsuario);
                intent.PutExtra("Nombre", nombre);
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                StartActivity(intent);
                Toast.MakeText(ApplicationContext, "Dirección actual añadida a la tarea.", ToastLength.Long).Show();
            };

            btnTerminarTarea.Click += delegate
            {
                TerminaTareaActual();
                Intent intent = new Intent(this, typeof(ActivityTareas));
                intent.PutExtra("Id", idUsuario);
                intent.PutExtra("Nombre", nombre);
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                StartActivity(intent);
                Toast.MakeText(ApplicationContext, "La tarea ha terminado definitavemente!", ToastLength.Long).Show();
            };

        }

        private void EstableceFormatos()
        {
            string htmlNombreTarea = "<b><u>Nombre Tarea: </u></b>";
            string htmlTipoTarea = "<b><u>Tipo Tarea: </u></b>";
            string htmlDescripcionTarea = "<b><u>Descripción: </u></b>";
            string htmlDireccion = "<b><u>Dirección a añadir: </u></b>";
            lblNomTar.TextFormatted = Html.FromHtml(htmlNombreTarea);
            lblTipoTar.TextFormatted = Html.FromHtml(htmlTipoTarea);
            lblDescTarea.TextFormatted = Html.FromHtml(htmlDescripcionTarea);
            lblDireccionAnadir.TextFormatted = Html.FromHtml(htmlDireccion);
        }

        private Usuario GetUsuarioById(int id)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            Usuario usuario = new Usuario();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nombre, Profesion, Telefono, Nick FROM Usuario WHERE Id = " +  id, con))
                    {
                        //Nos creamos una lista para añadir dentro los datos a comprobar (Nick(en la posicion 0) y Password (en la posicion 1))
                        List<string> lista = new List<string>();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(reader.GetString(0).ToString());  //Nombre
                                lista.Add(reader.GetString(1).ToString()); //Profesion
                                lista.Add(reader.GetString(2).ToString()); //Telefono
                                lista.Add(reader.GetString(3).ToString()); //Nick
                            }

                            if (lista.Count() > 0)
                            { //si hay algun elemento en la lista significa que lo ha encontrado
                                usuario = new Usuario(id, lista[0], lista[1], lista[2], lista[3]);
                            }
                            else //sino es que serán incorrectos alguno de los datos
                            {
                                Toast.MakeText(ApplicationContext, "Recogida de datos del usuario ha fallado", ToastLength.Long).Show();
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
            return usuario;
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

        public void TerminaTareaActual()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                con.Open();
                MySqlCommand cmdsql = new MySqlCommand("UPDATE Tarea SET HoraFin = @fechaactual WHERE Tarea.Id = @idtarea", con);
                
                //Actualiza y pone en el campo de la hora fin de la tarea, la hora actual del ese momento.
                cmdsql.Parameters.AddWithValue("@fechaactual", DateTime.Now);
                cmdsql.Parameters.AddWithValue("@idtarea", Convert.ToInt32(idTarea));
                cmdsql.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Toast.MakeText(ApplicationContext,ex.ToString(), ToastLength.Long).Show();
            }
            finally
            {
                con.Close();
            }
        }

        public void AnadirDireccionByLocations()
        {
            int idCliente = DevuelveIdClienteSeleccionadoByName(spClientesDireccion.SelectedItem.ToString());
            string dis = string.Format("{0:N2}", Math.Truncate(distancia * 100) / 100);
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                con.Open();
                MySqlCommand cmdsql = new MySqlCommand("INSERT INTO Direccion(IdTarea,DireccionActual,Latitud,Longitud,Distancia,IdCliente) VALUES(@idtarea,@direccionactual,@latitud,@longitud,@distancia,@idcliente)", con);
                cmdsql.Parameters.AddWithValue("@idtarea", Convert.ToInt32(idTarea));
                cmdsql.Parameters.AddWithValue("@direccionactual", direccion);
                cmdsql.Parameters.AddWithValue("@latitud", lati);
                cmdsql.Parameters.AddWithValue("@longitud", longi);
                cmdsql.Parameters.AddWithValue("@distancia", dis);
                cmdsql.Parameters.AddWithValue("@idcliente", idCliente);
                cmdsql.ExecuteNonQuery();
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

        public void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;
                if (currentLocation == null)
                {
                    Toast.MakeText(ApplicationContext, "Ubicacion no encontrada en base a su latitud/longitud", ToastLength.Long).Show();
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
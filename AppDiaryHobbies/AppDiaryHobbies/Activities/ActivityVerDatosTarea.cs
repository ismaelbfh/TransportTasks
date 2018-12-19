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
using MySql.Data.MySqlClient;
using System.Data;
using Android.Text;

namespace AppDiaryHobbies.Activities
{
    [Activity(Label = "Ver datos de la Tarea")]
    public class ActivityVerDatosTarea : Activity
    {
        private string idTarea, idUsuario, nombre, telefono, profesion, nick;
        private TextView txNomTarea, txTipoTarea, txDescTarea, txHoraInicio, txHoraFin, txIdTarea,
                         lblDirecciones, lblNomT, lblTipoT, lblDescripcionT, lblHoraInicio, lblHoraFin;
        private ListView listaDirecciones;
        private List<string> elementosLista;
        private Button btnVolverAtras;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutVerDatosTarea);

            //Sacamos datos enviados desde layout anterior (su tarea a la que hayamos pulsado click)
            idTarea = Intent.GetStringExtra("IdTareaVer");
            idUsuario = Intent.GetStringExtra("IdUsuario");
            Usuario user = GetUsuarioById(Convert.ToInt32(idUsuario));
            nombre = user.NombreUsuario;
            telefono = user.Telefono;
            nick = user.Nick;
            profesion = user.Profesion;
            
            var nombreTarea = Intent.GetStringExtra("NombreTarea");
            var tipoTarea = Intent.GetStringExtra("TipoTarea");
            var descripcionTarea = Intent.GetStringExtra("DescripcionTarea");
            var horaInicioTarea = Intent.GetStringExtra("HoraInicioTarea");
            var horaFinTarea = Intent.GetStringExtra("HoraFinTarea");
            var contadorTarea = Intent.GetStringExtra("ContadorTarea");

            //Se identifican los elementos y se les da el formato
            txNomTarea = FindViewById<TextView>(Resource.Id.txNomT);
            txDescTarea = FindViewById<TextView>(Resource.Id.txDescripcionT);
            txTipoTarea = FindViewById<TextView>(Resource.Id.txTipoT);
            txHoraInicio = FindViewById<TextView>(Resource.Id.txHoraInicio);
            txHoraFin = FindViewById<TextView>(Resource.Id.txHoraFin);
            txIdTarea = FindViewById<TextView>(Resource.Id.txIdTarea);
            lblDirecciones = FindViewById<TextView>(Resource.Id.lblDirecciones);
            lblNomT = FindViewById<TextView>(Resource.Id.lblNomT);
            lblTipoT = FindViewById<TextView>(Resource.Id.lblTipoT);
            lblDescripcionT = FindViewById<TextView>(Resource.Id.lblDescripcionT);
            lblHoraInicio = FindViewById<TextView>(Resource.Id.lblHoraInicio);
            lblHoraFin = FindViewById<TextView>(Resource.Id.lblHoraFin);
            btnVolverAtras = FindViewById<Button>(Resource.Id.btnVolverAtras);
            listaDirecciones = FindViewById<ListView>(Resource.Id.listaDirecciones);

            EstableceFormatos();

            //Se rellenan con los datos asociados a esa tarea
            txNomTarea.Text = nombreTarea;
            txDescTarea.Text = descripcionTarea;
            txTipoTarea.Text = tipoTarea;
            txHoraInicio.Text = horaInicioTarea;
            txHoraFin.Text = horaFinTarea;
            txIdTarea.Text = "#T" + contadorTarea.ToString().PadLeft(4, '0');

            //Consultar los nombres de direcciones registrados en esa tarea
            elementosLista = GetNamesAddressByIdTarea(Convert.ToInt32(idTarea));

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, elementosLista);

            listaDirecciones.Adapter = adapter;

            btnVolverAtras.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityTareas));
                intent.PutExtra("Id", idUsuario);
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
            };
        }

        private void EstableceFormatos()
        {
            string htmlNombreTarea = "<b><u>Nombre Tarea: </u></b>";
            string htmlTipoTarea = "<b><u>Tipo Tarea: </u></b>";
            string htmlDescripcionTarea = "<b><u>Descripción: </u></b>";
            string htmlHoraInicio = "<b><u>Hora de inicio de la tarea: </u></b>";
            string htmlHoraFin = "<b><u>Hora de fin de la tarea: </u></b>";
            string htmlDireccionesTarea = "<b><u>Direcciones de la tarea: </u></b>";
            lblNomT.TextFormatted = Html.FromHtml(htmlNombreTarea);
            lblDescripcionT.TextFormatted = Html.FromHtml(htmlDescripcionTarea);
            lblTipoT.TextFormatted = Html.FromHtml(htmlTipoTarea);
            lblHoraInicio.TextFormatted = Html.FromHtml(htmlHoraInicio);
            lblHoraFin.TextFormatted = Html.FromHtml(htmlHoraFin);
            lblDirecciones.TextFormatted = Html.FromHtml(htmlDireccionesTarea);
        }


        public List<string> GetNamesAddressByIdTarea(int idTarea)
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            List<string> listAddress = new List<string>();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT DireccionActual FROM Direccion WHERE IdTarea = " + idTarea, con))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listAddress.Add(reader.GetString(0).ToString());
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
            return listAddress;
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
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nombre, Profesion, Telefono, Nick FROM Usuario WHERE Id = " + id, con))
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
    }
}
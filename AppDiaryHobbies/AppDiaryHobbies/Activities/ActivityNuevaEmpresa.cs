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
    [Activity(Label = "Añadir Empresa")]
    public class ActivityNuevaEmpresa : Activity
    {
        private EditText txtNombre, txtFuncion, txtCiudad, txtTelefono;
        private Spinner spSector;
        private Button btnRegistrarEmpresa;
        private ImageButton btnAtrasRegistroEmpresa;
        private string nombre, profesion, telefono, nick, id;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutNuevaEmpresa);

            txtNombre = FindViewById<EditText>(Resource.Id.txtNombreEmpresa);
            txtFuncion = FindViewById<EditText>(Resource.Id.txtFuncion);
            txtTelefono = FindViewById<EditText>(Resource.Id.txtTelefonoEmpresa);
            txtCiudad = FindViewById<EditText>(Resource.Id.txtCiudadEmpresa);
            btnRegistrarEmpresa = FindViewById<Button>(Resource.Id.btnRegistrarEmpresa);
            btnAtrasRegistroEmpresa = FindViewById<ImageButton>(Resource.Id.btnAtrasEmpresa);
            spSector = FindViewById<Spinner>(Resource.Id.spSector);

            nombre = Intent.GetStringExtra("Nombre");
            profesion = Intent.GetStringExtra("Profesion");
            telefono = Intent.GetStringExtra("Telefono");
            nick = Intent.GetStringExtra("Usuario");
            id = Intent.GetStringExtra("Id");

            //Rellena el spinner:
            spSector.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spSector_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.sectores_array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spSector.Adapter = adapter;
            ///////////////////

            btnRegistrarEmpresa.Click += delegate
            {
                CompruebaInsertaDatosCorrectos();
            };

            btnAtrasRegistroEmpresa.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityTareas));
                intent.PutExtra("Id", id.ToString());
                intent.PutExtra("Usuario", nick);
                intent.PutExtra("Telefono", telefono);
                intent.PutExtra("Profesion", profesion);
                intent.PutExtra("Nombre", nombre);
                StartActivity(intent);
            };

        }

        private void spSector_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
        }

        private void CompruebaInsertaDatosCorrectos()
        {
            if (txtNombre.Text.Equals("") || txtFuncion.Text.Equals("") || txtCiudad.Text.Equals("") || txtTelefono.Text.Equals(""))
            {
                Toast.MakeText(ApplicationContext, "Lo siento tienes que rellenar TODOS los datos.", ToastLength.Long).Show();
            }
            else
            {
                if (txtTelefono.Text.Length == 9)
                {
                    InsertaEmpresaBaseDatos();
                    Intent intent = new Intent(this, typeof(ActivityTareas));
                    intent.PutExtra("Id", id.ToString());
                    intent.PutExtra("Usuario", nick);
                    intent.PutExtra("Telefono", telefono);
                    intent.PutExtra("Profesion", profesion);
                    intent.PutExtra("Nombre", nombre);
                    StartActivity(intent);
                    Toast.MakeText(ApplicationContext, "Empresa creada con exito.", ToastLength.Long).Show();
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Lo siento el numero de telefono solo debe tener 9 numeros", ToastLength.Long).Show();
                }
            }
        }

        private void InsertaEmpresaBaseDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO Empresa(Nombre,Funcion,Sector,Ciudad,Telefono) VALUES(@nombre,@funcion,@sector,@ciudad,@telefono)", con);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@funcion", txtFuncion.Text);
                    cmd.Parameters.AddWithValue("@sector", spSector.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@ciudad", txtCiudad.Text);
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
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
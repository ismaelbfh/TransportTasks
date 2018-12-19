using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MySql.Data.MySqlClient;
using System.Data;
using AppDiaryHobbies.Activities;
using System.Globalization;
using static Android.App.DatePickerDialog;

namespace AppDiaryHobbies
{
    [Activity(Label = "Añadir Cliente")]
    public class ActivityAnadirCliente : Activity, IOnDateSetListener
    {
        private EditText txtNombre, txtApellidos, txtTelefono;
        private Button btnAnadirCliente, btnDialogFecha;
        private ImageButton btnAtrasCliente;
        private int idUsuario;
        private string nombreTarea, tipoTarea, descripcion, compruebaPantalla, idTarea;
        private const int DATE_DIALOG = 1;
        private int year, month, day;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.LayoutAgregarCliente);

            txtNombre = FindViewById<EditText>(Resource.Id.txtNombreCliente);
            txtApellidos = FindViewById<EditText>(Resource.Id.txtApellidosCliente);
            txtTelefono = FindViewById<EditText>(Resource.Id.txtTelefonoCliente);
            btnAnadirCliente = FindViewById<Button>(Resource.Id.btnAnadirCliente);
            btnAtrasCliente = FindViewById<ImageButton>(Resource.Id.btnAtrasCliente);
            btnDialogFecha = FindViewById<Button>(Resource.Id.btnDialogFecha);

            idUsuario = Convert.ToInt32(Intent.GetStringExtra("IdUser"));
            idTarea = Intent.GetStringExtra("IdTareaAnadir");
            nombreTarea = Intent.GetStringExtra("NombreTarea");
            tipoTarea = Intent.GetStringExtra("TipoTarea");
            descripcion = Intent.GetStringExtra("DescripcionTarea");
            compruebaPantalla = Intent.GetStringExtra("AnadeDireccion");

            btnDialogFecha.Click += delegate
            {
                ShowDialog(DATE_DIALOG);
            };

            btnAnadirCliente.Click += delegate
            {
                CompruebaInsertaDatosCorrectos();
            };

            btnAtrasCliente.Click += delegate
            {
                if(compruebaPantalla != null)
                {
                    Intent intent = new Intent(this, typeof(ActivityAnadirDireccion));
                    intent.PutExtra("IdUsuario", idUsuario.ToString());
                    intent.PutExtra("AnadeDireccion", compruebaPantalla);
                    intent.PutExtra("IdTareaAnadir", idTarea);
                    intent.PutExtra("NombreTarea", nombreTarea);
                    intent.PutExtra("DescripcionTarea", descripcion);
                    intent.PutExtra("TipoTarea", tipoTarea);
                    StartActivity(intent);
                    Finish();
                }
                else
                {
                    Intent intent = new Intent(this, typeof(ActivityNuevaTarea));
                    intent.PutExtra("IdUsuario", idUsuario.ToString());
                    intent.PutExtra("NombreTarea", nombreTarea);
                    intent.PutExtra("TipoTarea", tipoTarea);
                    intent.PutExtra("DescripcionTarea", descripcion);
                    intent.PutExtra("AgregaCliente", "True");
                    StartActivity(intent);
                    Finish();
                }
            };
        }

        public void OnDateSet(DatePicker view, int year, int month, int day)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            btnDialogFecha.Text = day + "/" + month + "/" + year;
        }

        protected override Dialog OnCreateDialog(int id)
        {
            switch (id)
            {
                case DATE_DIALOG:
                    {
                        return new DatePickerDialog(this, this, year, month, day);
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

        private void CompruebaInsertaDatosCorrectos()
        {
            if (txtNombre.Text.Equals("") || txtApellidos.Text.Equals("") || txtTelefono.Text.Equals(""))
            {
                Toast.MakeText(ApplicationContext, "Lo siento tienes que rellenar TODOS los datos.", ToastLength.Long).Show();
            }
            else
            {
                if (txtTelefono.Text.Length == 9)
                {
                    InsertaClienteBaseDatos();
                    if(compruebaPantalla != null)
                    {
                        Intent intent = new Intent(this, typeof(ActivityAnadirDireccion));
                        intent.PutExtra("IdUsuario", idUsuario.ToString());
                        intent.PutExtra("AnadeDireccion", compruebaPantalla);
                        intent.PutExtra("IdTareaAnadir", idTarea);
                        intent.PutExtra("NombreTarea", nombreTarea);
                        intent.PutExtra("DescripcionTarea", descripcion);
                        intent.PutExtra("TipoTarea", tipoTarea);
                        StartActivity(intent);
                        Finish();
                    }
                    else
                    {
                        Intent intent = new Intent(this, typeof(ActivityNuevaTarea));
                        intent.PutExtra("IdUsuario", idUsuario.ToString());
                        intent.PutExtra("NombreTarea", nombreTarea);
                        intent.PutExtra("TipoTarea", tipoTarea);
                        intent.PutExtra("DescripcionTarea", descripcion);
                        intent.PutExtra("AgregaCliente", "True");
                        StartActivity(intent);
                        Finish();
                    }
                    
                }
                else
                {
                    Toast.MakeText(ApplicationContext, "Lo siento el numero de telefono solo debe tener 9 numeros", ToastLength.Long).Show();
                }
            }
        }
        

        private void InsertaClienteBaseDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");

            var fechaNacimiento = new DateTime(year, month, day);
            
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO Cliente(Nombre,Apellidos,FechaNacimiento,Telefono,IdUsuario) VALUES(@nombre,@apellidos,@fechanac,@telefono,@idusuario)", con);
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text);
                    cmd.Parameters.AddWithValue("@apellidos", txtApellidos.Text);
                    cmd.Parameters.AddWithValue("@fechanac", fechaNacimiento);
                    cmd.Parameters.AddWithValue("@telefono", txtTelefono.Text);
                    cmd.Parameters.AddWithValue("@idusuario", idUsuario);
                    cmd.ExecuteNonQuery();
                    Toast.MakeText(ApplicationContext, "Se ha dado de alta al cliente correctamente.", ToastLength.Long).Show();
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

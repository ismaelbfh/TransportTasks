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
    [Activity(Label = "Logueo de Usuarios")]
    public class ActivityLogueo : Activity
    {
        private EditText txtNickLogueo, txtPasswordLogueo;
        private Button btnLoguear;
        private ImageButton btnAtrasLogueo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LayoutLogueo);
            txtPasswordLogueo = FindViewById<EditText>(Resource.Id.txtPasswordLogueo);
            txtNickLogueo = FindViewById<EditText>(Resource.Id.txtNickLoguear);

            btnLoguear = FindViewById<Button>(Resource.Id.btnLoguear);
            btnAtrasLogueo = FindViewById<ImageButton>(Resource.Id.btnAtrasLogueo);

            btnLoguear.Click += delegate
            {
                ConsultaDatos();
            };

            btnAtrasLogueo.Click += delegate
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };
             
        }

        private void ConsultaDatos()
        {
            MySqlConnection con = new MySqlConnection("Server=mysql8.db4free.net;Port=3307;database=agendaismael;User Id=ismalmysql;Password=administrador;charset=utf8");

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT Nick, Password, Nombre, Profesion, Telefono, Id FROM Usuario WHERE Nick = '" + txtNickLogueo.Text + "' AND Password = '" + txtPasswordLogueo.Text + "'", con))
                    {
                        //Nos creamos una lista para añadir dentro los datos a comprobar (Nick(en la posicion 0) y Password (en la posicion 1))
                        List<string> lista = new List<string>();

                        using (MySqlDataReader reader = cmd.ExecuteReader()) 
                        {
                            while (reader.Read())
                            {
                                lista.Add(reader.GetString(0).ToString());  //esto será el primer campo del select (Nick)
                                lista.Add(reader.GetString(1).ToString()); //este será el segundo campo del select (Password)
                                lista.Add(reader.GetString(2).ToString()); //nombre
                                lista.Add(reader.GetString(3).ToString()); //profesion
                                lista.Add(reader.GetString(4).ToString()); //telefono
                                lista.Add(reader.GetString(5).ToString()); //Id
                            }

                            if (lista.Count() > 0)
                            { //si hay algun elemento en la lista significa que lo ha encontrado
                                if (lista[0].Equals(txtNickLogueo.Text) && lista[1].Equals(txtPasswordLogueo.Text)) //comprobamos que los datos que ha introducido el usuario son los mismos que los consultados en base de datos
                                {
                                    Toast.MakeText(ApplicationContext, "Usuario y contraseña correctos, adelante!", ToastLength.Long).Show();
                                    Intent intent = new Intent(this, typeof(ActivityTareas));
                                    intent.PutExtra("Nombre",lista[2]);
                                    intent.PutExtra("Profesion", lista[3]);
                                    intent.PutExtra("Telefono", lista[4]);
                                    intent.PutExtra("Usuario", lista[0]);
                                    intent.PutExtra("Password", lista[1]);
                                    intent.PutExtra("Id", lista[5]);
                                    StartActivity(intent);
                                }
                            }
                            else //sino es que serán incorrectos alguno de los datos
                            {
                                Toast.MakeText(ApplicationContext, "Usuario y/o contraseña incorrectos!", ToastLength.Long).Show();
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
        }
    }
}
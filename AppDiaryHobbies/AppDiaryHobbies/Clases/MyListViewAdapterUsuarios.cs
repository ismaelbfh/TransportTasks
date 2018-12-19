using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using AppDiaryHobbies.Activities;

namespace AppDiaryHobbies
{
    class MyListViewAdapterUsuarios : BaseAdapter<Usuario>
    {
        public List<Usuario> mItems;
        public Context mContext;
        public bool esSpinner;
        public Usuario user;

        public MyListViewAdapterUsuarios(Context context, List<Usuario> items, Usuario usu, bool isSpinner = false)
        {
            mItems = items;
            mContext = context;
            esSpinner = isSpinner;
            user = usu;
        }

        public override Usuario this [int position]
        {
            get { return mItems[position]; }
        }

        public override int Count
        {
            get { return mItems.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;   
        }
        
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if(row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ListUsuariosDesign, null, false);
            }

            //Carga el nombre por si lleva apellidos o es muy largo:
            var nameUser = mItems[position].NombreUsuario.Split(' ');
            var nom = string.Empty;

            if(nameUser.Length == 0)
            {
                nom = mItems[position].NombreUsuario;
            }
            else if(nameUser.Length == 1)
            {
                nom = nameUser[0];
            }
            else if(nameUser.Length > 1)
            {
                nom = nameUser[0] + " " + nameUser[1];
            }

            TextView txtNickList = row.FindViewById<TextView>(Resource.Id.txtNickUsuarioList);
            txtNickList.Text = "       " + nom;

            TextView txtRolList = row.FindViewById<TextView>(Resource.Id.txtRolUsuarioList);
            txtRolList.Text = "      " + mItems[position].Rol;


            if (!esSpinner)//Para distinguir entre el spinner del borrado de tareas y el del grid de tareas
            { //si es el grid principal

                //si clickas encima te deberia llevar a la pantalla de Modificacion de ese usuario
                txtNickList.Click += delegate
                {
                    OnClickUsuario(position);
                };

                txtRolList.Click += delegate
                {
                    OnClickUsuario(position);
                };
            }
            else
            {
                txtNickList.SetBackgroundColor(Color.Blue);
                txtRolList.SetBackgroundColor(Color.Blue);
            }
            
            return row;
        }

        public void OnClickUsuario(int position)
        {
            //Llevar a la pantalla que crearemos nueva "ModificaUsuario"

            Intent intent = new Intent(mContext, typeof(ActivityModificarUsuario));
            intent.PutExtra("NombreUsu", mItems[position].NombreUsuario);
            intent.PutExtra("ProfesionUsu", mItems[position].Profesion);
            intent.PutExtra("TelefonoUsu", mItems[position].Telefono);
            intent.PutExtra("NickUsu", mItems[position].Nick);
            intent.PutExtra("PasswordUsu", mItems[position].Password);
            intent.PutExtra("RolUsu", mItems[position].Rol);
            intent.PutExtra("IdUsu", mItems[position].Id.ToString());
            intent.PutExtra("IdEmpresa", mItems[position].IdEmpresa.ToString());

            intent.PutExtra("IdUsuActual", user.Id.ToString());
            intent.PutExtra("NombreActual", user.NombreUsuario);
            intent.PutExtra("ProfesionActual", user.Profesion);
            intent.PutExtra("TelefonoActual", user.Telefono);
            intent.PutExtra("NickActual", user.Nick);
            mContext.StartActivity(intent);
        }
    }
}
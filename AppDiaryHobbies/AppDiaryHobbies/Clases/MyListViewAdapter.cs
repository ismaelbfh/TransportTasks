using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using AppDiaryHobbies.Activities;

namespace AppDiaryHobbies
{
    class MyListViewAdapter : BaseAdapter<Tarea>
    {
        public List<Tarea> mItems;
        public Context mContext;
        public bool esSpinner;

        public MyListViewAdapter(Context context, List<Tarea> items, bool isSpinner = false)
        {
            mItems = items;
            mContext = context;
            esSpinner = isSpinner;
        }

        public override Tarea this [int position]
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ListTareasDesign, null, false);
            }

            TextView txtNombre = row.FindViewById<TextView>(Resource.Id.txtNombreTareaList);
            txtNombre.Text = mItems[position].NombreTarea;

            TextView txtIdTarea = row.FindViewById<TextView>(Resource.Id.txtIdTareaList);
            txtIdTarea.Text = "  #T" + mItems[position].contador.ToString().PadLeft(4, '0');


            if (!esSpinner)//Para distinguir entre el spinner del borrado de tareas y el del grid de tareas
            { //si es el grid principal
                if (mItems[position].HoraFin == null)
                {
                    txtNombre.SetBackgroundColor(Color.Green);
                    txtIdTarea.SetBackgroundColor(Color.Green);
                }
                else
                {
                    txtNombre.SetBackgroundColor(Color.Red);
                    txtIdTarea.SetBackgroundColor(Color.Red);
                }

                //Si pulsas cualquier item, te llevara a una ventana u otra dependiendo de si la tarea esta terminada o no
                txtNombre.Click += delegate
                {
                    OnClickTarea(position);
                };

                txtIdTarea.Click += delegate
                {
                    OnClickTarea(position);
                };
            }
            else
            {
                txtNombre.SetBackgroundColor(Color.Blue);
                txtIdTarea.SetBackgroundColor(Color.Blue);
            }
            

            return row;
        }

        public void OnClickTarea(int position)
        {
            if (!mItems[position].HoraFin.HasValue) //Si la tarea está sin terminar
            {
                Intent intent = new Intent(mContext, typeof(ActivityAnadirDireccion));
                intent.PutExtra("NombreTarea", mItems[position].NombreTarea);
                intent.PutExtra("IdTareaAnadir", mItems[position].IdTarea.ToString());
                intent.PutExtra("TipoTarea", mItems[position].TipoTarea);
                intent.PutExtra("DescripcionTarea", mItems[position].DescripcionTarea);
                intent.PutExtra("IdUsuario", mItems[position].IdUsuario.ToString());
                intent.PutExtra("AnadeDireccion", "True");
                mContext.StartActivity(intent);
            }
            else
            {
                Intent intent = new Intent(mContext, typeof(ActivityVerDatosTarea));
                intent.PutExtra("NombreTarea", mItems[position].NombreTarea);
                intent.PutExtra("IdTareaVer", mItems[position].IdTarea.ToString());
                intent.PutExtra("TipoTarea", mItems[position].TipoTarea);
                intent.PutExtra("DescripcionTarea", mItems[position].DescripcionTarea);
                intent.PutExtra("HoraInicioTarea", mItems[position].HoraInicio.ToString());
                intent.PutExtra("HoraFinTarea", mItems[position].HoraFin.ToString());
                intent.PutExtra("ContadorTarea", mItems[position].contador.ToString());
                intent.PutExtra("IdUsuario", mItems[position].IdUsuario.ToString());
                mContext.StartActivity(intent);
            }
        }
    }
}
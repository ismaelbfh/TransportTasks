using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using AppDiaryHobbies;

namespace AppDiaryHobbies
{
    class MyListViewAdapterInformes : BaseAdapter<DireccionTarea>
    {
        public List<DireccionTarea> mItems;
        public Context mContext;

        public MyListViewAdapterInformes(Context context, List<DireccionTarea> items)
        {
            mItems = items;
            mContext = context;
        }

        public override DireccionTarea this [int position]
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ListInformesDesign, null, false);
            }

            TextView txtDireccionList = row.FindViewById<TextView>(Resource.Id.txtDireccionInf);
            txtDireccionList.Text = mItems[position].DireccionActual;

            TextView txtHoraInicioList = row.FindViewById<TextView>(Resource.Id.txtHoraInicioInf);
            txtHoraInicioList.Text = mItems[position].HoraInicio.ToShortDateString();

            TextView txtHoraFinList = row.FindViewById<TextView>(Resource.Id.txtHoraFinInf);
            txtHoraFinList.Text = mItems[position].HoraFin.HasValue? mItems[position].HoraFin.Value.ToString("dd/MM/yyyy") : "Sin Terminar";
           
            return row;
        }
    }
}
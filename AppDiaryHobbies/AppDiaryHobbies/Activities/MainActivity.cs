using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Text;

namespace AppDiaryHobbies
{
    [Activity(Label = "TransportTasks", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            var tituloApp = FindViewById<TextView>(Resource.Id.txtTituloApp);
            var txtLogueate = FindViewById<TextView>(Resource.Id.txtLogueate);

            //FORMATOS DE FUENTE:
            string htmlTitulo = "<u>TRANSPORTASKS</u>";
            string htmlLogueate = "<u>Loguéate</u>";
            tituloApp.TextFormatted = Html.FromHtml(htmlTitulo);
            txtLogueate.TextFormatted = Html.FromHtml(htmlLogueate);

            //Al darle click al botón de Loguearse se irá al ActivityLogueo

            txtLogueate.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ActivityLogueo));
                StartActivity(intent);
            };
        }
    }
}


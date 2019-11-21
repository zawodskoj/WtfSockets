using System.Net.Sockets;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;

namespace WtfSockets
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var runSync = FindViewById(Resource.Id.run_sync);
            var runAsync = FindViewById(Resource.Id.run_async);
            runSync.Click += (o, e) => Task.Run(() => Run(true));
            runAsync.Click += (o, e) => Task.Run(() => Run(false));
        }

        private async Task Run(bool sync)
        {
            var (host, port) = ("dev-01.perfema.com", 11883);
            
            Notify("");
            
            var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.ReceiveTimeout = 1000;
            sock.SendTimeout = 1000;

            try
            {
                var connT = sync
                    ? Task.Run(() => sock.Connect(host, port))
                    : sock.ConnectAsync(host, port);
                var rT = await Task.WhenAny(connT, Task.Delay(3000));
                
                await rT; // for exception

                Notify(rT != connT ? "Timeout" : "Ok");

                sock.Dispose();
            }
            catch
            {
                Notify("Fail");
            }
        }

        private void Notify(string text)
        {
            RunOnUiThread(() => FindViewById<TextView>(Resource.Id.text).Text = text);
        }
    }
}
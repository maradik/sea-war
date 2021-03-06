﻿using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;

namespace SeaWar.Android
{
    [Activity(Label = "SeaWar", Theme = "@style/SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Landscape, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            Task.Factory.StartNew(() => Startup());
        }

        public override void OnBackPressed()
        {
        }

        async void Startup()
        {
            await Task.Delay(2000);
            StartActivity(new Intent(Application.ApplicationContext, typeof(MainActivity)));
        }
    }
}
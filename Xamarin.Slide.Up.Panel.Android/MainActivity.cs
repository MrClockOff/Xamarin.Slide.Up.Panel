using System;
using Android.Content.PM;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;
using Xamarin.Slide.Up.Panel.Android.Controls;

namespace Xamarin.Slide.Up.Panel.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            MaterialButton showPanelButton = FindViewById<MaterialButton>(Resource.Id.ShowPanelButton);
            showPanelButton.Click += ShowPanelButton_OnClick;

            var slideUpPanel = FindViewById<SlideUpPanelLayout>(Resource.Id.SlideUpPanelLinearLayout);
            MaterialButton helloButton = slideUpPanel.CompactView.FindViewById<MaterialButton>(Resource.Id.HelloButton);
            helloButton.Click += HelloButton_OnClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void ShowPanelButton_OnClick(object sender, EventArgs eventArgs)
        {
            FindViewById<SlideUpPanelLayout>(Resource.Id.SlideUpPanelLinearLayout).PresentPanel(SlideUpPanelPresentation.Default);
        }

        private void HelloButton_OnClick(object sender, EventArgs e)
        {
            FindViewById<SlideUpPanelLayout>(Resource.Id.SlideUpPanelLinearLayout).PresentPanel(SlideUpPanelPresentation.Default);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

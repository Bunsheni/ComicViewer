using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading;
using FFImageLoading.Cache;

namespace ComicViewer.Droid
{
    [Activity(Label = "ComicViewer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            var config = new FFImageLoading.Config.Configuration()
            {
                VerboseLogging = false,
                VerbosePerformanceLogging = false,
                VerboseMemoryCacheLogging = false,
                VerboseLoadingCancelledLogging = false,
                FadeAnimationForCachedImages = false,
                MaxMemoryCacheSize = 100000000,
                FadeAnimationDuration = 500
                //Logger = new CustomLogger(),
            };
            ImageService.Instance.Initialize(config);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        protected override async void JavaFinalize()
        {
            await ImageService.Instance.InvalidateCacheAsync(CacheType.All);
            base.JavaFinalize();
        }
    }
}
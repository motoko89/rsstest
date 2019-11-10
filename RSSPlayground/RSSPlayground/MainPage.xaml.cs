using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RSSPlayground
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private static int imgCount = 1;
        public static List<string> StatTexts = new List<string>();

        private double width;
        private double height;
        private double density;

        private string manufacturer;
        private string model;
        private string platform;
        private string idiom;
        private string deviceType;
        private string osVersion;

        public MainPage()
        {
            InitializeComponent();
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            width = mainDisplayInfo.Width;
            height = mainDisplayInfo.Height;
            density = mainDisplayInfo.Density;
            manufacturer = DeviceInfo.Manufacturer;
            model = DeviceInfo.Model;
            platform = Device.RuntimePlatform;
            idiom = DeviceInfo.Idiom.ToString();
            deviceType = DeviceInfo.DeviceType.ToString();
            osVersion = DeviceInfo.VersionString;
            DeviceInfoLabel.Text = "Screensize: W" + width + " x H" + height + ". Density " + density + Environment.NewLine + Environment.NewLine +
                        "Device information: " + manufacturer + " " + model + ". " + platform + " " + idiom + " " + deviceType + ". OS version " + osVersion;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Stats.Text = string.Join(Environment.NewLine, StatTexts);
        }

        private void TakePhoto_OnClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () => await TakePhotoUI());
        }

        private async Task TakePhotoUI()
        {
            if (await EnsurePermissions())
            {
                MediaFile file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        MaxWidthHeight = 1200,
                        SaveToAlbum = false,
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        CompressionQuality = 100,
                        AllowCropping = false
                    });
                if (file != null)
                {
                    string newLine = imgCount + " Taken photo size " + (file.GetStream()?.Length.ToString() ?? "-1");
                    imgCount++;
                    StatTexts.Add(newLine);
                    Stats.Text += Environment.NewLine + newLine;
                    Navigation.PushAsync(new PhotoEditPage(file));
                }
            }
            else
            {
                await DisplayAlert("PermissionDenied", "InsufficientPermissions", "OK");
            }
        }

        private async Task<bool> EnsurePermissions()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var photoStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (cameraStatus != PermissionStatus.Granted ||
                storageStatus != PermissionStatus.Granted ||
                photoStatus != PermissionStatus.Granted)
            {
                Debug.WriteLine("Request permissions");
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] {
                    Permission.Camera,
                    Permission.Storage,
                    Permission.Photos
                });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
                photoStatus = results[Permission.Photos];
            }

            Debug.WriteLine("Camera status {0}. Storage status {1}. Photo status {2}", cameraStatus, storageStatus, photoStatus);
            return cameraStatus == PermissionStatus.Granted &&
                   storageStatus == PermissionStatus.Granted &&
                   photoStatus == PermissionStatus.Granted;
        }
    }
}

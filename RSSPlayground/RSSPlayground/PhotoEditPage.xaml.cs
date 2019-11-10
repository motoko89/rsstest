using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Plugin.Media.Abstractions;
using Syncfusion.SfImageEditor.XForms;
using Xamarin.Forms;

namespace RSSPlayground
{
    public partial class PhotoEditPage : ContentPage
    {
        private readonly MediaFile photo;
        public PhotoEditPage(MediaFile photo)
        {
            InitializeComponent();
            this.photo = photo;
            InitPhotoEditor(photo.GetStream());
        }

        private void InitPhotoEditor(Stream stream)
        {
            IPEditor.Source = ImageSource.FromStream(() => stream);
        }

        private void Handle_ImageSaving(object sender, ImageSavingEventArgs args)
        {
            Debug.WriteLine(DateTime.UtcNow + " Image saving");
            args.Cancel = true;
            var stream = args.Stream;
            MainPage.StatTexts.Add("SfImageEditor stream size " + stream?.Length.ToString());

            Navigation.PopAsync();
            Debug.WriteLine(DateTime.UtcNow + " Image saving done");
        }

        private void Handle_ImageSaved(object sender, ImageSavedEventArgs args)
        {
            string savedLocation = args.Location; // You can get the saved image location with the help of this argument
            Debug.WriteLine(DateTime.UtcNow + " args.Location = " + args.Location);
            try
            {
                File.Delete(savedLocation);
            }
            catch (Exception ex)
            {
            }
        }

        private void SavePhoto_OnClicked(object sender, EventArgs e)
        {
            Debug.WriteLine(DateTime.UtcNow + " SavePhoto_OnClicked");
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    Debug.WriteLine(DateTime.UtcNow + " Save thread");
                    IPEditor.Save(".jpg");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetType() + ":" + ex.Message + ":" + ex.StackTrace);
                }
            });
            Debug.WriteLine(DateTime.UtcNow + " SavePhoto_OnClicked ends");
        }
    }
}

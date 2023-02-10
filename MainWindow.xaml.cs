using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;


namespace Youtubedownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public string FolderPath { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            progress.Visibility = Visibility.Collapsed;
        }

        private void SelectFolder_Click(object sender, MouseButtonEventArgs e)
        {
            //Code to execute on click

            FileDialog dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                ValidateNames = false,
                FileName = "selected folder"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                FolderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                filePathTextBox.Text = FolderPath;
            }
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            progress.Value = 0;
            SetProgressBar();

            var youtube = new YoutubeExplode.YoutubeClient();

            var video = await youtube.Videos.GetAsync(link.Text);

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link.Text);

            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            string outputPath = $@"{FolderPath}\{video.Title.Replace(" ", "-").Replace("|", "-") ?? $"yt-downloader-song-{DateTime.Now.ToString().Replace("/", "-")}"}.{(rbMP3.IsChecked.Value ? "mp3" : "mp4")}";

            IProgress<double> ProgressPercentage = new Progress<double>(p =>
            {
                progress.Value = p * 100;

                if (progress.IsLoaded)
                    progress.Visibility = Visibility.Collapsed;
            });

            await youtube.Videos.Streams.DownloadAsync(streamInfo, @$"{outputPath}", ProgressPercentage);

        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progress.Dispatcher.Invoke(() => progress.Value = e.ProgressPercentage);
        }

        private void SetProgressBar()
        {
            progress.Visibility = Visibility.Visible;
            progress.IsIndeterminate = false;
        }
    }
}

//-----------------------------------------------------------------------
// <copyright file="MainPage.xaml.cs" company="AndyCnzh">
//     Copyright (c) 2017 by AndyCnzh.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ShakeThenSong
{
    using System;
    using ShakeThenSong.DataModels;
    using ShakeThenSong.Services;
    using Windows.Devices.Sensors;
    using Windows.Foundation;
    using Windows.Media.Playback;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// This is main page for this application
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Sensor and dispatcher variables 
        /// </summary>
        private Accelerometer accelerometer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            this.accelerometer = Accelerometer.GetDefault();

            if (this.accelerometer != null)
            {
                txtMessage.Text = "Good. Accelerometer support.";

                uint minReportInterval = this.accelerometer.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                this.accelerometer.ReportInterval = reportInterval;

                this.accelerometer.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(this.ReadingChanged);
            }
            else
            {
                txtMessage.Text = "No Accelerometer support.";
            }
        }

        /// <summary>
        /// MediaPlayer singleton instance
        /// </summary>
        private MediaPlayer Player => PlaybackService.Instance.Player;

        /// <summary>
        /// Gets or sets Playback list
        /// </summary>
        private MediaPlaybackList PlaybackList
        {
            get { return this.Player.Source as MediaPlaybackList; }
            set { this.Player.Source = value; }
        }

        /// <summary>
        /// Gets or sets Media list
        /// </summary>
        private MediaList MediaList
        {
            get { return PlaybackService.Instance.CurrentPlaylist; }
            set { PlaybackService.Instance.CurrentPlaylist = value; }
        }

        /// <summary>
        /// Page load
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event args</param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load the playlist data model if needed
            if (MediaList == null)
            {
                // Create the playlist data model
                MediaList = new MediaList();
                await MediaList.LoadFromMusicLibrary();
            }

            // Create a new playback list matching the data model if one does not exist
            if (this.PlaybackList == null)
            {
                this.PlaybackList = MediaList.ToPlaybackList();
            }
        }

        /// <summary>
        /// Play button test
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event args</param>
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            this.Player.Play();
        }

        /// <summary>
        /// Accelerometer test
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="args">Event args</param>
        private async void ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    AccelerometerReading reading = args.Reading;
                    txtXAxis.Text = string.Format("{0,5:0.00}", reading.AccelerationX);
                    txtYAxis.Text = string.Format("{0,5:0.00}", reading.AccelerationY);
                    txtZAxis.Text = string.Format("{0,5:0.00}", reading.AccelerationZ);
                });
        }
    }
}

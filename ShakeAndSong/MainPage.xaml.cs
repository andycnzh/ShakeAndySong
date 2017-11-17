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
            if (PlaybackList.Items.Count > 0)
            {
                this.Player.Play();
                txtMessage.Text = "Enjoy your music.";
            }
            else
            {
                txtMessage.Text = "Sorry, there is no music in your music library.";
            }
        }

        /// <summary>
        /// Play songs if MediaPlay is not running
        /// </summary>
        private void PlaySongs()
        {
            if (PlaybackList.Items.Count > 0)
            {
                if (!this.PlayerStateIsRunning())
                {
                    this.Player.Play();
                    txtMessage.Text = "Enjoy your music.";
                }
            }
            else
            {
                txtMessage.Text = "Sorry, there is no music in your music library.";
            }
        }

        /// <summary>
        /// Check the MediaPlay state
        /// </summary>
        /// <returns>Return a value indicate the player is running or not</returns>
        private bool PlayerStateIsRunning()
        {
            if (PlaybackList.Items.Count > 0)
            {
                if (this.Player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                {
                    txtMessage.Text = "Enjoy your music.";
                    return true;
                }
            }

            return false;
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
                    ////txtXAxis.Text = string.Format("{0,5:0.00}", reading.AccelerationX);
                    ////txtYAxis.Text = string.Format("{0,5:0.00}", reading.AccelerationY);
                    ////txtZAxis.Text = string.Format("{0,5:0.00}", reading.AccelerationZ);

                    double x1 = 0;
                    double x2 = 0;
                    x1 = reading.AccelerationX;
                    x2 = Math.Abs(x2) + Math.Abs(x1);

                    if (x2 > 1)
                    {
                        txtMessage.Text = "Yes, will play music soon.";
                        PlaySongs();
                    }
                });
        }
    }
}

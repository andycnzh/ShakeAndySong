//-----------------------------------------------------------------------
// <copyright file="PlaybackService.cs" company="AndyCnzh">
//     Copyright (c) 2017 by AndyCnzh.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ShakeThenSong.Services
{
    using DataModels;
    using Windows.Media.Playback;

    /// <summary>
    /// The central authority on playback in the application
    /// providing access to the player and active playlist.
    /// </summary>
    public class PlaybackService
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static PlaybackService instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaybackService"/> class
        /// </summary>
        public PlaybackService()
        {
            // Create the player instance
            this.Player = new MediaPlayer();
            this.Player.AutoPlay = false;
        }

        /// <summary>
        /// Gets Singleton instance
        /// </summary>
        public static PlaybackService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PlaybackService();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets MediaPlayer
        /// This application only requires a single shared MediaPlayer
        /// that all pages have access to. The instance could have 
        /// also been stored in Application.Resources or in an 
        /// application defined data model.
        /// </summary>
        public MediaPlayer Player { get; private set; }

        /// <summary>
        /// Gets or sets Playback list
        /// The data model of the active playlist. An application might
        /// have a database of items representing a user's media library,
        /// but here we use a simple list loaded from a JSON asset.
        /// </summary>
        public MediaList CurrentPlaylist { get; set; }
    }
}

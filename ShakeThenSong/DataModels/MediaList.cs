//-----------------------------------------------------------------------
// <copyright file="MediaList.cs" company="AndyCnzh">
//     Copyright (c) 2017 by AndyCnzh.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ShakeThenSong.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Media.Core;
    using Windows.Media.Playback;
    using Windows.Storage;
    using Windows.Storage.Search;

    /// <summary>
    /// MediaList contains music list
    /// </summary>
    public class MediaList : List<MediaPlaybackItem>
    {
        /// <summary>
        /// Load music list from music library
        /// </summary>
        /// <returns>A void task</returns>
        public async Task LoadFromMusicLibrary()
        {
            this.Clear();

            QueryOptions queryOption = new QueryOptions(CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".mp4", ".wma" });
            queryOption.FolderDepth = FolderDepth.Deep;

            Queue<IStorageFolder> folder = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions(queryOption).GetFilesAsync();

            foreach (var file in files)
            {
                var source = MediaSource.CreateFromStorageFile(file);
                var playbackItem = new MediaPlaybackItem(source);

                var displayProperties = playbackItem.GetDisplayProperties();
                displayProperties.Type = Windows.Media.MediaPlaybackType.Music;
                displayProperties.MusicProperties.Title = file.Name;

                playbackItem.ApplyDisplayProperties(displayProperties);

                this.Add(playbackItem);
            }
        }

        /// <summary>
        /// Convert MediaList to MediaPlaybackList
        /// </summary>
        /// <returns>The MediaPlaybackList</returns>
        public MediaPlaybackList ToPlaybackList()
        {
            var playbackList = new MediaPlaybackList();

            // Make a new list and enable looping
            playbackList.AutoRepeatEnabled = true;

            // Add playback items to the list
            foreach (var mediaItem in this)
            {
                playbackList.Items.Add(mediaItem);
            }

            return playbackList;
        }
    }
}

//
// Copyright Fela Ameghino 2015-2024
//
// Distributed under the GNU General Public License v3.0. (See accompanying
// file LICENSE or copy at https://www.gnu.org/licenses/gpl-3.0.txt)
//
using System;
using System.Collections.Generic;
using Telegram.Controls;
using Telegram.Services;
using Telegram.Td.Api;

namespace Telegram.ViewModels.Gallery
{
    // TODO: reactor the whole GalleryMedia to just have two classes with different constructors
    // GalleryMedia
    //      |------- GalleryPhoto
    //      |
    // GalleryVideo
    public abstract class GalleryMedia
    {
        protected readonly IClientService _clientService;

        public GalleryMedia(IClientService clientService)
        {
            _clientService = clientService;
        }

        public IClientService ClientService => _clientService;

        public RotationAngle RotationAngle { get; set; }

        public abstract File GetFile();

        public abstract File GetThumbnail();

        public virtual bool IsHls()
        {
            return false;
        }

        public virtual IList<AlternativeVideo> AlternativeVideos => Array.Empty<AlternativeVideo>();

        public virtual object Constraint { get; private set; }

        public virtual object From { get; private set; }

        public virtual FormattedText Caption { get; private set; }

        public virtual int Date { get; private set; }

        public virtual int Duration { get; private set; }

        public bool IsPhoto => !IsVideo;

        public virtual bool IsVideo { get; private set; }
        public virtual bool IsStreamable { get; private set; } = true;
        public virtual bool IsLoop { get; private set; }
        public virtual bool IsVideoNote { get; private set; }

        public virtual bool HasStickers { get; private set; }

        public virtual bool CanShare { get; private set; }
        public virtual bool CanView { get; private set; }

        public virtual bool CanSave { get; private set; }
        public virtual bool CanCopy { get; private set; }

        public virtual bool IsProtected { get; private set; } = false;

        public virtual bool IsPublic { get; protected set; }
        public virtual bool IsPersonal { get; protected set; }

        public virtual InputMessageContent ToInput()
        {
            return null;
        }
    }
}

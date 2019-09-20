﻿using System;
using System.Collections.Generic;
using ImageGallery.API.Entities;

namespace ImageGallery.API.Services
{
    public interface IGalleryRepository
    {
        IEnumerable<Image> GetImages();
        bool IsImageOwner(Guid id, string ownerId);
        Image GetImage(Guid id);
        bool ImageExists(Guid id);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(Image image);
        bool Save();
    }
}

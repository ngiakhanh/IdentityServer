﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageGallery.Client.Services;
using ImageGallery.Client.ViewModels;
using ImageGallery.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace ImageGallery.Client.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;
        private readonly string ApiBaseUrl = "https://localhost:44390/"; 

        public GalleryController(IImageGalleryHttpClient imageGalleryHttpClient)
        {
            _imageGalleryHttpClient = imageGalleryHttpClient;
        }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient(); 

            var response = await httpClient.GetAsync($"{ApiBaseUrl}api/images").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var galleryIndexViewModel = new GalleryIndexViewModel(
                    JsonConvert.DeserializeObject<IList<Image>>(imagesAsString).ToList());

                return View(galleryIndexViewModel);
            }          

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        public async Task<IActionResult> EditImage(Guid id)
        {
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.GetAsync($"{ApiBaseUrl}api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var imageAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedImage = JsonConvert.DeserializeObject<Image>(imageAsString);

                var editImageViewModel = new EditImageViewModel
                {
                    Id = deserializedImage.Id,
                    Title = deserializedImage.Title
                };
                
                return View(editImageViewModel);
            }
           
            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForUpdate instance
            var imageForUpdate = new ImageForUpdate
                { Title = editImageViewModel.Title };

            // serialize it
            var serializedImageForUpdate = JsonConvert.SerializeObject(imageForUpdate);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PutAsync(
                $"{ApiBaseUrl}api/images/{editImageViewModel.Id}",
                new StringContent(serializedImageForUpdate, Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);                        

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
          
            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        public async Task<IActionResult> DeleteImage(Guid id)
        {
            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.DeleteAsync($"{ApiBaseUrl}api/images/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
       
            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }
        
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImage(AddImageViewModel addImageViewModel)
        {   
            if (!ModelState.IsValid)
            {
                return View();
            }

            // create an ImageForCreation instance
            var imageForCreation = new ImageForCreation { Title = addImageViewModel.Title };

            // take the first (only) file in the Files list
            var imageFile = addImageViewModel.Files.First();

            if (imageFile.Length > 0)
            {
                using (var fileStream = imageFile.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    imageForCreation.Bytes = ms.ToArray();                     
                }
            }
            
            // serialize it
            var serializedImageForCreation = JsonConvert.SerializeObject(imageForCreation);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PostAsync(
                $"{ApiBaseUrl}api/images",
                new StringContent(serializedImageForCreation, Encoding.Unicode, "application/json"))
                .ConfigureAwait(false); 

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        public async Task WriteOutIdentityInformation()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            Debug.WriteLine("Identity token: " + identityToken);

            foreach (var claim in User.Claims)
            {
                Debug.WriteLine(claim.Type + ": " + claim.Value);
            }
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
        }
    }
}

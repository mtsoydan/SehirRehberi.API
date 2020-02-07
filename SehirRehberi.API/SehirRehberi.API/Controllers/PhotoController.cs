using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SehirRehberi.API.Dtos;
using SehirRehberi.API.Helpers;
using SehirRehberi.API.Models;

namespace SehirRehberi.API.Data
{
    [Route("api/cities/{cityID}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private IAppRepository _appRepository;
        private IMapper _mapper;
        private IOptions<CloudinarySettings> _cloudinarConfig;


        private Cloudinary _cloudinary;
        public PhotoController(IAppRepository appRepository, IMapper mapper , IOptions<CloudinarySettings> cloudinarConfig)
        {
            _appRepository = appRepository;
            _mapper = mapper;
            _cloudinarConfig = cloudinarConfig;

            //Cloudinary hesabını aktif edelim cloudinary sınıfından türedi
            //account bilgilerini sisteme geçelim
            Account account = new Account(_cloudinarConfig.Value.CloudName,
                _cloudinarConfig.Value.ApiKey,
                _cloudinarConfig.Value.ApiSecret);

            //Cloudinary i bağlıyoruz
            _cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        public ActionResult AddPhotoForCity(int CityID,[FromBody]PhotoForCreationDto photoForCreationDto)
        {
            var city = _appRepository.GetCityById(CityID);
            if (city == null)
            {
                return BadRequest("could not find the city");
            }
            //Sistemdeki token girişli kişinin verisini elde ediyoruz

            var CurrentUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (CurrentUserID != city.UserID.ToString())
            {
                return Unauthorized();

            }
            //Dosya bilgimizi elde ettik
            var file = photoForCreationDto.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length>0)
            {
                using (var stream = file.OpenReadStream())
                {
                    //dosyanın parametrelerini giriyourz
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };
                    //Upload gerçekleşti
                    uploadResult = _cloudinary.Upload(uploadParams);

                }
                //Dönen verileri alıp veri tabanına kaydetmemiz lazım
                photoForCreationDto.Url = uploadResult.Uri.ToString();
                photoForCreationDto.PublicID = uploadResult.PublicId;

                //fotoğraf ekleyeceğiz ama map etmemiz gerek
                var photo = _mapper.Map<Photo>(photoForCreationDto);
                photo.City = city;
                //
                if (!city.Photos.Any(p=> p.IsMain))
                {
                    photo.IsMain = true;
                }
                city.Photos.Add(photo);
                if (_appRepository.SaveAll())
                {
                    var photoToReturn = _mapper.Map<PhotosForReturnDto>(photo);
                    return CreatedAtRoute("GetPhoto", new { id = photo.ID }, photoToReturn);
                }
                

            }
            return BadRequest("could not add the photo");
        }


        [HttpGet("{id}",Name ="GetPhoto")]
        public ActionResult GetPhoto(int PhotoID)
        {
            var photoFromDb = _appRepository.GetPhoto(PhotoID);
            var photo = _mapper.Map<PhotosForReturnDto>(photoFromDb);
            return Ok(photo);
            
        }
    }
}
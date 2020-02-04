using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SehirRehberi.API.Data;
using SehirRehberi.API.Dtos;
using SehirRehberi.API.Models;

namespace SehirRehberi.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private IAppRepository _appRepository;
        private IMapper _mapper;
        public CitiesController(IAppRepository appRepository, IMapper mapper)
        {
            _mapper = mapper;
            _appRepository = appRepository;
        }
        [HttpGet("GetCities")]
        public ActionResult GetCities()
        {
            //var cities = _appRepository.GetCities().Select(c => new CityForListDto
            //{
            //    //mantık bu şekilde lakin otomatik kütüphanesi var 
            //    CityID = c.CityID,
            //    Description = c.Description,
            //    Name = c.Name,
            //    PhotoUrl = c.Photos.FirstOrDefault(p => p.IsMain == true).Url
            //});

            var cities = _appRepository.GetCities();
            var citiesToReturn = _mapper.Map<List<CityForListDto>>(cities);
            return Ok(citiesToReturn);
        }
        [HttpPost]
        [Route("add")]
        public ActionResult Add([FromBody] City city)
        {
            _appRepository.Add(city);
            _appRepository.SaveAll();
            return Ok(city);
        }

        [HttpGet]
        [Route("detail/{id:int}")]
        public ActionResult GetCities(int id)
        {
            //var cities = _appRepository.GetCities().Select(c => new CityForListDto
            //{
            //    //mantık bu şekilde lakin otomatik kütüphanesi var 
            //    CityID = c.CityID,
            //    Description = c.Description,
            //    Name = c.Name,
            //    PhotoUrl = c.Photos.FirstOrDefault(p => p.IsMain == true).Url
            //});

            var city = _appRepository.GetCityById(id);
            var cityToReturn = _mapper.Map<CityForDetailDto>(city);
            return Ok(cityToReturn);
        }



    }
}
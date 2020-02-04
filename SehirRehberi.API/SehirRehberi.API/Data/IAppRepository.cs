using SehirRehberi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SehirRehberi.API.Data
{
   public interface IAppRepository
    {
        void Add<TEntity>(TEntity entity) where TEntity:class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;

        bool SaveAll();

        List<City> GetCities();
        List<Photo> GetPhotosByCity(int id);
        City GetCityById(int cityID);
        Photo GetPhoto(int id);


    }
}

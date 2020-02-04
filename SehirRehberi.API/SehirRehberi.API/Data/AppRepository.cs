using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;
using SehirRehberi.API.Models;

namespace SehirRehberi.API.Data
{
    public class AppRepository : IAppRepository
    {
        private DataContext _DataContext;
        public AppRepository(DataContext dataContext)
        {
            _DataContext = dataContext;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            var AddedEntry = _DataContext.Entry(entity);
            AddedEntry.State =EntityState.Added;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            var deletedEntry = _DataContext.Entry(entity);
            deletedEntry.State = EntityState.Deleted;
        }

        public List<City> GetCities()
        {
            var cities = _DataContext.Cities.Include(p => p.Photos).ToList();
            return cities;
        }

        public City GetCityById(int cityID)
        {
            var city = _DataContext.Cities.Include(p => p.Photos).FirstOrDefault(c => c.ID == cityID);
            return city;
        }

        public Photo GetPhoto(int id)
        {
            var photo = _DataContext.Photos.FirstOrDefault(p => p.ID == id);
            return photo;
        }

        public List<Photo> GetPhotosByCity(int id)
        {
            var photos = _DataContext.Photos.Where(p => p.CityID == id).ToList();
            return photos;
        }

        public bool SaveAll()
        {
            return _DataContext.SaveChanges() > 0;
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            var updatedEntry = _DataContext.Entry(entity);
            updatedEntry.State = EntityState.Modified;
            _DataContext.SaveChanges();
        }
    }
}

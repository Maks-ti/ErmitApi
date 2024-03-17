

using AutoMapper;
using ErmitApi.DAL;
using ErmitApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ErmitApi.BLL;

public class LocationService : ServiceBase<DAL.Models.Location, int>
{
    public LocationService(IServiceProvider serviceProvider, ILogger<LocationService> logger)
    : base(serviceProvider, logger)
    {
        this.Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;

            #region Base To ViewModel
            cfg.CreateMap<DAL.Models.Location, Location>();
            cfg.CreateMap<DAL.Models.Showplace, Showplace>();
            #endregion

            #region ViewModel To Base 
            cfg.CreateMap<LocationCreateModel, DAL.Models.Location>()
                .ForMember(dest => dest.PictureData, opt => opt.MapFrom(src => ConvertToBytes(src.PictureFile)))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => GetFileExtension(src.PictureFile)));

            cfg.CreateMap<LocationUpdateModel, DAL.Models.Location>()
                .ForMember(dest => dest.PictureData, opt => opt.MapFrom(src => ConvertToBytes(src.PictureFile)))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => GetFileExtension(src.PictureFile)));
            #endregion
        }).CreateMapper();
    }

    public async Task<List<Location>> GetAllAsync()
    {
        await using var context = GetContext<DataBaseContext>();

        var baseResult = await context.Locations
            .Include(l => l.Showplaces).ToListAsync();

        return Mapper.Map<List<Location>>(baseResult);
    }

    public async Task<Location> GetByStandIdAsync(int standId)
    {
        await using var context = GetContext<DataBaseContext>();

        var query = from stand in context.Stands.Where(stand => stand.Id == standId)
                    join location in context.Locations on stand.LocationId equals location.Id
                    select location;

        var loc = await query.Include(l => l.Showplaces).FirstOrDefaultAsync();

        return Mapper.Map<Location>(loc);
    }

    public async Task<Location> GetByIdAsync(int id)
    {
        await using var context = GetContext<DataBaseContext>();

        var baseResult = await context.Locations.Where(l => l.Id == id)
            .Include(l => l.Showplaces).FirstOrDefaultAsync();
        
        return Mapper.Map<Location>(baseResult);
    }

    public async Task<Location> CreateAsync(LocationCreateModel model)
    {
        var achievement = Mapper.Map<DAL.Models.Location>(model);

        var baseResult = await base.AddAsync(achievement);

        return Mapper.Map<Location>(baseResult);
    }

    public async Task<Location> UpdateAsync(LocationUpdateModel model)
    {
        var achievement = Mapper.Map<DAL.Models.Location>(model);

        var baseResult = await base.UpdateAsync(achievement);

        return Mapper.Map<Location>(baseResult);
    }

    public async Task DeleteAsync(int id)
    {
        await base.DeleteByIdAsync(id);
    }


}

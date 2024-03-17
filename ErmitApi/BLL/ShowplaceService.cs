
using AutoMapper;
using ErmitApi.DAL;
using ErmitApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ErmitApi.BLL;


public class ShowplaceService : ServiceBase<DAL.Models.Showplace, int>
{
    public ShowplaceService(IServiceProvider serviceProvider, ILogger<ShowplaceService> logger)
    : base(serviceProvider, logger)
    {
        this.Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;

            #region Base To ViewModel
            cfg.CreateMap<DAL.Models.Showplace, Showplace>();
            #endregion

            #region ViewModel To Base 
            cfg.CreateMap<ShowplaceCreateModel, DAL.Models.Showplace>()
                .ForMember(dest => dest.PictureData, opt => opt.MapFrom(src => ConvertToBytes(src.PictureFile)))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => GetFileExtension(src.PictureFile)));

            cfg.CreateMap<ShowplaceUpdateModel, DAL.Models.Showplace>()
                .ForMember(dest => dest.PictureData, opt => opt.MapFrom(src => ConvertToBytes(src.PictureFile)))
                .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => GetFileExtension(src.PictureFile)));
            #endregion
        }).CreateMapper();
    }

    public async Task<List<Showplace>> GetAllAsync()
    {
        var baseResult = (await base.GetAllAsync()).ToList();

        return Mapper.Map<List<Showplace>>(baseResult);
    }

    public async Task<Showplace> GetByIdAsync(int id)
    {
        var baseResult = await base.GetByIdAsync(id);

        return Mapper.Map<Showplace>(baseResult);
    }

    public async Task<Showplace> CreateAsync(ShowplaceCreateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.Showplace>(model);

        var baseResult = await base.AddAsync(achievement);

        return Mapper.Map<Showplace>(baseResult);
    }

    public async Task<Showplace> UpdateAsync(ShowplaceUpdateModel model)
    {
        await Validate(model);

        var achievement = Mapper.Map<DAL.Models.Showplace>(model);

        var baseResult = await base.UpdateAsync(achievement);

        return Mapper.Map<Showplace>(baseResult);
    }

    public async Task DeleteAsync(int id)
    {
        await base.DeleteByIdAsync(id);
    }

    private async Task Validate(ShowplaceCreateModel model)
    {
        await using var context = this.GetContext<DataBaseContext>();

        var location = await context.Locations.FirstOrDefaultAsync(l => l.Id == model.LocationId);

        if (location == null)
        {
            throw new ArgumentException($"Location id = {model.LocationId} does not exists");
        }

        return;
    }
}



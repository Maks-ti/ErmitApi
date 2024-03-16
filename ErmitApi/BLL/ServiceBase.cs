using ErmitApi.DAL.Models;
using System.Collections.Generic;
using AutoMapper;
using ErmitApi.DAL;
using ErmitApi.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Internal.Mappers;
using ErmitApi.Models;

namespace ErmitApi.BLL;

public abstract class ServiceBase<TEntity, IdType, TModel, TModelCreate>
   where TEntity : class, IBaseEntity<IdType>
   where IdType : IEquatable<IdType>
{
    public ServiceBase(IServiceProvider serviceProvider, ILogger logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;

        this.Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;

            #region Base To ViewModel
            cfg.CreateMap<TEntity, TModel>();
            #endregion

            #region Base To ViewModel
            cfg.CreateMap<TModel, TEntity>();
            cfg.CreateMap<TModelCreate, TEntity>();
            #endregion
        }).CreateMapper();

    }

    protected ILogger Logger { get; }
    protected IMapper Mapper;
    private IServiceProvider ServiceProvider { get; }

    protected ContextType GetContext<ContextType>()
    where ContextType : DbContext
    {
        return ServiceProvider.GetRequiredService<ContextType>();
    }

    public virtual async Task<IEnumerable<TModel>> GetAllAsync()
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        var list =  await dbSet.ToListAsync();

        return Mapper.Map<IEnumerable<TModel>>(list);
    }

    public virtual async Task<TModel?> GetByIdAsync(IdType entityId)
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        var query = from entity in dbSet
                    where entity.Id.Equals(entityId)
                    select entity;

        TEntity? res =  await query.FirstOrDefaultAsync();

        return Mapper.Map<TModel>(res);
    }

    public virtual async Task DeleteByIdAsync(IdType entityId)
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        var oldEntity = await dbSet.FirstOrDefaultAsync(e => e.Id.Equals(entityId));

        if (oldEntity == null) throw new ArgumentException($"{nameof(TEntity)} с id = {entityId} не существует");

        if (context.Entry(oldEntity).State == EntityState.Detached)
        {
            dbSet.Attach(oldEntity);
        }

        dbSet.Remove(oldEntity);

        await context.SaveChangesAsync();
    }

    public virtual async Task<TModel> AddAsync(TModelCreate entity)
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        TEntity entityCreate = Mapper.Map<TEntity>(entity);

        dbSet.Add(entityCreate);

        await context.SaveChangesAsync();

        return Mapper.Map<TModel>(entity);
    }

    public virtual async Task<TModel?> UpdateAsync(TModel entity)
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        TEntity entityUpdate = Mapper.Map<TEntity>(entity);

        if ((await dbSet.AnyAsync(e => e.Id.Equals(entityUpdate.Id))) == false) throw new ArgumentException($"{nameof(TEntity)} с id = {entityUpdate.Id} не существует"); // сущности в базе нет (не обновляем)

        dbSet.Update(entityUpdate);

        await context.SaveChangesAsync();

        return Mapper.Map<TModel>(entity);
    }
}

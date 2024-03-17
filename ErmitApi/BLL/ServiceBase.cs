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

public abstract class ServiceBase<TEntity, IdType>
   where TEntity : class, IBaseEntity<IdType>
   where IdType : IEquatable<IdType>
{
    public ServiceBase(IServiceProvider serviceProvider, ILogger logger)
    {
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    protected ILogger Logger { get; }
    protected IMapper Mapper;
    private IServiceProvider ServiceProvider { get; }

    protected ContextType GetContext<ContextType>()
    where ContextType : DbContext
    {
        return ServiceProvider.GetRequiredService<ContextType>();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        return await dbSet.ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int entityId)
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        var query = from entity in dbSet
                    where entity.Id.Equals(entityId)
                    select entity;

        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task DeleteByIdAsync(int entityId)
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

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        dbSet.Add(entity);

        await context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task<TEntity?> UpdateAsync(TEntity entity)
    {
        await using var context = GetContext<DataBaseContext>();

        DbSet<TEntity> dbSet = context.Set<TEntity>();

        if ((await dbSet.AnyAsync(e => e.Id.Equals(entity.Id))) == false) throw new ArgumentException($"{nameof(TEntity)} с id = {entity.Id} не существует"); // сущности в базе нет (не обновляем)

        dbSet.Update(entity);

        await context.SaveChangesAsync();

        return entity;
    }

    protected byte[] ConvertToBytes(IFormFile pictureFile)
    {
        if (pictureFile == null || pictureFile.Length == 0) return null;

        using (var memoryStream = new MemoryStream())
        {
            pictureFile.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    protected string GetFileExtension(IFormFile pictureFile)
    {
        if (pictureFile == null || pictureFile.Length == 0) return null;
        return Path.GetExtension(pictureFile.FileName);
    }


}

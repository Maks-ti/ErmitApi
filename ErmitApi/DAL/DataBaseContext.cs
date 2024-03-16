using ErmitApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Reflection;
using System;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Reflection; // for Assembly


namespace ErmitApi.DAL;

public class DataBaseContext : DbContext
{
    public DataBaseContext(DbContextOptions<DataBaseContext> options) 
        : base(options) { }

    // список DbSet`ов
    public DbSet<User> Users { get; set; }
    public DbSet<Achievement> Achives { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурации хранятся рядом с моделями сущностей базы (в том же файле)
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

}

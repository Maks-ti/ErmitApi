

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErmitApi.DAL.Models;

public class Stand : IBaseEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LocationId { get; set; }

    public virtual Location Location { get; set; }
}

public class StandSourceConfiguration : IEntityTypeConfiguration<Stand>
{
    public void Configure(EntityTypeBuilder<Stand> builder)
    {
        builder.ToTable("stand");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name");

        builder.Property(x => x.LocationId)
            .HasColumnName("location_id");

        builder.HasOne(x => x.Location)
            .WithMany(location => location.Stands)
            .HasForeignKey(x => x.LocationId);
    }
}
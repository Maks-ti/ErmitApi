
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErmitApi.DAL.Models;

public class Location : IBaseEntity<int>
{
    public int Id {  get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public byte[] PictureData { get; set; }
    public string Extension { get; set; }


    public virtual ICollection<Showplace> Showplaces { get; set; }
    public virtual ICollection<Stand> Stands { get; set; }
    public virtual ICollection<Achievement> Achievements { get; set; }
}

public class LocationSourceConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("location");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name");

        builder.Property(x => x.Description)
            .HasColumnName("description");

        builder.Property(x => x.PictureData)
            .HasColumnName("picture_data");

        builder.Property(x => x.Extension)
            .HasColumnName("picture_extension");
    }
}

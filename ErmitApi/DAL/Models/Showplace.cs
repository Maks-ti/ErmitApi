
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ErmitApi.DAL.Models;

public class Showplace : IBaseEntity<int>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public int LocationId { get; set; }
    public byte[] PictureData { get; set; }
    public string Extension { get; set; }

    public virtual Location Location { get; set; }
    public virtual ICollection<Achievement> Achievements { get; set; }
}

public class ShowplaceSourceConfiguration : IEntityTypeConfiguration<Showplace>
{
    public void Configure(EntityTypeBuilder<Showplace> builder)
    {
        builder.ToTable("showplace");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name");

        builder.Property(x => x.Description)
            .HasColumnName("description");

        builder.Property(x => x.Address)
            .HasColumnName("address");

        builder.Property(x => x.PictureData)
            .HasColumnName("picture_data");

        builder.Property(x => x.Extension)
            .HasColumnName("picture_extension");

        builder.Property(x => x.LocationId)
            .HasColumnName("location_id");

        builder.HasOne(x => x.Location)
            .WithMany(location => location.Showplaces)
            .HasForeignKey(x => x.LocationId);
    }
}
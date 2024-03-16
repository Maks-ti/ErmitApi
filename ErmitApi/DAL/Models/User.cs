
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ErmitApi.DAL.Models;

public class User : IBaseEntity<Guid>
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public bool IsAdmin { get; set; }
    public string Salt { get; set; }
    public string PasswordHash { get; set; }

    public virtual ICollection<UserAchievement> UserAchivements { get; set; }
}

public class UserSourceConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Login)
            .HasColumnName("login");

        builder.Property(x => x.Name)
            .HasColumnName("name");

        builder.Property(x => x.IsAdmin)
            .HasColumnName("is_admin");

        builder.Property(x => x.Salt)
            .HasColumnName("salt");

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash");
    }
}

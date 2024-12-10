using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configuration
{
    internal class AuthorConfiguration : BaseEntityConfiguration<Author,int>
    {
        public override void Configure(EntityTypeBuilder<Author> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Description).HasMaxLength(500);
            builder.Property(e => e.BirthDate).IsRequired();
            builder.Property(e => e.Email).IsRequired().HasMaxLength(100);
            builder.HasMany(e => e.Books)
                .WithOne(e => e.Author)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

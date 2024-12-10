using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Domains;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configuration
{
    internal class PublisherConfiguration : BaseEntityConfiguration<Publisher,int>
    {
        public override void Configure(EntityTypeBuilder<Publisher> builder)
        {
            base.Configure(builder);
            builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Address).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Phone).HasMaxLength(20).IsRequired();
            builder.Property(p => p.Email).HasMaxLength(100).IsRequired();
            builder.HasMany(p => p.Books)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId);
        }
    }
}

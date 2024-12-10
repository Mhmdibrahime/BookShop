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
    internal class BookConfiguration : BaseEntityConfiguration<Book,int>
    {
        public override void Configure(EntityTypeBuilder<Book> builder)
        {
            base.Configure(builder);
            builder.Property(b => b.Title).HasMaxLength(100).IsRequired();
            builder.Property(b => b.Description).HasMaxLength(500);
            builder.Property(b => b.PublishedYear).IsRequired();
            builder.Property(b => b.SalePrice).HasColumnType("decimal(8,2)").IsRequired();
            builder.Property(b => b.RentPrice).HasColumnType("decimal(8,2)").IsRequired();
            builder.Property(b => b.Stock).IsRequired();
            builder.Property(b => b.Cover).IsRequired(false);
        }
    }
}

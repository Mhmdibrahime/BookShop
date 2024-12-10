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
    internal class BaseEntityConfiguration<TEntity,TKey> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.ModifiedDate).HasDefaultValueSql("GETDATE()");
            builder.Property(e => e.DeletedDate).HasDefaultValue(null);
            builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        }
    }
}

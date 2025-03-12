using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TestBulkDbLibraries.Entities;

public abstract class EntityBase : EntityBase<int>, IEntity<int>, IMappedEntity
{
    protected EntityBase() : base()
    {
    }

    protected EntityBase(int id, DateTime createdAt, DateTime modifiedAt) : base(id, createdAt, modifiedAt)
    {
    }
}

public abstract class EntityBase<TPrimaryKey> : IEntity<TPrimaryKey>, IMappedEntity
    where TPrimaryKey : struct, IComparable
{
    protected EntityBase()
    {
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = CreatedAt;
    }

    protected EntityBase(TPrimaryKey id, DateTime createdAt, DateTime modifiedAt)
    {
        if (id.CompareTo(default(TPrimaryKey)) <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(createdAt));
        }

        if (modifiedAt == DateTime.MinValue || modifiedAt == DateTime.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(modifiedAt));
        }

        if (modifiedAt < createdAt)
        {
            throw new ArgumentException($"Argument \"{nameof(createdAt)}\" cannot be greater than argument \"{nameof(modifiedAt)}\".");
        }

        Id = id;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    [Key]
    public TPrimaryKey Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public uint Xmin { get; set; }

    public abstract void Map(ModelBuilder modelBuilder);
}

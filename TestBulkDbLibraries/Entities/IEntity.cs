namespace TestBulkDbLibraries.Entities;

public interface IEntity
{
    DateTime CreatedAt { get; set; }

    DateTime ModifiedAt { get; set; }
}


public interface IEntity<TPrimaryKey> : IEntity
    where TPrimaryKey : struct, IComparable
{
    TPrimaryKey Id { get; set; }
}
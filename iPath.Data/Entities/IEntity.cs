namespace iPath.Data.Entities;

public interface IEntity
{
}

public class BaseEntity : IEntity
{
    public int Id { get; set; }
}
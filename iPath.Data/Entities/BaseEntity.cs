namespace iPath.Data.Entities;

public abstract class BaseEntity : IEntity
{
    public int Id { get; set; }
}


public abstract class BaseEntityWithDeleteFlag : BaseEntity
{
    public DateTime? DeletedOn { get; set; }
}
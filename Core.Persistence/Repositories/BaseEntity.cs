namespace Core.Persistence.Repositories;

public  abstract class BaseEntity<TId>
{
    public TId Id { get; set; }
}

namespace WaterFilterBusiness.Domain.Entities;

public class Entity<TKey>
{
    public TKey Id { get; set; }
}

public class Entity : Entity<int>
{
}
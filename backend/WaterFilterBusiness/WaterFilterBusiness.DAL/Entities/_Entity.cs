namespace WaterFilterBusiness.DAL.Entities;

public class Entity<TKey>
{
    public TKey Id { get; set; }
}

public class Entity : Entity<int>
{
}
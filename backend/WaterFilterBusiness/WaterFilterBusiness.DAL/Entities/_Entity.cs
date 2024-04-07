namespace WaterFilterBusiness.DAL.Entities;

public abstract class BaseEntity<TKey>
{
}

public class WeakEntity<TKey> : BaseEntity<TKey>
{
}

public class StrongEntity<TKey> : BaseEntity<TKey>
{
    public TKey Id { get; set; }
}

public class StrongEntity : StrongEntity<int>
{
}

public class PlainStrongEntity : StrongEntity
{
    public string Name { get; set; }
}
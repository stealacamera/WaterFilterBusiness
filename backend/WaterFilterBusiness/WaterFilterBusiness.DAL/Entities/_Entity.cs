namespace WaterFilterBusiness.DAL.Entities;

public abstract class BaseEntity
{
}

public abstract class BaseEntity<TKey> : BaseEntity
{
}

public abstract class WeakCompositeEntity<TKey1, TKey2> : BaseEntity
{
}

public abstract class StrongEntity<TKey> : BaseEntity<TKey>
{
    public TKey Id { get; set; }
}

public abstract class StrongEntity : StrongEntity<int>
{
}

public abstract class PlainStrongEntity : StrongEntity
{
    public string Name { get; set; }
}
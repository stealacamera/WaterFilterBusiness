namespace WaterFilterBusiness.DAL.Entities;

public abstract class BaseEntity { }

public abstract class BaseEntity<TKey> : BaseEntity { }

public abstract class StrongEntity<TKey> : BaseEntity<TKey>
{
    public TKey Id { get; set; }
}

public class StrongEntity: StrongEntity<int> { }

public abstract class PlainStrongEntity : StrongEntity
{
    public string Name { get; set; } = null!;
}

public abstract class CompositeEntity<TKey1, TKey2> : BaseEntity { }
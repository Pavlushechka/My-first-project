using UnityEngine;

public struct HitResult
{
    public bool destroyed;
    public ItemData dropItem;
    public int dropAmount;

    public static HitResult NotDestroyed()
    {
        return new HitResult { destroyed = false };
    }

    public static HitResult Destroyed(ItemData item, int amount)
    {
        return new HitResult
        {
            destroyed = true,
            dropItem = item,
            dropAmount = amount
        };
    }
}
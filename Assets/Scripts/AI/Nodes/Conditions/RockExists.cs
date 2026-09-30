using UnityEngine;

public class RockExists : BTLeaf
{
    protected override BTStatus OnTick()
    {
        var rocks = Object.FindObjectsByType<ResourceRock>(FindObjectsSortMode.None);
        return rocks.Length > 0 ? BTStatus.Success : BTStatus.Failure;
    }
}
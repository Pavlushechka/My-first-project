using UnityEngine;

public class TreeExists : BTLeaf
{
    protected override BTStatus OnTick()
    {
        // Ищем все деревья на сцене
        var trees = Object.FindObjectsByType<ResourceTree>(FindObjectsSortMode.None);

        // Если хоть одно есть — Success
        return trees.Length > 0 ? BTStatus.Success : BTStatus.Failure;
    }
}
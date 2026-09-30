using UnityEngine;

public class HasItems : BTLeaf
{
    private NPCInventory inv;

    protected override BTStatus OnTick()
    {
        if (agent == null)
        {
            Debug.LogWarning("[HasItems] agent == null");
            return BTStatus.Failure;
        }

        if (inv == null)
            inv = agent.GetComponent<NPCInventory>();

        if (inv == null)
        {
            Debug.LogWarning("[HasItems] NPCInventory не найден");
            return BTStatus.Failure;
        }

        return inv.HasItems ? BTStatus.Success : BTStatus.Failure;
    }
}
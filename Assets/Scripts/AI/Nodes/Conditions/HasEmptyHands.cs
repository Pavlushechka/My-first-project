using UnityEngine;

public class HasEmptyHands : BTLeaf
{
    private NPCInventory inv;

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        if (inv == null)
            inv = agent.GetComponent<NPCInventory>();

        if (inv == null) return BTStatus.Failure;

        // Руки пусты — Success, иначе Failure
        return inv.HasItems ? BTStatus.Failure : BTStatus.Success;
    }
}
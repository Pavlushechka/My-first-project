public class IsHungry : BTLeaf
{
    private NPCStats stats;

    protected override BTStatus OnTick()
    {
        if (agent == null)
        {
            UnityEngine.Debug.LogWarning("[IsHungry] agent == null");
            return BTStatus.Failure;
        }

        if (stats == null)
            stats = agent.GetComponent<NPCStats>();

        if (stats == null)
        {
            UnityEngine.Debug.LogWarning("[IsHungry] NPCStats не найден");
            return BTStatus.Failure;
        }

        return stats.IsHungry() ? BTStatus.Success : BTStatus.Failure;
    }
}
public class Eat : BTLeaf
{
    private NPCStats stats;
    private float eatDuration = 2f;
    private float timer;

    protected override BTStatus OnTick()
    {
        if (agent == null)
        {
            UnityEngine.Debug.LogWarning("[Eat] agent == null");
            return BTStatus.Failure;
        }

        if (stats == null)
            stats = agent.GetComponent<NPCStats>();

        if (stats == null)
        {
            UnityEngine.Debug.LogWarning("[Eat] NPCStats не найден");
            return BTStatus.Failure;
        }

        timer += UnityEngine.Time.deltaTime;

        if (timer < eatDuration)
            return BTStatus.Running;

        stats.Eat(100f);
        timer = 0f;

        UnityEngine.Debug.Log($"[NPC] Поел. Голод: {stats.currentHunger}");
        return BTStatus.Success;
    }
}
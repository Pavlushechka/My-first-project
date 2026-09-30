using UnityEngine;

public class FindNearestTree : BTLeaf
{
    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        // Ищем все деревья
        var trees = Object.FindObjectsByType<ResourceTree>(FindObjectsSortMode.None);

        if (trees.Length == 0)
            return BTStatus.Failure;

        // Ищем ближайшее
        Transform closest = null;
        float closestDist = float.MaxValue;
        Vector3 npcPos = agent.transform.position;

        foreach (var tree in trees)
        {
            float dist = Vector3.Distance(npcPos, tree.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = tree.transform;
            }
        }

        if (closest == null)
            return BTStatus.Failure;

        // ПИШЕМ В BLACKBOARD: ближайшее дерево — наша цель
        blackboard.Set(BBKeys.TargetTree, closest);
        blackboard.Set(BBKeys.NearestTree, closest);

        return BTStatus.Success;
    }
}
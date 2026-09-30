using UnityEngine;

public class FindNearestWarehouse : BTLeaf
{
    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        // Ищем все склады на сцене
        var warehouses = Object.FindObjectsByType<StorageContainer>(FindObjectsSortMode.None);

        if (warehouses.Length == 0)
        {
            Debug.LogWarning("[FindNearestWarehouse] Складов на сцене нет!");
            return BTStatus.Failure;
        }

        // Ищем ближайший
        Transform closest = null;
        float closestDist = float.MaxValue;
        Vector3 npcPos = agent.transform.position;

        foreach (var w in warehouses)
        {
            float dist = Vector3.Distance(npcPos, w.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = w.transform;
            }
        }

        if (closest == null)
            return BTStatus.Failure;

        // ПИШЕМ В BLACKBOARD
        blackboard.Set(BBKeys.TargetWarehouse, closest);
        blackboard.Set(BBKeys.NearestWarehouse, closest);

        Debug.Log($"[FindNearestWarehouse] Нашёл склад на расстоянии {closestDist:F1}");

        return BTStatus.Success;
    }
}
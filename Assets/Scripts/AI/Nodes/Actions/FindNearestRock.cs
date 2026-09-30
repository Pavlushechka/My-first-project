using UnityEngine;

public class FindNearestRock : BTLeaf
{
    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        var rocks = Object.FindObjectsByType<ResourceRock>(FindObjectsSortMode.None);

        if (rocks.Length == 0)
            return BTStatus.Failure;

        Transform closest = null;
        float closestDist = float.MaxValue;
        Vector3 npcPos = agent.transform.position;

        foreach (var rock in rocks)
        {
            float dist = Vector3.Distance(npcPos, rock.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = rock.transform;
            }
        }

        if (closest == null)
            return BTStatus.Failure;

        blackboard.Set(BBKeys.TargetTree, closest);   // используем тот же ключ
        blackboard.Set(BBKeys.NearestTree, closest);

        Debug.Log($"[FindNearestRock] Нашёл камень на расстоянии {closestDist:F1}");

        return BTStatus.Success;
    }
}
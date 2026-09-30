using UnityEngine;
using UnityEngine.AI;

public class ChopTree : BTLeaf
{
    [Header("Chop Settings")]
    public float chopInterval = 1f;
    public float damagePerHit = 10f;
    public float maxDistance = 2f;

    private float timer;
    private NavMeshAgent navAgent;

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        var treeTransform = blackboard.Get<Transform>(BBKeys.TargetTree);

        if (treeTransform == null)
        {
            Debug.LogWarning("[ChopTree] TargetTree == null");
            return BTStatus.Failure;
        }

        var tree = treeTransform.GetComponent<ResourceTree>();

        if (tree == null)
        {
            Debug.LogWarning("[ChopTree] ResourceTree не найден");
            return BTStatus.Failure;
        }

        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        float dist = Vector3.Distance(agent.transform.position, tree.transform.position);

        if (dist > maxDistance)
        {
            Debug.Log($"[ChopTree] Слишком далеко ({dist:F1})");
            return BTStatus.Failure;
        }

        if (navAgent != null && navAgent.hasPath)
            navAgent.ResetPath();

        timer += Time.deltaTime;

        if (timer < chopInterval)
            return BTStatus.Running;

        // ===== УДАР! =====
        timer = 0f;

        // Используем ЕДИНЫЙ API
        var result = tree.TakeHit(agent.gameObject, damagePerHit);

        if (result.destroyed)
        {
            // Кладём ресурс в NPCInventory
            var inv = agent.GetComponent<NPCInventory>();

            if (inv != null && result.dropItem != null)
            {
                inv.Pickup(result.dropItem, result.dropAmount);
            }

            blackboard.Remove(BBKeys.TargetTree);

            Debug.Log("[ChopTree] Дерево срублено!");
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}
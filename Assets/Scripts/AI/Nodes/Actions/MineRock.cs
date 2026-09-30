using UnityEngine;
using UnityEngine.AI;

public class MineRock : BTLeaf
{
    public float mineInterval = 1.5f;    // медленнее, чем у дерева
    public float damagePerHit = 5f;      // меньше урона
    public float maxDistance = 2f;

    private float timer;
    private NavMeshAgent navAgent;

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        var rockTransform = blackboard.Get<Transform>(BBKeys.TargetTree);

        if (rockTransform == null)
            return BTStatus.Failure;

        var rock = rockTransform.GetComponent<ResourceRock>();

        if (rock == null)
            return BTStatus.Failure;

        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        float dist = Vector3.Distance(agent.transform.position, rock.transform.position);

        if (dist > maxDistance)
            return BTStatus.Failure;

        if (navAgent != null && navAgent.hasPath)
            navAgent.ResetPath();

        timer += Time.deltaTime;

        if (timer < mineInterval)
            return BTStatus.Running;

        timer = 0f;

        var result = rock.TakeHit(agent.gameObject, damagePerHit);

        Debug.Log($"[MineRock] Удар. HP камня: {rock.currentHealth}");

        if (result.destroyed)
        {
            var inv = agent.GetComponent<NPCInventory>();
            if (inv != null && result.dropItem != null)
                inv.Pickup(result.dropItem, result.dropAmount);

            blackboard.Remove(BBKeys.TargetTree);

            Debug.Log("[MineRock] Камень добыт!");
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}
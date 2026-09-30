using UnityEngine;
using UnityEngine.AI;

public class MoveToRock : BTLeaf
{
    public float arriveDistance = 1.5f;

    private NavMeshAgent navAgent;

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        var rock = blackboard.Get<Transform>(BBKeys.TargetTree);

        if (rock == null)
        {
            Debug.LogWarning("[MoveToRock] TargetTree == null в blackboard");
            return BTStatus.Failure;
        }

        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        if (navAgent == null)
            return BTStatus.Failure;

        navAgent.SetDestination(rock.position);

        float dist = Vector3.Distance(agent.transform.position, rock.position);

        if (dist <= arriveDistance)
        {
            navAgent.ResetPath();
            Debug.Log("[MoveToRock] Пришёл к камню");
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}
using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget : BTLeaf
{
    private Vector3 targetPosition;
    private bool hasTarget;
    private NavMeshAgent navAgent;

    public MoveToTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    public MoveToTarget()
    {
        hasTarget = false;
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        if (navAgent == null) return BTStatus.Failure;

        if (!hasTarget)
            return BTStatus.Failure;

        navAgent.SetDestination(targetPosition);

        if (navAgent.pathPending)
            return BTStatus.Running;

        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            hasTarget = false;
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}
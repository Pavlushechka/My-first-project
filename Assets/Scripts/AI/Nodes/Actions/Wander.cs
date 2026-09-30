using UnityEngine;
using UnityEngine.AI;

public class Wander : BTLeaf
{
    public float wanderRadius = 10f;

    private NavMeshAgent navAgent;
    private Vector3 targetPos;

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        if (navAgent == null) return BTStatus.Failure;

        // Если ещё не дошли — идём
        if (navAgent.pathPending || navAgent.remainingDistance > navAgent.stoppingDistance)
            return BTStatus.Running;

        // Дошли — выбираем новую точку и продолжаем
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
        randomDir.y = 0f;
        Vector3 newTarget = agent.transform.position + randomDir;

        if (NavMesh.SamplePosition(newTarget, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            navAgent.SetDestination(hit.position);

        return BTStatus.Running;  // Wander никогда не заканчивается
    }
}
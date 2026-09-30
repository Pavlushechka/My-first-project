using UnityEngine;
using UnityEngine.AI;

public class MoveToTree : BTLeaf
{
    public float arriveDistance = 1.5f;

    private NavMeshAgent navAgent;

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        // ЧИТАЕМ ИЗ BLACKBOARD: куда идти
        var tree = blackboard.Get<Transform>(BBKeys.TargetTree);

        if (tree == null)
        {
            Debug.LogWarning("[MoveToTree] TargetTree == null в blackboard");
            return BTStatus.Failure;
        }

        // Инициализируем NavMeshAgent (один раз)
        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        if (navAgent == null)
            return BTStatus.Failure;

        // Ставим цель движения
        navAgent.SetDestination(tree.position);

        float dist = Vector3.Distance(agent.transform.position, tree.position);

        // Пришли?
        if (dist <= arriveDistance)
        {
            navAgent.ResetPath();
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}
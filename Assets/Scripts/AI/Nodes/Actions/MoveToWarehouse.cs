using UnityEngine;
using UnityEngine.AI;

public class MoveToWarehouse : BTLeaf
{
    public float arriveDistance = 3f;

    private NavMeshAgent navAgent;

    protected override BTStatus OnTick()
    {

        if (agent == null) return BTStatus.Failure;

        var warehouse = blackboard.Get<Transform>(BBKeys.TargetWarehouse);

        if (warehouse == null)
        {
            Debug.LogWarning("[MoveToWarehouse] TargetWarehouse == null в blackboard");
            return BTStatus.Failure;
        }

        if (navAgent == null)
            navAgent = agent.GetComponent<NavMeshAgent>();

        if (navAgent == null)
            return BTStatus.Failure;

        navAgent.SetDestination(warehouse.position);

        float dist = Vector3.Distance(agent.transform.position, warehouse.position);

        if (dist <= arriveDistance)
        {
            navAgent.ResetPath();
            Debug.Log("[MoveToWarehouse] Пришёл к складу");
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}
using UnityEngine;

public enum BTStatus
{
    Success,
    Failure,
    Running
}

public abstract class BTNode
{
    public string name = "Node";
    public BTStatus lastStatus = BTStatus.Failure;

    protected BTAgent agent;
    protected Blackboard blackboard;

    public void SetAgent(BTAgent agent)
    {
        this.agent = agent;
        this.blackboard = agent != null ? agent.Blackboard : null;
    }

    public BTStatus Tick()
    {
        lastStatus = OnTick();
        return lastStatus;
    }

    protected abstract BTStatus OnTick();

    public virtual void Reset()
    {
        lastStatus = BTStatus.Failure;
    }
}
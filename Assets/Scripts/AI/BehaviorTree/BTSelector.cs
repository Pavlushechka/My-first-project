public class BTSelector : BTComposite
{
    public BTSelector(params BTNode[] nodes) : base(nodes) { }

    protected override BTStatus OnTick()
    {
        foreach (var child in children)
        {
            child.SetAgent(agent);

            var status = child.Tick();

            if (status == BTStatus.Running)
                return BTStatus.Running;

            if (status == BTStatus.Success)
                return BTStatus.Success;

            // Failure → пробуем следующего
        }

        return BTStatus.Failure;
    }
}
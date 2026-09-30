public class BTSequence : BTComposite
{
    private int currentIndex;

    public BTSequence(params BTNode[] nodes) : base(nodes) { }

    protected override BTStatus OnTick()
    {
        while (currentIndex < children.Count)
        {
            var child = children[currentIndex];
            child.SetAgent(agent);

            var status = child.Tick();

            if (status == BTStatus.Running)
                return BTStatus.Running;

            if (status == BTStatus.Failure)
            {
                currentIndex = 0;
                return BTStatus.Failure;
            }

            currentIndex++;
        }

        currentIndex = 0;
        return BTStatus.Success;
    }

    public override void Reset()
    {
        base.Reset();
        currentIndex = 0;
    }
}
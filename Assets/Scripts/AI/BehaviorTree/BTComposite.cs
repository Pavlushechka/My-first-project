using System.Collections.Generic;

public abstract class BTComposite : BTNode
{
    public List<BTNode> children = new List<BTNode>();

    public BTComposite(params BTNode[] nodes)
    {
        children.AddRange(nodes);
    }

    public void AddChild(BTNode node)
    {
        children.Add(node);
    }

    public override void Reset()
    {
        base.Reset();
        foreach (var child in children)
            child.Reset();
    }
}
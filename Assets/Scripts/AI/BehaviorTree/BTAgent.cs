using UnityEngine;

public class BTAgent : MonoBehaviour
{
    [Header("Debug")]
    public bool debugLogs = false;
    public bool logBlackboard = false;

    protected BTNode root;

    public Blackboard Blackboard { get; private set; }

    void Awake()
    {
        Blackboard = new Blackboard();
    }

    public void SetRoot(BTNode newRoot)
    {
        root = newRoot;
        if (root != null)
            root.SetAgent(this);
    }

    void Update()
    {
        if (root == null) return;

        var status = root.Tick();

        if (debugLogs)
            

        if (logBlackboard && Time.frameCount % 60 == 0)
            Blackboard.DebugDump();
    }

    public void ResetTree()
    {
        root?.Reset();
        Blackboard.Clear();
    }
}
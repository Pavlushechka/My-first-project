using UnityEngine;

public class BTTest : BTAgent
{
    [Header("Test Action")]
    public int successAfterTicks = 3;

    private int tickCount;

    void Start()
    {
        // Дерево: Sequence [ PrintHello, WaitN ]
        var tree = new BTSequence(
            new PrintHello(),
            new WaitNTicks(successAfterTicks)
        );

        SetRoot(tree);
    }

    class PrintHello : BTLeaf
    {
        protected override BTStatus OnTick()
        {
            Debug.Log("Hello from BT!");
            return BTStatus.Success;
        }
    }

    class WaitNTicks : BTLeaf
    {
        private int maxTicks;
        private int current;

        public WaitNTicks(int max) { maxTicks = max; }

        protected override BTStatus OnTick()
        {
            current++;
            Debug.Log($"[WaitNTicks] {current}/{maxTicks}");

            if (current < maxTicks)
                return BTStatus.Running;

            current = 0;
            return BTStatus.Success;
        }
    }
}
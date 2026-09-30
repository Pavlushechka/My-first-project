using UnityEngine;

[RequireComponent(typeof(BTAgent))]
public class NPCBrain : MonoBehaviour
{
    [Header("Role")]
    public NPCRole role = NPCRole.Woodcutter;

    [Header("Identity")]
    public int npcId = 0;

    [Header("Debug")]
    public bool debugLogs = false;

    private BTAgent btAgent;

    void Awake()
    {
        btAgent = GetComponent<BTAgent>();
    }

    void Start()
    {
        BuildTree();
    }

    public void SetRole(NPCRole newRole)
    {
        if (role == newRole) return;

        role = newRole;
        BuildTree();

        Debug.Log($"[NPCBrain] {gameObject.name} сменил роль на {newRole}");
    }

    void BuildTree()
    {
        if (btAgent == null)
        {
            Debug.LogError("[NPCBrain] btAgent == null");
            return;
        }

        // Ветка 1: Отнести на склад (общая для всех ролей)
        var depositSequence = new BTSequence(
            new HasItems(),
            new FindNearestWarehouse(),
            new MoveToWarehouse(),
            new DepositWood()
        );

        // Ветка 2: Поесть (общая)
        var eatSequence = new BTSequence(
            new IsHungry(),
            new Eat()
        );

        // Ветка 3: Работа (зависит от роли)
        BTSequence workSequence = role switch
        {
            NPCRole.Woodcutter => BuildWoodcutterSequence(),
            NPCRole.Miner => BuildMinerSequence(),
            _ => BuildWoodcutterSequence()
        };

        // Корень: Selector
        var root = new BTSelector(
            depositSequence,
            eatSequence,
            workSequence,
            new Wander()
        );

        btAgent.SetRoot(root);
        btAgent.debugLogs = debugLogs;

        Debug.Log($"[NPCBrain] {gameObject.name} дерево построено. Роль: {role}");
    }

    BTSequence BuildWoodcutterSequence()
    {
        return new BTSequence(
            new HasEmptyHands(),
            new TreeExists(),
            new FindNearestTree(),
            new MoveToTree(),
            new ChopTree()
        );
    }

    BTSequence BuildMinerSequence()
    {
        return new BTSequence(
            new HasEmptyHands(),
            new RockExists(),
            new FindNearestRock(),
            new MoveToRock(),
            new MineRock()
        );
    }
}
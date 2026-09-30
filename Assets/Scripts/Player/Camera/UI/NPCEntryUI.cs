using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCEntryUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI nameText;
    public TMP_Dropdown roleDropdown;

    private NPCBrain targetNPC;

    void Awake()
    {
        if (roleDropdown != null)
            roleDropdown.onValueChanged.AddListener(OnRoleChanged);
    }

    void OnDestroy()
    {
        if (roleDropdown != null)
            roleDropdown.onValueChanged.RemoveListener(OnRoleChanged);
    }

    public void Setup(NPCBrain npc)
    {
        targetNPC = npc;

        if (nameText != null)
            nameText.text = $"Поселенец #{npc.npcId}";

        if (roleDropdown != null)
        {
            // Выставляем текущую роль без вызова события
            roleDropdown.SetValueWithoutNotify((int)npc.role);
        }
    }

    void OnRoleChanged(int index)
    {
        if (targetNPC == null) return;

        NPCRole newRole = (NPCRole)index;
        targetNPC.SetRole(newRole);

        Debug.Log($"[NPCEntryUI] Поселенец #{targetNPC.npcId} → {newRole}");
    }
}
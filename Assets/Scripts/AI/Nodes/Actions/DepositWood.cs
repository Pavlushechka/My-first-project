using UnityEngine;

public class DepositWood : BTLeaf
{
    public float depositInterval = 0.3f;
    public float maxDistance = 3f;

    private float timer;

    protected override BTStatus OnTick()
    {
        if (agent == null) return BTStatus.Failure;

        // Читаем цель из blackboard
        var warehouseTransform = blackboard.Get<Transform>(BBKeys.TargetWarehouse);

        if (warehouseTransform == null)
        {
            Debug.LogWarning("[DepositWood] TargetWarehouse == null");
            return BTStatus.Failure;
        }

        var container = warehouseTransform.GetComponent<StorageContainer>();

        if (container == null)
        {
            Debug.LogWarning("[DepositWood] StorageContainer не найден");
            return BTStatus.Failure;
        }

        // Проверка дистанции
        float dist = Vector3.Distance(agent.transform.position, warehouseTransform.position);

        if (dist > maxDistance)
        {
            Debug.Log($"[DepositWood] Слишком далеко ({dist:F1})");
            return BTStatus.Failure;
        }

        // Получаем инвентарь NPC
        var npcInv = agent.GetComponent<NPCInventory>();

        if (npcInv == null || !npcInv.HasItems)
        {
            Debug.Log("[DepositWood] Нет предметов для выгрузки");
            return BTStatus.Success;   // нечего носить — считаем работу выполненной
        }

        // Тикаем таймер
        timer += Time.deltaTime;

        if (timer < depositInterval)
            return BTStatus.Running;

        // Складываем ПО ОДНОМУ предмету за интервал
        timer = 0f;

        var item = npcInv.carriedItem;
        int amount = 1;

        // Списываем 1 из NPCInventory
        npcInv.currentCarry -= amount;

        // Добавляем 1 в склад
        container.inventory.AddItem(item, amount);

        Debug.Log($"[DepositWood] Положил {item.displayName} x{amount}. Осталось: {npcInv.currentCarry}");

        // Если всё выгружено — сбрасываем carriedItem
        if (npcInv.currentCarry <= 0)
        {
            npcInv.carriedItem = null;
            blackboard.Remove(BBKeys.TargetWarehouse);

            Debug.Log("[DepositWood] Всё выгружено!");
            return BTStatus.Success;
        }

        return BTStatus.Running;
    }
}
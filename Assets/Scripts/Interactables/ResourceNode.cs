using UnityEngine;

public abstract class ResourceNode : Interactable
{
    [Header("Resource")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Reward")]
    public ItemData dropItem;
    public int amountPerHit = 1;

    [Header("Tool")]
    [Tooltip("Какой инструмент нужен для эффективной добычи")]
    public ItemData requiredTool;

    [Tooltip("Множитель урона при правильном инструменте")]
    public float correctToolMultiplier = 3f;

    [Tooltip("Множитель урона без правильного инструмента")]
    public float wrongToolMultiplier = 1f;

    [Header("Save")]
    [Tooltip("ID типа ноды для сохранения: 'tree', 'rock'")]
    public string nodeTypeId = "node";

    // =========================================
    //              ГЛАВНЫЙ API
    // =========================================

    /// <summary>
    /// Нанести удар по ноде. Возвращает результат: разрушена или нет.
    /// НЕ выдаёт ресурсы — это делает вызывающий.
    /// </summary>
    public HitResult TakeHit(GameObject attacker, float damage)
    {
        currentHealth -= damage;
        OnHit(attacker, damage);

        if (currentHealth <= 0f)
        {
            ItemData item = dropItem;
            int amount = amountPerHit;

            // ЗАПИСЬ В РЕЕСТР: эта нода уничтожена
            DestroyedNodesRegistry.Add(nodeTypeId, transform.position);

            OnDepleted(attacker);
            Destroy(gameObject);

            return HitResult.Destroyed(item, amount);
        }

        return HitResult.NotDestroyed();
    }

    // =========================================
    //      ЛОГИКА ДЛЯ ИГРОКА (через Interact)
    // =========================================

    public override void Interact(GameObject interactor)
    {
        float damage = GetPlayerDamage(interactor);
        var result = TakeHit(interactor, damage);

        // Игрок сам выдаёт себе дроп в свой Inventory
        if (result.destroyed && result.dropItem != null)
        {
            var inv = interactor.GetComponent<Inventory>();
            if (inv != null)
            {
                inv.AddItem(result.dropItem, result.dropAmount);
                Debug.Log($"Получено: {result.dropItem.displayName} x{result.dropAmount}");
            }
        }
    }

    /// <summary>
    /// Считает урон для игрока с учётом экипированного инструмента.
    /// </summary>
    float GetPlayerDamage(GameObject interactor)
    {
        if (requiredTool == null)
            return amountPerHit;

        var equipment = interactor.GetComponent<EquipmentSlot>();

        if (equipment == null)
            return amountPerHit * wrongToolMultiplier;

        if (equipment.IsEquipped(requiredTool))
            return amountPerHit * correctToolMultiplier;

        return amountPerHit * wrongToolMultiplier;
    }

    // =========================================
    //            БАЗОВЫЕ МЕТОДЫ
    // =========================================

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    protected virtual void OnHit(GameObject attacker, float damage)
    {
        Debug.Log($"{displayName}: удар ({damage}). HP = {currentHealth}");
    }

    protected virtual void OnDepleted(GameObject attacker)
    {
        // Ничего не делаем — это хук для наследников.
        // Выдачу ресурса делает Interact (для игрока) или ChopTree (для NPC).
    }
}
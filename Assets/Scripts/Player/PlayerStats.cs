using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Hunger")]
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerDrainRate = 0.5f;

    [Header("Thirst")]
    public float maxThirst = 100f;
    public float currentThirst;
    public float thirstDrainRate = 0.7f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 8f;
    public float staminaRegenDelay = 1.2f;

    public bool IsDead => currentHealth <= 0f;

    private float lastStaminaUseTime;

    public event Action OnStatsChanged;

    void Start()
    {
        currentHealth  = maxHealth;
        currentHunger  = maxHunger;
        currentThirst  = maxThirst;
        currentStamina = maxStamina;
        OnStatsChanged?.Invoke();
    }

    void Update()
    {
        if (IsDead) return;

        if (currentHunger > 0f) currentHunger -= hungerDrainRate * Time.deltaTime;
        if (currentThirst > 0f) currentThirst -= thirstDrainRate * Time.deltaTime;

        if (Time.time - lastStaminaUseTime > staminaRegenDelay && currentStamina < maxStamina)
            currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);

        if (currentHunger <= 0f || currentThirst <= 0f)
            TakeDamage(1f * Time.deltaTime);

        OnStatsChanged?.Invoke();
    }

    public bool TryUseStamina(float amount)
    {
        if (currentStamina < amount) return false;
        currentStamina -= amount;
        lastStaminaUseTime = Time.time;
        return true;
    }

    public void TakeDamage(float dmg)
    {
        if (IsDead) return;
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0f, maxHealth);
        if (IsDead) Die();
    }

    public void Heal(float amount)  => currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
    public void Eat(float amount)   => currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
    public void Drink(float amount) => currentThirst = Mathf.Clamp(currentThirst + amount, 0f, maxThirst);

    void Die()
    {
        Debug.Log("Игрок умер");
    }
}
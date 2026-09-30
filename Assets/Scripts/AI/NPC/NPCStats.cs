using System;
using UnityEngine;

public class NPCStats : MonoBehaviour
{
    [Header("Hunger")]
    public float maxHunger = 100f;
    public float currentHunger;
    public float hungerDrainRate = 0.3f;

    [Header("Energy")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDrainRate = 0.2f;

    [Header("Thresholds")]
    public float hungryThreshold = 30f;
    public float tiredThreshold = 20f;

    public event Action OnStatsChanged;

    void Start()
    {
        currentHunger = maxHunger;
        currentEnergy = maxEnergy;
        OnStatsChanged?.Invoke();
    }

    void Update()
    {
        if (currentHunger > 0f)
            currentHunger -= hungerDrainRate * Time.deltaTime;

        if (currentEnergy > 0f)
            currentEnergy -= energyDrainRate * Time.deltaTime;

        OnStatsChanged?.Invoke();
    }

    public bool IsHungry() => currentHunger < hungryThreshold;
    public bool IsTired() => currentEnergy < tiredThreshold;

    public void Eat(float amount)
    {
        currentHunger = Mathf.Clamp(currentHunger + amount, 0f, maxHunger);
    }

    public void Sleep(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
    }
    public void ApplySaveData(float hunger, float energy)
    {
    currentHunger = Mathf.Clamp(hunger, 0f, maxHunger);
    currentEnergy = Mathf.Clamp(energy, 0f, maxEnergy);
    OnStatsChanged?.Invoke();
    }
}
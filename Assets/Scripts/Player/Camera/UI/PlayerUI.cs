using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Bars (0..1)")]
    public Slider healthBar;
    public Slider hungerBar;
    public Slider thirstBar;
    public Slider staminaBar;

    private PlayerStats stats;

    void Start()
    {
       stats = FindFirstObjectByType<PlayerStats>();
        if (!stats) { Debug.LogError("PlayerStats not found"); return; }
        stats.OnStatsChanged += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        if (stats) stats.OnStatsChanged -= Refresh;
    }

    void Refresh()
    {
        healthBar.value  = stats.currentHealth  / stats.maxHealth;
        hungerBar.value  = stats.currentHunger  / stats.maxHunger;
        thirstBar.value  = stats.currentThirst  / stats.maxThirst;
        staminaBar.value = stats.currentStamina / stats.maxStamina;
    }
}
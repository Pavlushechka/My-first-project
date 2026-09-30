using UnityEngine;
using UnityEngine.InputSystem;

public class SaveTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("[SaveTest] Start — компонент работает");
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            Debug.Log("[SaveTest] F5 нажата — сохраняем");

            var data = new SaveData();
            data.player.posX = 5.2f;
            data.player.posY = 0.1f;
            data.player.posZ = 3.7f;
            data.inventory.Add(new ItemStackSaveData { itemId = "wood", amount = 40 });
            data.inventory.Add(new ItemStackSaveData { itemId = "stone", amount = 20 });

            SaveSystem.Save(data, 1);
        }

        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            Debug.Log("[SaveTest] F9 нажата — загружаем");

            var data = SaveSystem.Load(1);
            if (data != null)
            {
                Debug.Log($"Позиция игрока: ({data.player.posX}, {data.player.posY}, {data.player.posZ})");
                Debug.Log($"Инвентарь: {data.inventory.Count} стаков");
                foreach (var stack in data.inventory)
                    Debug.Log($"  {stack.itemId} x{stack.amount}");
            }
            else
            {
                Debug.LogWarning("[SaveTest] Сохранение не найдено или битое");
            }
        }
    }
}
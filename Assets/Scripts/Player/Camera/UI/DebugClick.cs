using UnityEngine;
using UnityEngine.EventSystems;

public class DebugClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"=== КЛИК ПО {gameObject.name}! ===");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"--- Мышь вошла в {gameObject.name} ---");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"--- Мышь вышла из {gameObject.name} ---");
    }
}
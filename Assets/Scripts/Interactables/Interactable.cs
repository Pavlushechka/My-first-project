using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string displayName = "Объект";

    public abstract void Interact(GameObject interactor);
}
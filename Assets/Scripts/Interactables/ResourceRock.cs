using UnityEngine;

public class ResourceRock : ResourceNode
{
    void Reset()
    {
        displayName = "Камень";
        maxHealth = 100f;
        amountPerHit = 5;
        correctToolMultiplier = 3f;
        wrongToolMultiplier = 1f;
        nodeTypeId = "rock";   // ← добавили
    }
}
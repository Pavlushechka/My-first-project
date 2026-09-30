using UnityEngine;

public class ResourceTree : ResourceNode
{
    void Reset()
    {
        displayName = "Дерево";
        maxHealth = 50f;
        amountPerHit = 10;
        correctToolMultiplier = 3f;
        wrongToolMultiplier = 1f;
        nodeTypeId = "tree";   // ← добавили
    }
}
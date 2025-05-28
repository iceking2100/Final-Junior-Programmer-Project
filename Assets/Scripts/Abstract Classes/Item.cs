using UnityEngine;

public abstract class Item : MonoBehaviour
{
    string ItemName;
    Sprite ItemSprite;

    public abstract void OnCollected(Player player);
}

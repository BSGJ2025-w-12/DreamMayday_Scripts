using UnityEngine;

public class SetOrderInLayer : MonoBehaviour
{
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = -(int)transform.position.y;
    }

}

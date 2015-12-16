using UnityEngine;
using System.Collections;

/// <summary>
/// Unity doesn't auto-scale colliders that are on uGUI objects, so this will help with resizing the colliders.
/// This only works with box colliders though.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class ColliderScaler : MonoBehaviour
{
	void Start ()
    {
        Resize();
    }

    public void Resize()
    {
        RectTransform rectForm = GetComponent<RectTransform>();
        BoxCollider2D c = GetComponent<BoxCollider2D>();
        c.size = rectForm.rect.size;
        c.offset = new Vector2(c.size.x * (rectForm.pivot.x - 0.5f), c.size.y * (0.5f - rectForm.pivot.y));
    }
}

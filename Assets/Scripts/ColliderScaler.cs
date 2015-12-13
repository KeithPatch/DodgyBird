using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class ColliderScaler : MonoBehaviour
{
	void Start ()
    {
        RectTransform rectForm = GetComponent<RectTransform>();
        BoxCollider2D c = GetComponent<BoxCollider2D>();
        c.size = rectForm.rect.size;
    }
}

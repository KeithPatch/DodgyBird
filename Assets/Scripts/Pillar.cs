using UnityEngine;
using System.Collections;

/// <summary>
/// The pillar class does little... It's purpose is to resize the image and make sure the collider matches
/// </summary>
public class Pillar : ImageHelper
{
    public void SetHeight(float height)
    {
        cachedRectForm.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        ColliderScaler scaler = GetComponent<ColliderScaler>();//might be worth caching this
        scaler.Resize();
    }
}

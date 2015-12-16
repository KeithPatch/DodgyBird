using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// a raycasting system intended to watch for the player passing by
/// </summary>
public class Scorecaster : CachedBehavior
{
    public LayerMask m_Layer;               //physics layer we're raycasting against
    public UnityEvent OnScoreEvent = null;  //called when an object on the specified layer has been hit with the raycast

    public float Length = 100f;

    void FixedUpdate()
    {
        if (OnScoreEvent == null)
            return;

        RaycastHit2D hit = Physics2D.Raycast(cachedTransform.position, Vector2.up, Length,  m_Layer.value);
        if (hit.collider != null)
            OnScoreEvent.Invoke();
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * Length);
    }
}

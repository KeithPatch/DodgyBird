using UnityEngine;
using System.Collections;

/// <summary>
/// Not much to explain here, the camera will follow the player with an offset.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollower : CachedBehavior
{
    public Transform m_ToFollow = null;
    public Vector2 m_FollowOffset = Vector2.zero;

    public bool m_FollowX;
    public bool m_FollowY;

    void Update()
    {
        if (m_ToFollow == null)
            return;

        Vector3 position = cachedTransform.position;
        if (m_FollowX == true)
            position.x = m_ToFollow.position.x + m_FollowOffset.x;

        if (m_FollowY == true)
            position.y = m_ToFollow.position.y + m_FollowOffset.y;

        cachedTransform.position = position;
    }
}

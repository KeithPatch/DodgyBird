using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// the turret class sits at the end of a pillar and fires projectiles horizontally
/// </summary>
public class Turret : CachedBehavior
{
    private float m_RemainingCooldown = 0f; //the time remaining before we can fire another projectile

    public float m_ShootCooldown = 1f;      //the delay between firing projectiles
    public Image m_VisualComponent = null;  //the turret's visual component, we'll be adding some visual queues so the player will know something is about to happen

    public delegate void ShootFunction(Vector2 spawnPosition, Vector2 velocity);
    public ShootFunction Shoot;             //called when we wish to fire a projectile. This way the turret isn't responsible for instantiation of objects

    void FixedUpdate()
    {
        m_RemainingCooldown -= Time.fixedDeltaTime;

        //let's give the user a visual queue that we're nearly ready to fire
        if(m_VisualComponent != null)
            m_VisualComponent.color = Color.Lerp(Color.red, Color.white, m_RemainingCooldown / m_ShootCooldown);

        if (m_RemainingCooldown > 0f)
            return;
        m_RemainingCooldown = m_ShootCooldown;

        //we're using a linear (horizontal) check for the player. We want to see if the player is in the line of fire.
        //Think bullet bills from Mario
        float firingRangeX = Core.Instance.CanvasRoot.sizeDelta.x * Core.Instance.CanvasRoot.localScale.x;

        float x = Mathf.Abs(cachedTransform.position.x - Core.Instance.Player.cachedRectForm.position.x);

        if (x < firingRangeX)
        {
            if (Shoot != null)
                Shoot(cachedTransform.position, new Vector3(-1f, 0f, 0f));
        }
    }

}

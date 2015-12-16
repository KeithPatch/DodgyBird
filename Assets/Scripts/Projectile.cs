using UnityEngine;
using System.Collections;

/// <summary>
/// The projectile class is a simple bullet. It has a limited lifespan and moves at a fixed speed
/// </summary>
public class Projectile : CachedBehavior, IMovingEntity
{
    public delegate void DestroyFunction(Projectile self);
    public DestroyFunction OnDestroyed;         //Called when the projectile collides with an object or when it's lifespan has run out

    private Rigidbody2D m_RigidBody2D;          //simply caches the rigidbody for easy access
    private Coroutine m_LifeSpanRoutine;        //the coroutine used to destroy the projectile when it runs out of time... we track this so we can stop the coroutine early if needed

    [SerializeField]
    private float m_Force = 3f;                 //the speed of the projectile

    [SerializeField]
    private float m_LifeSpan = 1f;              //how long the projectile will remain alive

    public float lifeSpan
    {
        get { return m_LifeSpan; }
        set { m_LifeSpan = value; }
    }

    public Rigidbody2D rigidBody2D
    {
        get { return m_RigidBody2D; }
        set { m_RigidBody2D = value; }
    }

    public Vector2 velocity
    {
        get { return rigidBody2D.velocity; }
        set { rigidBody2D.velocity = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        m_RigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void SetVelocity(Vector2 velocity)
    {
        rigidBody2D.velocity = velocity;
    }

    public void SetDirection(Vector2 direction)
    {
        rigidBody2D.velocity = direction.normalized * m_Force;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleDestroy();
    }

    IEnumerator DelayedDestroy(float lifeSpan)
    {
        yield return new WaitForSeconds(lifeSpan);
        HandleDestroy();
    }

    public void HandleDestroy()
    {
        if(m_LifeSpanRoutine != null)
            StopCoroutine(m_LifeSpanRoutine);
        if (OnDestroyed != null)
            OnDestroyed(this);
    }

    public void SetLifeSpan(float lifeSpan)
    {
        m_LifeSpanRoutine = StartCoroutine(DelayedDestroy(lifeSpan));
    }

    public void SetLifeSpan()
    {
        m_LifeSpanRoutine = StartCoroutine(DelayedDestroy(lifeSpan));
    }
}

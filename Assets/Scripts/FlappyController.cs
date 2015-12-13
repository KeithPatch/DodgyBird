using UnityEngine;
using System.Collections;

public class FlappyController : CachedBehavior
{
    Rigidbody2D m_RigidBody;
    public float m_Force = 0f;
    public ForceMode2D m_ForceMode = ForceMode2D.Force;

    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
    }
	
	void Update()
    {
        HandleInput();
        UpdateRotation();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) == true)
            Flap();
    }

    void Flap()
    {
        Vector2 velocity = m_RigidBody.velocity;
        velocity.y = 0f;
        m_RigidBody.velocity = velocity;
        m_RigidBody.AddForce(Vector2.up * m_Force, m_ForceMode);
    }

    void UpdateRotation()
    {
        float lerp = (m_RigidBody.velocity.y) / Physics2D.gravity.y;
        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -90f, lerp));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Die();
    }

    void Die()
    {
        Debug.Log("<color=red>On Death</color>");
    }
}

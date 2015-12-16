using UnityEngine;


/// <summary>
/// Our basic character controller. Handles user input
/// </summary>
public class FlappyController : CachedBehavior, IMovingEntity
{

    public KeyCode m_ActionKey = KeyCode.Mouse0;                //press this key to flap
    public float m_Force = 0f;                                  //The force to be applied when you flap
    public ForceMode2D m_ForceMode = ForceMode2D.Force;         //adjustable forcemode, mostly exists because it was convenient for testing
    public float m_HorizontalForce = 3f;                        //the constant horizontal speed of the bird

    //we want to be able to tie additional (external) functionality to the player's death
    public delegate void DeathFunction();
    public DeathFunction OnDeath;

    private bool m_InputEnabled = false;                        //allows us to enable/disable user input
    private Rigidbody2D m_RigidBody2D;                          //cache the rigidbody2D so that we can easily access it

    public Rigidbody2D rigidBody2D
    {
        get { return m_RigidBody2D; }
        set { m_RigidBody2D = value; }
    }

    //makes it easier to adjust the velocity of the player, basically truncates the original method
    public Vector2 velocity
    {
        get { return rigidBody2D.velocity; }
        set { rigidBody2D.velocity = value; }
    }

    public void SetVelocity(Vector2 velocity)
    {
        rigidBody2D.velocity = velocity;
    }

    protected override void Awake()
    {
        base.Awake();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void StartMoving()
    {
        rigidBody2D.velocity = new Vector2(m_HorizontalForce, 0f);
    }
	
	void Update()
    {
        HandleInput();
        UpdateRotation();
    }

    void HandleInput()
    {
        if (m_InputEnabled == false)
            return;
        if (Input.GetKeyDown(m_ActionKey) == true)
            Flap();
    }

    /// <summary>
    /// negate vertical velocity and apply force.
    /// </summary>
    void Flap()
    {
        Vector2 tempVelocity = velocity;
        tempVelocity.y = 0f;
        rigidBody2D.velocity = tempVelocity;
        rigidBody2D.AddForce(Vector2.up * m_Force, m_ForceMode);
    }

    /// <summary>
    /// updates the bird's rotation based on their vertical velocity
    /// </summary>
    void UpdateRotation()
    {
        float gravity = Mathf.Min(-0.01f, Physics2D.gravity.y);//just to make sure we won't have a div-by-zero
        float lerp = (rigidBody2D.velocity.y) / gravity;
        cachedTransform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -90f, lerp));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Die();
    }

    void Die()
    {
        m_InputEnabled = false;
        Debug.Log("<color=red>On Death</color>");
        if (OnDeath != null)
            OnDeath();
    }

    public void EnableInput()
    {
        m_InputEnabled = true;
    }

    public void DisableInput()
    {
        m_InputEnabled = false;
    }
}

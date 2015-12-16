using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// The Obstacle Manager is a heavy lifter intended to create, store, and reuse the obstacles within the game. 
/// </summary>
public class ObstacleManager : CachedBehavior
{
    private Queue<Projectile> m_BulletPool = new Queue<Projectile>();       //stores the unused bullets
    private Queue<Pillar> m_PillarQueue = new Queue<Pillar>();              //we use this for checking where pillars should be placed
    private Pillar[] m_PillarContainer;                                     //inactive pillars will be stored here
    private List<Projectile> m_BulletContainer = null;                      //let's us track all instantiated bullets...
    private float m_PillarOffset = 0f;                                      //the offset of the next pillar to be created (from origin)
    private Coroutine m_DelayedStartRoutine;                                //simply stores out coroutine so that we can stop it if the player dies before the delayed start is even finished

    public Pillar pillarPrefab;                                             //We'll be using this to instantiate our pillars
    public Projectile projectilePrefab;                                     //We'll be using this to instantiate our bullets

    public float m_PillarSpacing = 3f;                                      //the world space distance between pillar placement
    public int m_PillarCount = 5;                                           //the number of pillars that we wish to keep in memory (a dynamic approach would be overkill)
    public int m_BulletCount = 10;                                          //The number of bullets we wish to instantiate and pool... we can generate more as needed

    public RectTransform m_ObstacleAnchor;                                  //the parent object for our newly instantiate obstacles
    public Transform m_ScoringLine = null;                                  //used to scan for the player and provide score when the user passes a pillar
    
    protected override void Awake()
    {
        base.Awake();
        //instantiate pillars, turrets, and bullets
        //pillars and turrets will get respawned as they exit the screen

        m_PillarContainer = new Pillar[m_PillarCount];
        for (int i = 0; i < m_PillarCount; i++)
            m_PillarContainer[i] = CreatePillar();

        m_BulletContainer = new List<Projectile>(m_BulletCount);
        for(int i = 0; i < m_BulletCount; i++)
        {
            Projectile bullet = MakeBullet();
            bullet.HandleDestroy();
            m_BulletContainer.Add(bullet);
        }
    }

    /// <summary>
    /// delays the spawning of pillars/turrets
    /// </summary>
    /// <param name="seconds">the number of seconds to delay for</param>
    public void BeginDelayed(float seconds)
    {
        m_DelayedStartRoutine = StartCoroutine(delayedStart(seconds));
    }

    IEnumerator delayedStart(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        RectTransform canvasRoot = Core.Instance.CanvasRoot;
        float offset = (Camera.main.transform.position.x * 1f / canvasRoot.localScale.x + canvasRoot.sizeDelta.x * 0.5f);
        EnablePillars(offset);
    }

    /// <summary>
    /// Enables the placement of pillars/turrets
    /// </summary>
    /// <param name="pillarOffset">the initial offset to start placing pillars at</param>
    public void EnablePillars(float pillarOffset)
    {
        m_ScoringLine.gameObject.SetActive(true);
        m_PillarOffset = pillarOffset;
        m_PillarQueue.Clear();
        //enable pillar spawning
        for (int i = 0; i < m_PillarCount; i++)
        {
            PlacePillar(m_PillarContainer[i]);
            m_PillarQueue.Enqueue(m_PillarContainer[i]);
            m_PillarContainer[i].gameObject.SetActive(true);
        }

        Vector3 position = m_ScoringLine.position;
        position.x = m_PillarContainer[0].position.x;
        ResetScoreLine(position); //we anchor the scoring mechanism to the initial offset... it'll step the appropriate distance on its own
    }

    /// <summary>
    /// prevent pillars from spawn and disables the scoring mechanism
    /// </summary>
    public void DisablePillars()
    {
        m_ScoringLine.gameObject.SetActive(false);
        if (m_DelayedStartRoutine != null)
            StopCoroutine(m_DelayedStartRoutine);
        //disable pillar spawning for now
        m_PillarQueue.Clear();
    }

    /// <summary>
    /// This let's us remove the pillars so that the player has no obstacles in the way while flying in the main menu
    /// </summary>
    public void HidePillars()
    {
        DisablePillars();
        for (int i = 0; i < m_PillarCount; i++)
            m_PillarContainer[i].gameObject.SetActive(false);

        for (int i = 0; i < m_BulletContainer.Count; i++)
        {
            if (m_BulletContainer[i].isActiveAndEnabled == true)
                m_BulletContainer[i].HandleDestroy();
        }
    }

    void FixedUpdate()
    {
        if (m_PillarQueue.Count == 0)
            return;

        Pillar currentPillar = m_PillarQueue.Peek();
        currentPillar.UpdateCorners();
        //if currentPillar is off screen
        if (currentPillar.isVisible(Camera.main) == false)
        {
            m_PillarQueue.Dequeue();                //remove the pillar from the queue
            PlacePillar(currentPillar);             //place the pillar at the end of the line
            m_PillarQueue.Enqueue(currentPillar);   //put the pillar back into the queue so it gets another turn here
        }
    }

    /// <summary>
    /// Creates a pillar, it's expected that the caller will store the pillar
    /// This will also set up some of the turret's functionality (shooting)
    /// </summary>
    /// <returns>the new created pillar</returns>
    private Pillar CreatePillar()
    {        
        //create and store pillars
        Pillar pillar = Instantiate(pillarPrefab, Vector3.zero, Quaternion.identity) as Pillar;
        pillar.cachedTransform.SetParent(m_ObstacleAnchor);
        pillar.cachedTransform.localScale = Vector3.one;
        pillar.gameObject.SetActive(false);

        Turret t = pillar.GetComponentInChildren<Turret>();
        t.Shoot += FireProjectile;
        return pillar;
    }

    /// <summary>
    /// Places the provided pillar in the scene with a randomized height. The pillar will be anchored to the floor or the ceiling
    /// The function will also offset the turret based on the position/height of the pillar
    /// </summary>
    /// <param name="p">the pillar to be placed</param>
    private void PlacePillar(Pillar p)
    {
        //if end point is above Screen.height * 0.5f, then the pillar goes from ceiling to floor
        //if end point is less than Screen.height * 0.5f, then the pillar goes from floor to ceiling
        RectTransform canvasRoot = Core.Instance.CanvasRoot;
        float edgeOffset = canvasRoot.sizeDelta.y * 0.2f;
        float endPoint = Random.Range(canvasRoot.sizeDelta.y * -0.5f + edgeOffset, canvasRoot.sizeDelta.y * 0.5f - edgeOffset);
        float height = canvasRoot.sizeDelta.y * 0.5f;
        float sign = 1f;

        if (endPoint > 0f)
        {
            p.cachedRectForm.pivot = new Vector2(0.5f, 0f);
            height = height - endPoint;
            sign = -1f;
        }
        else
        {
            p.cachedRectForm.pivot = new Vector2(0.5f, 1f);
            height = height + endPoint;
        }


        p.cachedTransform.position = new Vector3(m_PillarOffset * canvasRoot.localScale.x, (endPoint) * canvasRoot.localScale.y, 0f);
        p.SetHeight(height);

        //get the turret and adjust its offset based on how we're anchoring the pillar (ceiling or floor)
        Turret t = p.GetComponentInChildren<Turret>();
        if(t != null)
            t.cachedRectForm.position = p.position + new Vector3(0f, t.cachedRectForm.sizeDelta.y * canvasRoot.localScale.y * 0.5f * sign, 0f);

        m_PillarOffset += m_PillarSpacing;
    }

    /// <summary>
    /// Instantiates a copy of the projectile prefab and sets up the return functionality for when the bullet has finished it's lifespan
    /// </summary>
    /// <returns>the instantiated bullet</returns>
    private Projectile MakeBullet()
    {
        Projectile bullet = Instantiate(projectilePrefab, cachedTransform.position, Quaternion.identity) as Projectile;
        bullet.cachedTransform.SetParent(m_ObstacleAnchor);
        bullet.cachedTransform.localScale = Vector3.one;
        bullet.OnDestroyed += ReturnBullet;
        return bullet;
    }

    /// <summary>
    /// Gets a bullet out of the pool or instanties a copy of the prefab (by calling MakeBullet)
    /// </summary>
    /// <returns>the bullet you were looking for</returns>
    private Projectile GetBullet()
    {
        if (m_BulletPool.Count > 0)
            return m_BulletPool.Dequeue();

        return MakeBullet();
    }

    /// <summary>
    /// Gets or Makes are bullet, sets the bullet's position and shoots it off in the specified direction
    /// </summary>
    /// <param name="spawnPosition">the position to start the bullet</param>
    /// <param name="direction">the direction for the bullet to travel</param>
    private void FireProjectile(Vector2 spawnPosition, Vector2 direction)
    {
        //fire bullet
        Projectile bullet = GetBullet();
        bullet.cachedTransform.position = spawnPosition;
        bullet.gameObject.SetActive(true);
        bullet.SetDirection(direction);
        bullet.SetLifeSpan();
    }

    /// <summary>
    /// Puts the bullet back in the queue... also hides the bullet and negates its velocity
    /// </summary>
    /// <param name="b">the bullet to be pooled</param>
    private void ReturnBullet(Projectile b)
    {
        b.rigidBody2D.velocity = Vector2.zero;
        b.gameObject.SetActive(false);
        m_BulletPool.Enqueue(b);
    }

    /// <summary>
    /// Called when the scoreline catches the player passing a pillar... it'll just move the scoreline to the next pillar
    /// </summary>
    public void OnScore()
    {
        ResetScoreLine(m_ScoringLine.position + new Vector3(m_PillarSpacing * 0.01f, 0f, 0f));
    }

    /// <summary>
    /// Moves the scoreline to the specified position
    /// </summary>
    /// <param name="position">the scorelines new position</param>
    private void ResetScoreLine(Vector3 position)
    {
        m_ScoringLine.position = position;
    }

    void OnDrawGizmos()
    {
        //some debuf info to help me with the spawning of pillars
        Gizmos.color = Color.blue;
        Canvas c = GetComponentInParent<Canvas>();
        RectTransform rectForm = c.GetComponent<RectTransform>();
        float edgeOffset = rectForm.sizeDelta.y * 0.2f;

        Gizmos.DrawLine(new Vector3(0f, rectForm.sizeDelta.y * 0.5f - edgeOffset, 0f) * 0.01f, Vector3.zero);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(0f, rectForm.sizeDelta.y * 0.5f - edgeOffset, 0f) * 0.01f, new Vector3(0f, rectForm.sizeDelta.y * 0.5f, 0f) * 0.01f);
    }
}

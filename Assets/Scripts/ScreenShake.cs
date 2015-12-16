using UnityEngine;
using System.Collections;

/// <summary>
/// An unused attempt at creating a decent looking screenshake (it didn't look decent enough).
/// </summary>
public class ScreenShake : CachedBehavior
{
    public float m_Intensity = 1f;
    public float m_Duration = 1f;
    private Vector3 m_InitialPosition = Vector3.one;

    protected override void Awake()
    {
        base.Awake();
        m_InitialPosition = cachedTransform.position;
    }

    public void Shake()
    {
        StartCoroutine(ProcessShake());
    }

    IEnumerator ProcessShake()
    {
        float startTime = Time.time;
        float delta = 0f;

        do
        {
            delta = Time.time - startTime;
            cachedTransform.Translate(m_Intensity * Random.insideUnitSphere);
            yield return null;
        } while (delta < m_Duration);

        cachedTransform.position = m_InitialPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Shake();
        
    }
}

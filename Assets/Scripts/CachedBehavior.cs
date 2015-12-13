using UnityEngine;

public class CachedBehavior : MonoBehaviour
{
    private Transform m_CachedTransform;
    private RectTransform m_CachedRectForm;

    public Transform cachedTransform
    {
        get { return m_CachedTransform; }
        set { m_CachedTransform = value; }
    }

    public RectTransform cachedRectForm
    {
        get { return m_CachedRectForm; }
        set { m_CachedRectForm = value; }
    }


    void Awake()
    {
        m_CachedTransform = GetComponent<Transform>();
        m_CachedRectForm = GetComponent<RectTransform>();
    }

	
}

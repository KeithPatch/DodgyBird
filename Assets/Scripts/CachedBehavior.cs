using UnityEngine;

/// <summary>
/// The class is simply meant to make for easy access to commonly accessed elements.
/// Under normally circumstances Unity will call GetComponent for these things.
/// </summary>
public class CachedBehavior : MonoBehaviour
{
    private Transform m_CachedTransform;
    private RectTransform m_CachedRectForm;
    private GameObject m_CachedGameObject;

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

    public GameObject cachedGameObject
    {
        get { return m_CachedGameObject; }
        set { m_CachedGameObject = value; }
    }

    protected virtual void Awake()
    {
        m_CachedTransform = GetComponent<Transform>();
        m_CachedRectForm = GetComponent<RectTransform>();
        m_CachedGameObject = GetComponent<GameObject>();
    }

	
}

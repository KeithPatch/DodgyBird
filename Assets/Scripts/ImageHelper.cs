using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple helper class designed to work with images and help us calculate the dimensions and coordinates of the image in world-space
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageHelper : CachedBehavior
{
    private Vector3[] m_WorldCorners;       //The corners of the uGUI item in world space
    protected bool isDirty = false;         //we can set this when we update the entity

    //returns the corners of the object in world space
    //if the object has been marked as dirty, this will update the corners
    public Vector3[] worldCorners
    {
        get
        {
            if (isDirty == true)
                UpdateCorners();
            return m_WorldCorners;
        }
        private set { m_WorldCorners = value; }
    }

    //uses the world-space corners to calculate the width of the object
    public float width
    {
        get
        {
            return worldCorners[3].x - worldCorners[1].x;
        }
    }

    //uses the world-space corners to calculate the height of the object
    public float height
    {
        get
        {
            return worldCorners[1].y - worldCorners[0].y;
        }
    }

    //Easy access of the object's position
    public Vector3 position
    {
        get
        {
            return cachedRectForm.position;
        }
        set
        {
            cachedRectForm.position = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        m_WorldCorners = new Vector3[4];
        UpdateCorners();
    }

    public void UpdateCorners()
    {
        cachedRectForm.GetWorldCorners(m_WorldCorners);
        isDirty = false;
    }

    //for this project we're only gonna check the left side of the screen
    //this method assumes that the image is not rotated
    public bool isVisible(Camera viewer)
    {
        Vector3 screenPoint = viewer.ScreenToWorldPoint(Vector3.zero);
        return worldCorners[3].x > screenPoint.x;
    }

    public void MarkAsDirty()
    {
        isDirty = true;
    }
}

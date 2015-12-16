using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Leapfrogs scenic objects so that we have a continuously scrolling background
/// </summary>
public class SceneryManager : MonoBehaviour
{
    public ImageHelper[] m_Scenery;                                         //the scenic items themselves
    public Vector3 m_Offset = Vector3.zero;                                 //the initial offset for scenery placement
    private Vector3 m_CurrentOffset = Vector3.zero;                         //The current offset to place the next peice of scenery at
    private Queue<ImageHelper> m_ScenicQueue = new Queue<ImageHelper>();    //it's worth noting that we could use the priority queue if we wanted a system that was generating scenery that needed to be inserted


    void FixedUpdate()
    {
        ImageHelper first = null;
        if (m_ScenicQueue.Count > 0)
        {
            first = m_ScenicQueue.Peek();
            //check to see if the item is still on screen
            if (first.isVisible(Camera.main) == false)
            {
                //it's no longer on screen, so let's leapfrog it to the end of the queue
                m_ScenicQueue.Dequeue();
                Push(first);
            }
        }
    }

    /// <summary>
    /// Queues the item up after setting it's position to fit at the end of the existing scenery
    /// </summary>
    /// <param name="piece">the scenic item we wish to place at the end of our existing scenery</param>
    void Push(ImageHelper piece)
    {
        piece.position = m_CurrentOffset;
        piece.UpdateCorners();
        m_CurrentOffset.x += piece.width;
        m_ScenicQueue.Enqueue(piece);
    }

    /// <summary>
    /// reset the offset and put everything back to its original positions. We use this when going from Game Over to a new playthrough
    /// </summary>
    public void ResetScenery()
    {
        m_ScenicQueue.Clear();
        m_CurrentOffset = m_Offset;
        for (int i = 0; i < m_Scenery.Length; i++)
            Push(m_Scenery[i]);
    }
}

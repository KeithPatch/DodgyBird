using UnityEngine;

/// <summary>
/// The following code is based on https://youtu.be/HM17mAmLd7k
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    public enum ScalingMode { PixelDensity, Width }
    public ScalingMode m_ScalingMode = ScalingMode.PixelDensity;

    public float m_PixelsToUnits = 100f;
    public float m_TargetWidth = 720;

    private Camera m_Camera;

    void Awake()
    {
        m_Camera = GetComponent<Camera>();
        UpdateScale();
    }

    void UpdateScale()
    {
        int height = Screen.height;
        switch(m_ScalingMode)
        {
            case ScalingMode.PixelDensity:
                break;
            case ScalingMode.Width:
                height = Mathf.RoundToInt(m_TargetWidth / (float)Screen.width * Screen.height);
                break;
        }
        m_Camera.orthographicSize = height / m_PixelsToUnits * 0.5f;
    }

    void Update()
    {
#if UNITY_EDITOR
        UpdateScale();
#endif
    }
}

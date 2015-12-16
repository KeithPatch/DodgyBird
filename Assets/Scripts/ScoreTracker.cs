using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simply tracks the score and updates the on-screen text
/// </summary>
public class ScoreTracker : MonoBehaviour {

    private int m_Score = 0;
    public Text m_ScoreDisplay = null;

    public void ResetScore()
    {
        m_Score = 0;
        UpdateDisplay();
    }

    public void IncrementScore()
    {
        m_Score++;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        m_ScoreDisplay.text = m_Score.ToString();
    }
}

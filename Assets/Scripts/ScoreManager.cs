using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;    // Reference to this manager
    
    [SerializeField]
    private Text _scoreText;                // UI text of current score
    
    private int _score;                     // Score variable

    private void Awake()
    {
        Instance = this;                    // Set reference 
        _score = 0;                         // Set default score
        UpdateScore();                      // Set default score on UI text object
    }

    public void IncreaseAndUpdateScore()
    {
        _score++;                            // Increment score
        UpdateScore();                       // Update score on screen
    }

    private void UpdateScore()
    {
        _scoreText.text = "SCORE: " + _score; // Update score UI text
    }
}
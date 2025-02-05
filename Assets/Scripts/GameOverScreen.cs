using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverPanel; // Reference to the game over UI panel
    public TextMeshProUGUI gameOverScoreText; // Reference to the score text
    public Button restartButton; // Reference to the restart button
    public PlayerMovement player;

    void Start()
    {
        gameOverPanel.SetActive(false); // Ensure the game over screen is hidden at start
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowGameOverScreen()
    {
        // Get the score from JumpOverGoomba
        JumpOverGoomba jumpOverGoomba = FindObjectOfType<JumpOverGoomba>();
        if (jumpOverGoomba != null)
        {
            gameOverScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
        }

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    void RestartGame()
    {
        gameOverPanel.SetActive(false); // Hide the game over screen
        Time.timeScale = 1.0f; // Resume time

        player.RestartButtonCallback(0); // Reset game using PlayerMovement reset function

        // Ensure score is reset properly
        JumpOverGoomba jumpOverGoomba = player.GetComponent<JumpOverGoomba>();
        if (jumpOverGoomba != null)
        {
            jumpOverGoomba.score = 0; // Reset the score
            jumpOverGoomba.scoreText.text = "Score: 0"; // Update the UI
        }
    }

}

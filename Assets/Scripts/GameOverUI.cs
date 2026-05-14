using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverCanvas;

    void Start()
    {
        gameOverCanvas.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
    }
}
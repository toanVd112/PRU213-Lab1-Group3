using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject huongDanPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("game");
    }
    public void ShowHuongDan()
    {
        menuPanel.SetActive(false);
        huongDanPanel.SetActive(true);
    }

    public void CloseHuongDan()
    {
        huongDanPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class DwButtonHandler : MonoBehaviour
{
    public void playAgain()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1f;
    }

    public void returnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
}

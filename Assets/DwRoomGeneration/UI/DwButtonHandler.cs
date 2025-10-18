using UnityEngine;
using UnityEngine.SceneManagement;

public class DwButtonHandler : MonoBehaviour
{
    public void continueBtn()
    {
        Time.timeScale = 1f; //continue the time
    }

    public void playAgain()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1f;
    }

    public void returnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }
}

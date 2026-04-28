using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartNewGame()
    {
        if (RunManager.Instance != null)
            RunManager.Instance.StartNewRun();
        else
            SceneManager.LoadScene("CharacterSelectScene"); // fallback for editor testing
    }
}

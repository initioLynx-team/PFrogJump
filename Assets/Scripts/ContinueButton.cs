using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
[SerializeField] private string menuSceneName = "Menu";

    public void GoToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

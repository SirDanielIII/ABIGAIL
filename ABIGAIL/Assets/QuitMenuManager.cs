using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitMenuController : MonoBehaviour
{
    public GameObject quitMenuPanel;
    public SceneManagement SceneManagement;
    public static bool isMenuOpen = false;

    void Start()
    {
        quitMenuPanel.SetActive(false);
    }

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleQuitMenu();
        }
    }

    public void ToggleQuitMenu()
    {
        quitMenuPanel.SetActive(!quitMenuPanel.activeSelf);
        Time.timeScale = quitMenuPanel.activeSelf ? 0 : 1;
        isMenuOpen = quitMenuPanel.activeSelf;
    }

    public void QuitToMainMenu()
    {
        isMenuOpen = false;
        SceneManagement.SaveGameState();
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        isMenuOpen = false;
        SceneManagement.SaveGameState();
        Application.Quit();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0)
        {
            CheckpointManager.Instance?.LoadPlayerState();
        }
    }
}

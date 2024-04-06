using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{

    public SceneManagement SceneManagement;
    public void NewGame()
    {
        int startingSceneIndex = 1;
        ResetGameState();
        PlayerPrefs.SetInt("SavedScene", startingSceneIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(startingSceneIndex, LoadSceneMode.Single);
    }

    private void ResetGameState()
    {
        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("PlayerStamina");
        PlayerPrefs.DeleteKey("CheckpointPositionX");
        PlayerPrefs.DeleteKey("CheckpointPositionY");
        PlayerPrefs.DeleteKey("HasKey");
        

        PlayerPrefs.Save();
    }

    public void ContinueGame()
    {
        int sceneToLoad = PlayerPrefs.GetInt("SavedScene", 1);
        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        SceneManagement.SaveGameState();
        Debug.Log("Game is exiting...");
        Application.Quit();
    }
}

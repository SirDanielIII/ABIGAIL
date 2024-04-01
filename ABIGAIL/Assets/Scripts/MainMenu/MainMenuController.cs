using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        PlayerPrefs.SetInt("SavedScene", 1); // Replace index_of_starting_level with the actual index
        PlayerPrefs.Save();
        SceneManager.LoadScene("Scenes/Cutscenes/Chapel Cutscene");
        
    }

    public void ContinueGame()
    {
        int sceneToLoad = PlayerPrefs.GetInt("SavedScene", 0); // Assuming 0 is your main menu or starting level
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Options()
    {
        SceneManager.LoadScene("Options");
    }
}

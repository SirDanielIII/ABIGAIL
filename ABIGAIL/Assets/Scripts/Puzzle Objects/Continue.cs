using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Continue : MonoBehaviour
{
    public SceneManagement sceneManager;
    public void Advance()
    {
        sceneManager.LoadNextScene();
    }
}

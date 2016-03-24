using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public int sceneNumber;

    public void LoadScene()
    {
        SceneManager.UnloadScene(sceneNumber);
        SceneManager.LoadScene(sceneNumber);
    }
}

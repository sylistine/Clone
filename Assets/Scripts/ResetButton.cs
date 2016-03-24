using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResetButton : MonoBehaviour
{
    public void ReloadLevel()
    {
        SceneManager.UnloadScene(0);
        SceneManager.LoadScene(0);
    }
}

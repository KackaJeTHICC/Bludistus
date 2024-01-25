using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    /// <summary>
    /// Loads a scene
    /// </summary>
    /// <param name="sceneNumber">Number of scene you want to load</param>
    public void LoadLevel(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);
    }
    /// <summary>
    /// Loads a scene
    /// </summary>
    /// <param name="sceneNumber">Name of scene you want to load</param>
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    /// <summary>
    /// Exits the game
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Exiting game...");
#else
        Application.Quit();
#endif
    }
}

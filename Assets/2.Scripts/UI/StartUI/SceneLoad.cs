using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    public static void LoadSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    public static void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}

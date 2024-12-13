using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartCanvas : MonoBehaviour
{
    [SerializeField] private List<Transform> parents = new();

    private IEnumerator Start()
    {
        if (!GameManager.Instance.isInitialized)
        {
            //call Manager Scene
            yield return SceneManager.LoadSceneAsync("DontDestroy", LoadSceneMode.Additive);

            //init all managers + @
            GameManager.Instance.InitApp();
            yield return new WaitUntil(() => GameManager.Instance.isInitialized);
        }

        //set UIManager-Parents
        UIManager.SetParents(parents);

        //wait until "Game Start" input
        yield return new WaitUntil(() => GameManager.isGameStart);

        //load BoardScene
        UIManager.LoadBoardScene();
    }
}

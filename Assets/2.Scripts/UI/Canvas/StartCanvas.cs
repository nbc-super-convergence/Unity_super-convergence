using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartCanvas : MonoBehaviour
{
    [SerializeField] private List<Transform> parents = new List<Transform>();
    public static bool isSet { get; private set; }
    private IEnumerator Start()
    {
        isSet = false;

        //call Managers
        yield return SceneManager.LoadSceneAsync("DontDestroy", LoadSceneMode.Additive);
        //set UIManager-Parents
        UIManager.SetParents(parents);
        isSet = true;

        //init all managers + @
        GameManager.Instance.InitApp();
        yield return new WaitUntil(() => GameManager.Instance.isInitialized);

        //wait until "Game Start" input
        yield return new WaitUntil(() => GameManager.isGameStart);

        //load BoardScene
        UIManager.LoadBoardScene();
    }
}

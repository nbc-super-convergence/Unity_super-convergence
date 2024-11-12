using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartCanvas : MonoBehaviour
{
    [SerializeField] private List<Transform> parents = new List<Transform>();
    
    private IEnumerator Start()
    {
        //call Managers
        yield return SceneManager.LoadSceneAsync("DontDestroy", LoadSceneMode.Additive);
        //set UIManager-Parents
        UIManager.SetParents(parents);
        //init all managers + @
        yield return GameManager.Instance.InitApp();
        
        //wait until "Game Start" input
        yield return new WaitUntil(() => GameManager.isGameStart);
        
        //load lobby
        //SceneManager.LoadSceneAsync("LobbyScene");
    }
}

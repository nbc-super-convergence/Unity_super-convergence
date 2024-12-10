using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGameDropper : MapBase
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private List<GameObject> levels;

    [SerializeField] private float cameraFollowDelay = 0.2f;

    private GameDropperData gameData;
    private int prevPhase;

    private void Start()
    {
        gameData = MinigameManager.Instance.GetMiniGame<GameDropper>().gameData;
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitUntil(() => gameData.phase == 1);
        prevPhase = gameData.phase;

        levels[0].SetActive(false);
        StartCoroutine(MoveCamera());
    }

    public void NextLevelEvent(int[] holes)
    {
        //바닥 없애기.
        foreach (int hole in holes)
        {
            levels[gameData.phase].transform.GetChild(hole).gameObject.SetActive(false);
        }
        gameData.phase++;
        StartCoroutine(MoveCamera());
    }

    private IEnumerator MoveCamera()
    {
        yield return new WaitForSeconds(cameraFollowDelay);
        
        Vector3 curPos = gameCamera.transform.position;
        Vector3 nextPos = new Vector3(curPos.x, curPos.y - 10, curPos.z);

        while (curPos == nextPos)
        {
            curPos = Vector3.Lerp(curPos, nextPos, 0.2f);
            gameCamera.transform.position = curPos;
            yield return null;
        }
        
    }
}
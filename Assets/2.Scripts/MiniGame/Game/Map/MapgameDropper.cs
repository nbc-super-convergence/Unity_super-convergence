using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGameDropper : MapBase
{
    [SerializeField] private List<GameObject> levels;

    [Header("Camera")]
    [SerializeField] private Camera gameCamera;
    [SerializeField] private float cameraFollowDelay = 0.5f;
    [SerializeField] private float cameraMoveTime = 2f;

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
        float elapsedTime = 0f;

        while (elapsedTime < cameraMoveTime)
        {
            float t = elapsedTime / cameraMoveTime;
            gameCamera.transform.position = Vector3.Lerp(curPos, nextPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameCamera.transform.position = nextPos;
    }
}
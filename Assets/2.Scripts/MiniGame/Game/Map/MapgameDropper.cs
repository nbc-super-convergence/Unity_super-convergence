using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGameDropper : MapBase
{
    [SerializeField] private List<GameObject> levels;

    [Header("Camera")]
    [SerializeField] private Camera gameCamera;
    private readonly float cameraFollowDelay = 0.3f;
    private readonly float cameraMoveTime = 2f;

    [Header("Light")]
    public Light spotLight;

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

    public IEnumerator NextLevelEvent(int[] holes)
    {
        //테스트 코드
        if (gameData.phase == 0) gameData.phase = 1;

        yield return new WaitForSeconds(1f);

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
        Vector3 nextPos = new (curPos.x, curPos.y - 10, curPos.z);

        gameCamera.transform.DOMove(nextPos, cameraMoveTime)
       .SetEase(Ease.InOutBack);

        yield return new WaitForSeconds(cameraMoveTime);
    }
}
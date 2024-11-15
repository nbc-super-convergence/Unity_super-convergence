using System.Collections.Generic;
using System.Collections;
using UnityEngine;

//전부 테스트 코드
public class MapControl : Singleton<MapControl>
{
    //시작 지점
    public Transform startNode;

    // 테스트 플레이어 프리펩
    public GameObject TestPlayerPrefab;

    //플레이어 리스트
    public List<PlayerTokenHandler> playerTokenHandlers = new();
    

    public Material[] materials;

    //현재 턴의 플레이어 인덱스
    private int playerIndex = -1;

    public List<IToggle> trophyNode = new List<IToggle>();
    private int prevTrophyIndex;
    public PlayerTokenHandler Curplayer
    {
        get { return playerTokenHandlers[playerIndex]; }
    }

    protected override void Awake()
    {
        //임시코드
        StartCoroutine(Test());

        base.Awake();
        isDontDestroyOnLoad = false;

        //시작 지점에 플레이어 생성
        PlayerTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<PlayerTokenHandler>();
        //리스트에 플레이어 보관
        playerTokenHandlers.Add(handle);
        //handle.curNode = startNode.nextBoard[0];
    }

    //임시코드
    public IEnumerator Test()
    {
        yield return ResourceManager.Instance.Init();
    }

    public void TestRandomDice()
    {
        //주사위 돌림
        int rand = Random.Range(1, 7);
        //나온 주사위의 수를 플레이어에 입력
        playerTokenHandlers[playerIndex].GetDice(3);
    }

    public void TestTurnEnd()
    {
        playerIndex++;
    }

    public void SetTrophyNode()
    {
        int rand = Random.Range(0, trophyNode.Count);

        while(rand == prevTrophyIndex)
            rand = Random.Range(0, trophyNode.Count);

        if(prevTrophyIndex != -1)
            trophyNode[prevTrophyIndex].Toggle();

        trophyNode[rand].Toggle();

        prevTrophyIndex = rand;
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

//전부 테스트 코드
public class BoardManager : Singleton<BoardManager>
{
    //시작 지점
    public Transform startNode;

    // 테스트 플레이어 프리펩
    public GameObject TestPlayerPrefab;

    //플레이어 리스트
    public List<PlayerTokenHandler> playerTokenHandlers = new();
    public Material[] materials;

    //현재 턴의 플레이어 인덱스
    [SerializeField] private int playerIndex = 0;

    public List<IToggle> trophyNode = new List<IToggle>();
    private int prevTrophyIndex = -1;

    public PlayerTokenHandler Curplayer
    {
        get { return playerTokenHandlers[playerIndex]; }
    }

    public int curPlayerIndex
    {
        get { return playerIndex; }
    }

    protected override void Awake()
    {
        //임시코드
        //StartCoroutine(Test());

        base.Awake();
        isDontDestroyOnLoad = false;

        for(int i =0; i < 2; i++)
        {
            //시작 지점에 플레이어 생성
            PlayerTokenHandler handle = Instantiate(TestPlayerPrefab, startNode.transform.position, Quaternion.identity).GetComponent<PlayerTokenHandler>();
            //리스트에 플레이어 보관
            playerTokenHandlers.Add(handle);
        }

        //handle.curNode = startNode.nextBoard[0];

        //트로피칸 설정
        //SetTrophyNode();
    }

    //임시코드
    //public IEnumerator Test()
    //{
    //    yield return GameManager.Instance.InitApp();
    //}

    public void TestRandomDice()
    {
        //주사위 돌림
        int rand = Random.Range(1, 7);

        //나온 주사위의 수를 플레이어에 입력
        Curplayer.GetDice(rand);
    }

    public void TestTurnEnd()
    {
        if(Curplayer.IsTurnEnd())
        {
            //(현재인원 + 1) % (현재인원 + 1)
            int count = playerTokenHandlers.Count;
            playerIndex = (playerIndex + 1) % (count + 1);
            //playerIndex++;
        }
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

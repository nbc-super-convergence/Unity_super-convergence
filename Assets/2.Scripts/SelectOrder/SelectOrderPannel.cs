using System.Collections.Generic;
using UnityEngine;

public class SelectOrderPannel : MonoBehaviour
{
    private List<DartData> distanceList;    //다트 거리 집합
    private List<float> distanceRank;    //다트 거리의 매겨줄 랭킹

    //다트판 속성
    private float xPositionLimit = 0.5f;  //옆으로 이동하기까지 제한
    private bool swapDirection = false;
    private bool isMove = true;

    private int curDartCnt = 0;
    public int maxDartCnt = 4;

    private void Awake()
    {
        distanceList = new List<DartData>();
        distanceRank = new List<float>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float dist = Vector3.Distance(collision.transform.position, gameObject.transform.position);
        string name = collision.gameObject.name;

        DartData dart = new DartData(dist, name);
        distanceList.Add(dart);

        Debug.Log($"{dart.Name}, {dart.Distance}");

        curDartCnt += 1;
        if (curDartCnt == maxDartCnt)
        {
            isMove = false;
            DistanceRank();
        }
        else
        {
            SelectOrderDartManage.Instance.NextDart();
        }
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            //다트판 이동하기 (세로 한쪽만 던지면 시시하니까)
            //좌우로 왔다갔다 하게
            if (transform.position.x < -xPositionLimit)
                swapDirection = true;
            else if (transform.position.x > xPositionLimit)
                swapDirection = false;

            transform.Translate((swapDirection ? Vector3.right : Vector3.left) * Time.deltaTime);
        }
    }

    //중심과 가까운 다트가 우선순위
    private void DistanceRank()
    {
        int rank = 1;
        
        foreach (DartData dart in distanceList)
            distanceRank.Add(dart.Distance);

        distanceRank.Sort();

        foreach (float distance in distanceRank)
        {
            for (int i = 0; i < 4; i++)
            {
                if (distance.Equals(distanceList[i].Distance))
                {
                    //각 플레이어의 순서 결정 (이게 맞나 싶나...?) 일단 임시로
                    GameManager.Instance.SessionDic[i.ToString()].SetOrder(rank - 1);
                    //distanceList[i].Rank = rank;
                    rank++;
                }
            }
        }

        foreach (DartData dart in distanceList)
        {
            Debug.Log($"{dart.Name} {dart.Rank}");
        }    
    }
}

//다트의 순위 정보들
class DartData
{
    public float Distance { get; private set; }
    public string Name { get; private set; }
    public int Rank { get; set; }

    public DartData(float d, string n)
    {
        Distance = d;
        Name = n;
    }
}
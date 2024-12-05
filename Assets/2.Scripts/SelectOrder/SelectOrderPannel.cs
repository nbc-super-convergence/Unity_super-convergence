using System.Collections.Generic;
using UnityEngine;

public class SelectOrderPannel : MonoBehaviour
{
    private List<float> distanceRank;    //다트 거리의 매겨줄 랭킹

    //다트판 속성
    private float xPositionLimit = 0.5f;  //옆으로 이동하기까지 제한
    private float pannelSpeed = 0.3f;   //다트판 이동 속도
    private bool swapDirection = false;
    private bool isMove = true;

    private int curDartCnt = 0;
    public int maxDartCnt = 4;

    private void Awake()
    {
        distanceRank = new List<float>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out SelectOrderDart dart))
        {
            float dist = Vector3.Distance(collision.transform.position, gameObject.transform.position);
            string name = collision.gameObject.name;

            //맞은 다트의 거리와 이름을 클래스에 전송
            dart.MyDistance = dist;

            //다음 차례
            SelectOrderManager.Instance.NextDart();
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

            transform.Translate((swapDirection ? Vector3.right : Vector3.left) * (Time.deltaTime * pannelSpeed));
        }
    }

    //중심과 가까운 다트가 우선순위
    private void DistanceRank()
    {
        int rank = 1;
        
        foreach (var dart in SelectOrderManager.Instance.DartOrder)
            distanceRank.Add(dart.MyDistance);

        distanceRank.Sort();

        //정렬후 랭킹
        for (int i = 0; i < distanceRank.Count; i++)
        {
            foreach (var dart in SelectOrderManager.Instance.DartOrder)
                if(dart.Equals(distanceRank[i]))
                {
                    dart.MyRank = rank;
                    rank++;
                }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class SelectOrderPannel : MonoBehaviour
{
    private List<DartData> distanceList;    //다트 거리 집합
    private List<float> distanceRank;    //다트 거리의 매겨줄 랭킹

    //다트판 속성
    private float xPositionLimit = 1.2f;  //옆으로 이동하기까지 제한
    private bool swapDirection = false;
    private bool isMove = false;

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

        isMove = false;
    }

    //다트판 이동하기 (세로 한쪽만 던지면 시시하니까)
    private void FixedUpdate()
    {
        if (isMove)
        {
            if (transform.position.x < -xPositionLimit)
                swapDirection = true;
            else if (transform.position.x > xPositionLimit)
                swapDirection = false;

            transform.Translate((swapDirection ? Vector3.right : Vector3.left) * Time.deltaTime);
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
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class BaseBoard : MonoBehaviour
//{
//    //다음 갈 수 있는 칸
//    public List<BaseBoard> nextBoard = new();

//    //갈 수 있는 방향이 2개 이상인 경우 나타나는 프리펩
//    public BoardArrow arrowPrefab;
//    public float arrowDistance;

//    public List<GameObject> arrows = new();
//    //도착한 노드의 갈 수 있는길이 2개 이상인 경우
//    //말이 다음 노드로 갈 수 있도록 설정하기

//    public void NextNode(Action<BaseBoard> action)
//    {
//        if (nextBoard.Count > 1)
//        {
//            if (arrows.Count != 0)
//            {
//                Choice(true);
//            }
//            else
//                CreateArrow(action);
//        }
//        else
//            action?.Invoke(nextBoard[0]);
//    }

//    public void Choice(bool active)
//    {
//        foreach (var g in arrows)
//            g.SetActive(active);
//    }

//    private void CreateArrow(Action<BaseBoard> action)
//    {
//        for (int i = 0; i < nextBoard.Count; i++)
//        {
//            Vector3 pos = (nextBoard[i].transform.position - transform.position).normalized * arrowDistance;
//            float angle = Mathf.Atan2(pos.z, pos.x) * Mathf.Rad2Deg;

//            BoardArrow a = Instantiate(arrowPrefab, transform.position + pos, Quaternion.Euler(90, -angle + 90, 0));
//            arrows.Add(a.gameObject);


//            a.board = nextBoard[i];
//            a.action = action;
//            a.OnEvent = () => Choice(false);
//        }
//    }
//}

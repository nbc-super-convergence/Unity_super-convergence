using System.Collections.Generic;
using UnityEngine;

public class PlayerTokenHandler : MonoBehaviour
{
    private bool isTurn = false; //내 턴인지?
    private float speed = 5f;
    private int dice; //주사위 눈

    private IBoardNode curNode; //현재 위치한 노드
    public PlayerTokenData data;

    public Queue<Transform> queue { get; private set; }

    private void Awake()
    {
        queue = new();
        Transform node = BoardManager.Instance.startNode;
        node.TryGetComponent(out curNode);
    }

    private void Update()
    {
        #region Old
        //if(isTurn)
        //{
        //    Vector3 target = curNode.transform.position;

        //    transform.position = 
        //        Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        //    if(transform.position == target)
        //    {
        //        if(dice > 0)
        //        {
        //            curNode.NextNode(SetNextNode);
        //        }
        //        else
        //        {
        //            //targetNode.GetComponent<IBoard>().Action();
        //            isTurn = false;
        //        }
        //    }
        //}
        #endregion

        if (isTurn)
        {
            Vector3 target = queue.Peek().position;

            transform.position =
                Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (transform.position == target)
            {
                Transform node = queue.Peek();
                
                if(node.TryGetComponent(out IAction n))
                    n.Action();

                queue.Dequeue();
            }

            if (queue.Count == 0)
                isTurn = false;
        }
    }

    //주사위 눈 입력
    public void GetDice(int num)
    {
        dice = dice > num ? dice : num;
        Enqueue(dice);
    }

    public void SetNode(Transform node,bool minus = false)
    {
        if(minus) dice -= 1;
        queue.Enqueue(node);
        node.TryGetComponent(out curNode);
    }

    private void Enqueue(int num)
    {
        for (int i = 0; i < num; i++,dice--)
        {
            if (curNode.TryGetNode(out Transform node))
                SetNode(node);
            else
                break;
        }

        if(queue.Count > 0) isTurn = true;
    }
}
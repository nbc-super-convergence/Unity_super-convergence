using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using TMPro;
using Sirenix.Utilities;


public class BoardTokenHandler : MonoBehaviour
{
    private bool isReady = false;
    private bool isTurn = false; //�� ������?
    private float speed = 5f;
    private float syncTime = 0f;


    public int dice { get; private set; } //�ֻ��� ��

    private IBoardNode curNode; //���� ��ġ�� ���
    public BoardTokenData data;
    [SerializeField] Transform character;

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

        #region �ֻ��� ����

        if (isReady)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                GamePacket packet = new();
                packet.RollDiceRequest = new()
                {

                };

                SocketManager.Instance.OnSend(packet);

                isReady = false;
            }
        }

        #endregion

        #region �����̴� ���� �۵�
        if (isTurn)
        {
            syncTime += Time.deltaTime;

            Vector3 target = queue.Peek().position;
            character.LookAt(queue.Peek());

            transform.position =
                Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (transform.position == target)
            {
                Transform node = queue.Peek();

                if (node.TryGetComponent(out IAction n))
                    n.Action();

                queue.Dequeue();
            }

            if(syncTime >= 1.0f)
            {
                GamePacket packet = new();

                packet.MovePlayerBoardRequest = new()
                {
                    SessionId = "", //���� ���̵� ���ؿ� �� �ʿ���
                    TargetPoint = SocketManager.ToVector(transform.position)
                };

                syncTime = 0.0f;
            }

            if (queue.Count == 0) isTurn = false;
        }

        #endregion
    }

    //�ֻ��� �� �Է�
    public void GetDice(int num)
    {
        dice = dice > num ? dice : num;
        Enqueue(dice);
    }

    public void SetNode(Transform node,bool minus = false)
    {
        if(minus) dice -= 1;
        if (dice < 0) return;        

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
            {
                Action action = curNode.transform.GetComponent<IAction>().Action;

                StartCoroutine(ArrivePlayer(action, curNode.transform));
                break;
            }
        }

        if(queue.Count > 0) isTurn = true;
    }

    public bool IsTurnEnd() => (dice == 0 && queue.Count == 0);

    protected IEnumerator ArrivePlayer(Action action,Transform t)
    {
        while (true)
        {
            if (transform.position.Equals(t.position))
                break;

            yield return null;
        }

        action?.Invoke();
    }

    public void Ready()
    {
        isReady = true;
    }
}
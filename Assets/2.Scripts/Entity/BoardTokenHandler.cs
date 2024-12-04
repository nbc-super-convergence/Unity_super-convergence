using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;


public class BoardTokenHandler : MonoBehaviour
{
    private bool isReady = false;
    private bool isTurn = false; //�� ������?
    public bool isMine = false;

    private float speed = 5f;
    private float syncTime = 0f;
    public Dice diceObject;
    private Vector3 nextPositon;
    //public MeshRenderer renderer;
    public SkinnedMeshRenderer renderer;
    public Animator animator;
    private int runhash;

    public int dice { get; private set; } //�ֻ��� ��

    private IBoardNode curNode; //���� ��ġ�� ���
    public BoardTokenData data;
    [SerializeField] Transform character;

    public Queue<Transform> queue { get; private set; }

    private void Awake()
    {
        nextPositon = transform.position;
        queue = new();
        Transform node = BoardManager.Instance.startNode;
        node.TryGetComponent(out curNode);

        runhash = Animator.StringToHash("Run");
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

        //�̵� ����ȭ, �����ʿ�

        if (!isMine)
        {
            if(transform.position != nextPositon)
            {
                float d = Vector3.Distance(transform.position, nextPositon);
                transform.position = Vector3.MoveTowards(transform.position, nextPositon, Time.deltaTime * d * 30);
            }

            return;
        }

        #region �ֻ��� ����

        if (isReady)
        {
            int rand = UnityEngine.Random.Range(0, 6);
            diceObject.ShowDice(rand);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isReady = false;

                GamePacket packet = new();
                packet.RollDiceRequest = new()
                {
                    SessionId = GameManager.Instance.myInfo.SessionId
                };
                SocketManager.Instance.OnSend(packet);


                diceObject.gameObject.SetActive(false);
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
                GamePacket packet = new();

                packet.MovePlayerBoardRequest = new()
                {
                    SessionId = GameManager.Instance.myInfo.SessionId,
                    TargetPoint = SocketManager.ToVector(transform.position),
                    Rotation = character.transform.eulerAngles.y
                };

                SocketManager.Instance.OnSend(packet);

                Transform node = queue.Peek();
                queue.Dequeue();

                if (node.TryGetComponent(out IAction n))
                    n.Action();
            }

            if (syncTime >= 0.1f)
            {
                GamePacket packet = new();

                packet.MovePlayerBoardRequest = new()
                {
                    SessionId = GameManager.Instance.myInfo.SessionId,
                    TargetPoint = SocketManager.ToVector(transform.position),
                    Rotation = character.transform.eulerAngles.y
                };

                SocketManager.Instance.OnSend(packet);
                Debug.Log("MovePlayerBoardRequest");

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

    public bool IsTurnEnd()
    {
        return (dice == 0 && queue.Count == 0);
    }

    protected IEnumerator ArrivePlayer(Action action,Transform t)
    {
        SetAnimation(true);

        while (true)
        {
            if (transform.position.Equals(t.position))
                break;

            yield return null;
        }

        SetAnimation(false);

        action?.Invoke();
    }

    public void Ready()
    {
        if (!isMine) return;

        isReady = true;
        diceObject.gameObject.SetActive(true);
    }

    public void ReceivePosition(Vector3 position,float rotY)
    {
        nextPositon = position;
        character.transform.eulerAngles = new Vector3(0, rotY, 0);
    }

    public void SetColor(int index)
    {
        renderer.material = BoardManager.Instance.materials[index];
    }

    public void SetAnimation(bool isRun)
    {
        animator.SetBool(runhash,isRun);
    }
}
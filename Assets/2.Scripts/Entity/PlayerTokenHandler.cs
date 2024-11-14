using UnityEngine;

public class PlayerTokenHandler : MonoBehaviour
{
    public PlayerTokenData data;
    private bool isTurn = false; //�� ������?
    public BaseBoard curNode; //���� ��ġ�� ���
    public float speed = 1f;
    public int dice; //�ֻ��� ��

    private void Update()
    {
        if(isTurn)
        {
            Vector3 target = curNode.transform.position;

            transform.position = 
                Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if(transform.position == target)
            {
                if(dice > 0)
                {
                    curNode.NextNode(SetNextNode);
                }
                else
                {
                    //targetNode.GetComponent<IBoard>().Action();
                    isTurn = false;
                }
            }
        }
    }

    //�ֻ��� �� �Է�
    public void GetDice(int num)
    {
        //SetNode()
        isTurn = true;
        dice = num;
    }

    private void SetNextNode(BaseBoard board)
    {
        curNode = board;
        dice--;

        if (dice == 0) isTurn = false;
    }
}

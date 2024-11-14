using UnityEngine;

public class PlayerTokenHandler : MonoBehaviour
{
    public PlayerTokenData data;
    private bool isTurn = false; //내 턴인지?
    public BaseBoard curNode; //현재 위치한 노드
    public float speed = 1f;
    public int dice; //주사위 눈

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

    //주사위 눈 입력
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

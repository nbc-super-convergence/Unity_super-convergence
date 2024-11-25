using UnityEngine;

public class TrophyNode : BaseNode,IToggle
{
    private bool isTrophy;

    private void Start()
    {
        BoardManager.Instance.trophyNode.Add(this);
    }

    public void Toggle()
    {
        isTrophy = !isTrophy;
    }

    //public override bool TryGetNode(out Transform node)
    //{
    //    node = transform;

    //    if (nodes.Count > 1 || isTrophy)
    //    {
    //        StartCoroutine(ArrivePlayer());
    //        return false;
    //    }
    //    else
    //        node = nodes[0];

    //    return true;
    //}

    public override void Action()
    {
        //Ʈ���� + 1
        //BoardManager.Instance.Curplayer.data.trophyAmount += 1;

        //Ʈ���� ȹ���� �̵�
        //PlayerTokenHandler p = BoardManager.Instance.Curplayer;
        //p.SetNode(targetNode, true);
        //p.GetDice(0);
    }

    protected override bool IsStopCondition()
    {
        return isTrophy;
    }
}

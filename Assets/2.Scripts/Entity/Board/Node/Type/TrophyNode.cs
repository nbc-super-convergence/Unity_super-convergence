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
        //트로피 + 1
        //BoardManager.Instance.Curplayer.data.trophyAmount += 1;

        //트로피 획득후 이동
        //PlayerTokenHandler p = BoardManager.Instance.Curplayer;
        //p.SetNode(targetNode, true);
        //p.GetDice(0);
    }

    protected override bool IsStopCondition()
    {
        return isTrophy;
    }
}

public class UIMinigameReady : UIBase
{
    private bool[] isReady = new bool[4];

    public override void Opened(object[] param)
    {
        for (int i = 0; i < isReady.Length;i++) isReady[i] = false;
        //��Ÿ �ʱ�ȭ �۾�
    }
}
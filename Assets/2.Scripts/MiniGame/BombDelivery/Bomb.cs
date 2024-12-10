using UnityEngine;

public class Bomb : MonoBehaviour
{
    //public float coolTime;
    //private float curCoolTime;
    //private float timer;
    int length, targetIndex;
    private Transform target;
    private MiniToken token;

    private void OnEnable()
    {
        //timer = Random.Range(10f, 15f);
        length = MinigameManager.Instance.miniTokens.Length;
    }

    private void Update()
    {
        if (target == null) return;

        transform.position = target.position + (Vector3.up * 3);

        //폭탄 전달 거리 및 폭탄 넘어가는 쿨타임 설정 할지 말지 물어봐야함

        //timer -= Time.deltaTime;

        //if (coolTime > curCoolTime)
        //{
        //    curCoolTime += Time.deltaTime;
        //}
        //else
        //{

        if(targetIndex == GameManager.Instance.myInfo.Color)
        {
            for (int i = 0; i < length; i++)
            {
                if (i == targetIndex) continue;

                Transform t = MinigameManager.Instance.miniTokens[i].transform;

                float d = Vector3.Distance(target.position, t.position);

                if (1.5f > d) ChangeTarget(i);
            }
        }
        //}

        //if (timer < 0.0f) Explosion();

        //GamePacket packet = new();

        //packet.BombMoveRequest = new()
        //{
        //    SessionId = 
        //};

        //SocketManager.Instance.OnSend(packet);
    }

    public void ChangeTarget(int i)
    {
        var c = MinigameManager.Instance.miniTokens[i].MyColor;

        string id = BoardManager.Instance.playerTokenHandlers.
            Find((p) => p.data.userInfo.Color == c).data.userInfo.SessionId;

        GamePacket packet = new();

        packet.BombMoveRequest = new()
        {
            SessionId = id,
        };

        SocketManager.Instance.OnSend(packet);

        if (MinigameManager.Instance.miniTokens[c] is MiniToken token && token.IsClient)
            token.Stun();
    }

    public void SetTarget(string id)
    {
        MiniToken token = MinigameManager.Instance.GetMiniToken(id);

        if (this.token != null)
        {
            this.token.MiniData.PlayerSpeed = 15;
            token.MiniData.PlayerSpeed = 15 * 1.2f;
        }

        this.token = token;
        targetIndex = token.MyColor;
        target = token.transform;
    }

    public void Explosion(string id)
    {
        MiniToken token = MinigameManager.Instance.GetMiniToken(id);    

        if (MinigameManager.Instance.mySessonId == id)
        {
            token.DisableMyToken();
        }

        token.DisableMiniToken();
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    //public float coolTime;
    //private float curCoolTime;
    //private float timer;
    int targetIndex;
    private Transform target;
    private MiniToken token;
    private List<int> colors = new();
    private void OnEnable()
    {
        colors.Clear();

        var list = BoardManager.Instance.playerTokenHandlers;

        for (int i = 0; i < list.Count; i++)
        {
            var c = list[i].data.userInfo.Color;
            colors.Add(c);
        }

        //timer = Random.Range(10f, 15f);
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

        if (!token.isStun && targetIndex == MinigameManager.Instance.GetMyToken().MyColor)
        {
            for (int i = 0; i < colors.Count; i++)
            {
                int c = colors[i];

                if (i == targetIndex || !colors.Contains(c)) continue;
                Debug.LogWarning(c);

                Transform t = MinigameManager.Instance.miniTokens[c].transform;

                float d = Vector3.Distance(target.position, t.position);

                if (1.5f > d) ChangeTarget(c);
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

    public void ChangeTarget(int c)
    {
        //var c = MinigameManager.Instance.miniTokens[i].MyColor;

        string id = BoardManager.Instance.playerTokenHandlers.
            Find((p) => p.data.userInfo.Color == c).data.userInfo.SessionId;

        GamePacket packet = new();

        packet.BombMoveRequest = new()
        {
            SessionId = id,
            BombUserId = MinigameManager.Instance.mySessonId,
        };

        SocketManager.Instance.OnSend(packet);
    }

    public void SetTarget(string id)
    {
        MiniToken token = MinigameManager.Instance.GetMiniToken(id);

        if (this.token != null)
        {
            this.token.MiniData.PlayerSpeed = 15;

            if (token.IsClient) token.Stun();
        }

        token.MiniData.PlayerSpeed = 15 * 1.2f;

        this.token = token;
        targetIndex = token.MyColor;

        target = token.transform;
    }

    public void Explosion(string id)
    {
        MiniToken token = MinigameManager.Instance.GetMiniToken(id);
        colors.Add(token.MyColor);
        Debug.LogWarning(token.MyColor);

        if (MinigameManager.Instance.mySessonId == id)
        {
            token.DisableMyToken();
        }

        token.DisableMiniToken();
        target = null;
        this.token = null;
    }
}
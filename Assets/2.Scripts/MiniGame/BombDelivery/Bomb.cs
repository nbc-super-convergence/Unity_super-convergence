using UnityEngine;

public class Bomb : MonoBehaviour
{
    //public float distance;
    //public float coolTime;
    //private float curCoolTime;
    //private float timer;
    private Transform target;
    //int length,targetIndex;


    //private void OnEnable()
    //{
    //    timer = Random.Range(10f, 15f);
    //    length = MinigameManager.Instance.miniTokens.Length;
    //}

    private void Update()
    {
        if (target == null) return;

        transform.position = target.position + (Vector3.up * 3);

        //timer -= Time.deltaTime;

        //if (coolTime > curCoolTime)
        //{
        //    curCoolTime += Time.deltaTime;
        //}
        //else
        //{
        //    for (int i = 0; i < length; i++)
        //    {
        //        if (i == targetIndex) continue;

        //        Transform t = MinigameManager.Instance.miniTokens[i].transform;

        //        float d = Vector3.Distance(target.position, t.position);

        //        if (distance > d) SetTarget(t, i);
        //    }
        //}

        //if (timer < 0.0f) Explosion();
    }

    public void SetTarget(string id)
    {
        MiniToken token = MinigameManager.Instance.GetMiniToken(id);
        target = token.transform;
    }

    public void Explosion(string id)
    {
        MiniToken token = MinigameManager.Instance.GetMiniToken(id);
        token.DisableMiniToken();
    }
}
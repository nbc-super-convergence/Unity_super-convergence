using System.Collections;
using UnityEngine;

public class MapGameIceSlider : MapBase
{
    [SerializeField] private Transform water;
    
    [SerializeField] private float phaseTime = 1f;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private float inputDelay = 1f;

    public override void HandleCollision(eCollisionType type, Collision collision)
    {
        if (collision.gameObject.GetComponent<MiniToken>().IsClient)
        {
            switch (type)
            {
                case eCollisionType.Bounce:
                    // 충돌 방향 계산
                    Vector3 collisionNormal = collision.contacts[0].normal;
                    Vector3 bounceDirection = -collisionNormal.normalized;

                    collision.rigidbody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
                    collision.gameObject.GetComponent<MiniToken>().PausePlayerInput(inputDelay);
                    break;
            }
        }
    }
            

    public override void HandleCollider(eCollisionType type, Collider other)
    {
        if (other.gameObject.GetComponent<MiniToken>().IsClient)
        {
            switch (type)
            {
                case eCollisionType.Damage:
                    MinigameManager.Instance.GetMiniGame<GameIceSlider>()
                        .GiveDamage(MinigameManager.Instance.mySessonId, 1, true);

                    GamePacket packet = new()
                    {
                        IcePlayerDamageRequest = new()
                        {
                            SessionId = MinigameManager.Instance.mySessonId
                        }
                    };
                    SocketManager.Instance.OnSend(packet);
                    break;
            }
        }
    }

    public void MapDecreaseEvent(int phase)
    {
        StartCoroutine(DecreaseSize(phase));
    }

    private IEnumerator DecreaseSize(int phase)
    {
        Vector3 startPos = water.localPosition;

        float targetHeight = phase switch
        {
            1 => -0.6f,
            2 => -0.2f,
            3 => 0f,
            4 => 1f,
            _ => 1f,
        };
        Vector3 targetPos = new Vector3(7.7247f, targetHeight, -4.7213f);
        
        float elapsedTime = 0f;
        while (elapsedTime < phaseTime)
        {
            water.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / phaseTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        water.localPosition = targetPos;
    }
}
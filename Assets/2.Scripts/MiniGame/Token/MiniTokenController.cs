using UnityEngine;

public enum eMoveType
{
    Server,
    AddForce,
    Velocity,
    Dropper
}

public class MiniTokenController
{
    private readonly MiniTokenData miniData;
    private readonly Transform transform;
    private readonly Rigidbody rb;

    public MiniTokenController(MiniTokenData data, Transform t, Rigidbody _rb)
    {
        miniData = data;
        transform = t;
        rb = _rb;
    }

    public void MoveToken(eMoveType type)
    {
       switch (type)
        {
            case eMoveType.Server:
                float distance = Vector3.Distance(transform.localPosition, miniData.nextPos);
                float threshold = 0.5f;

                if (distance > threshold)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, miniData.nextPos, 30 * Time.deltaTime * distance);
                }
                else
                {
                    transform.localPosition = miniData.nextPos;
                }
                break;
            case eMoveType.AddForce:
                Vector3 force = new(miniData.wasdVector.x, 0, miniData.wasdVector.y);
                rb.AddForce(force * miniData.PlayerSpeed, ForceMode.Force);
                break;
            case eMoveType.Velocity:
                rb.velocity = new Vector3(miniData.wasdVector.x, 0, miniData.wasdVector.y) * (miniData.PlayerSpeed * 0.1f);
                break;
            case eMoveType.Dropper:
                distance = Vector2.Distance(
                    new (transform.localPosition.x, transform.localPosition.z),
                    new (miniData.nextPos.x, miniData.nextPos.z)
                );
                threshold = 0.2f;

                if (distance > threshold)
                {
                    if (miniData.CurState != State.Move)
                        miniData.CurState = State.Move;
                    Vector3 direction = (miniData.nextPos - transform.localPosition).normalized;
                    rb.velocity = new Vector3(direction.x * miniData.PlayerSpeed, direction.y, direction.z * miniData.PlayerSpeed);
                }
                else
                {
                    if (miniData.CurState != State.Idle)
                        miniData.CurState = State.Idle;
                    rb.velocity = new(0, rb.velocity.y, 0);
                    miniData.rotY = 0;
                }
                break;
        }
    }

    public void RotateToken(float rotY)
    {
        transform.rotation = Quaternion.Euler(0f, rotY, 0f);
    }
}
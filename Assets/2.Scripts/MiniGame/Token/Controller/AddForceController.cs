using UnityEngine;

public class AddForceController : MiniPlayerController
{
    public AddForceController(MiniPlayerTokenData miniData, Rigidbody _rb) : base(miniData, _rb)
    {

    }


    public override void MoveVector2()
    {
        // WASD로 입력받아 3D로 컨버트
        Vector3 force = new(playerData.moveVector.x, 0, playerData.moveVector.y);
        rb.AddForce(force * playerData.icePlayerSpeed, ForceMode.Force);
    }
}

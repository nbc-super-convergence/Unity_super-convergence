using UnityEngine;

public class AddForceController : IController
{
    private Rigidbody rgdby;
    private float forceSpeed;   //AddForce의 속도
    private Vector3 forceDirection;  //Force 방향

    public AddForceController(Rigidbody _rgdby, float _spd)
    {
        rgdby = _rgdby; //컴포넌트를 가져오기
        forceSpeed = _spd;
    }

    public void Move(Vector3 pos)
    {
        rgdby.AddForce(pos * forceSpeed, ForceMode.Acceleration);  //이건 물리결과를 보고
        forceDirection = pos;  //Force방향을 저장
    }

    public Vector3 GetForce()
    {
        return forceDirection;
    }

    //미사용 메서드
    public virtual void Interaction(bool isPress)
    {

    }

    public virtual void Jump()
    {

    }
}

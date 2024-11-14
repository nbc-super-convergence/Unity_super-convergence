using UnityEngine;

public class IceSlidingCharacterRotate : MonoBehaviour
{
    [SerializeField] private float _rotationY = 0f; //시작할 때 초기 앵글부터 잡고

    //여기서 회전 적용
    private void FixedUpdate()
    {
        //여기서 Atan을 이용하여 (x,z 만 사용할거지만) 캐릭터 회전
        ApplyRotate();
    }

    /// <summary>
    /// 입력한 방향대로 각도를 조절
    /// </summary>
    /// <param name="dir">IceSlidingController의 InputControll에서 받을 거임</param>
    public void SetInput(Vector3 dir)
    {
        if(dir != Vector3.zero)
            _rotationY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 갱신한 _rotationY값을 각도에 설정
    /// </summary>
    private void ApplyRotate()
    {
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);
    }
}

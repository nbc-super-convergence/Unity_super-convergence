using UnityEngine;

public class MiniPlayerRotate : MonoBehaviour
{
    private float _rotY = 0f; //초기 Y축 회전 값

    private void FixedUpdate()
    {// 현재 Y축 회전 값을 Transform에 적용
        ApplyRotation();
    }

    /// <summary>
    /// 사용자 입력에 대한 Y축 회전 값
    /// </summary>
    public void InputRotation(Vector2 inputDir)
    {
        if (inputDir.sqrMagnitude > 0) // 입력이 존재하는 경우만 처리
            _rotY = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 서버에게 Receive 받은 Y축 회전 값.
    /// </summary>
    public void ReceiveRotation(float angle)
    {
        _rotY = angle;
    }

    //현재 Y축 회전 값을 Transform의 회전에 반영
    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(0f, _rotY, 0f);
    }
}
using UnityEngine;

public class IceSlidingController : MonoBehaviour
{
    private IceBoardTimeManager timeManager;    //가져올 싱글톤

    //컴포넌트 속성
    private IceSlidingBase _iceSlidingBase; //물리 작용으로 전달
    private IceSlidingCharacterRotate _characterRotate; //캐릭터 회전

    private Vector3 _inputDirection;    //입력받을 벡터

    private void Awake()
    {
        timeManager = IceBoardTimeManager.Instance; //싱글톤 적용
        
        _iceSlidingBase = GetComponent<IceSlidingBase>();
        _characterRotate = GetComponentInChildren<IceSlidingCharacterRotate>();
    }

    private void Update()
    {
        if (!timeManager.GameOver)
        {
            _inputDirection.x = Input.GetAxis("Horizontal");
            _inputDirection.z = Input.GetAxis("Vertical");
            InputControll();
        }
    }

    private void InputControll()
    {
        _iceSlidingBase.InputMove(_inputDirection);
        _characterRotate.SetInput(_inputDirection);
    }
}

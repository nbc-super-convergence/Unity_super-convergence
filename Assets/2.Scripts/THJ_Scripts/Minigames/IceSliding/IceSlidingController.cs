using UnityEngine;

public class IceSlidingController : MonoBehaviour
{
    private IceBoardTimeManager timeManager;    //가져올 싱글톤

    //컴포넌트 속성
    private IceSlidingBase _iceSlidingBase; //물리 작용으로 전달
    private CharacterRotate _characterRotate; //캐릭터 회전

    private Vector3 _inputDirection;    //입력받을 벡터

    private void Awake()
    {
        timeManager = IceBoardTimeManager.Instance; //싱글톤 적용
        
        _iceSlidingBase = GetComponent<IceSlidingBase>();
        _characterRotate = GetComponentInChildren<CharacterRotate>();
    }

    private void Update()
    {
        if (!timeManager.GameOver && !_iceSlidingBase.CheckStun)
        {
            _inputDirection.x = Input.GetAxis("Horizontal");
            _inputDirection.z = Input.GetAxis("Vertical");
            InputControll();
        }
    }

    /// <summary>
    /// 받은 입력을 각 메서드에 전달
    /// </summary>
    private void InputControll()
    {
        if (_iceSlidingBase.CheckAlive) //살아 있을 때만
        {
            _iceSlidingBase.InputMove(_inputDirection);
            _characterRotate.SetInput(_inputDirection);
        }
    }
}

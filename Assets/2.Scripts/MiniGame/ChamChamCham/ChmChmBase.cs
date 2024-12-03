using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChmChmBase : MonoBehaviour
{
    Vector3 lookingDirection = Vector3.zero;    //플레이어가 바라보는 방향


    /// <summary>
    /// 입력을 받아서 lookingDirection에 적용
    /// </summary>
    /// <param name="dir">Vector3지만 좌우 방향으로만 바라보게</param>
    public void InputDirection(Vector3 dir)
    {
        lookingDirection = dir;
    }
}

using System;
using System.Collections;
using UnityEngine;

public class IceBoardFloor : MonoBehaviour
{
    private Vector3 _boardLength;    //변경될 빙판 길이

    //서서히 줄어들 시간
    private WaitForSeconds _delayTime;
    private float _delaySecond = 0.1f;
    private float _decreaseGrid = 0.1f; //서서히 줄어들 단위

    private void Start()
    {
        _boardLength = new Vector3(15, transform.localScale.y, 15);  //빙판길이 초기
        transform.localScale = _boardLength;

        _delayTime = new WaitForSeconds(_delaySecond);
    }

    /// <summary>
    /// 빙판 넓이 설정
    /// </summary>
    /// <param name="length">받아올 길이</param>
    public void SetLength(float length)
    {
        /* 서버에서 받아서 적용하기 */

        //한번에 줄어들기
        ApplyLength(new Vector3(length, transform.localScale.y, length));

        //서서히 줄어들기
        //StartCoroutine(DecreaseFloor(length));
    }

    /// <summary>
    /// 변경된 빙판 길이를 적용
    /// </summary>
    /// <param name="scale"></param>
    private void ApplyLength(Vector3 scale)
    {
        transform.localScale = scale;
    }

    /// <summary>
    /// 빙판 넓이가 서서히 사라지게
    /// </summary>
    /// <param name="target">해당 값까지 감소</param>
    /// <returns>_boardLength 만큼 대기</returns>
    IEnumerator DecreaseFloor(float target)
    {
        float curLength = _boardLength.x;
        while (target < curLength)
        {
            if (target >= curLength)
                break;

            curLength -= _decreaseGrid;
            yield return _delayTime;
            //감소후 적용

            _boardLength.x = (float)Math.Round(curLength, 2);
            _boardLength.z = (float)Math.Round(curLength, 2);
            ApplyLength(_boardLength);
        }
    }
}

using System.Collections;
using UnityEngine;



public class ChmChmBase : MonoBehaviour
{
    public Vector3 LookDirection { get; protected set; }

    //오브젝트 속성
    [SerializeField] protected Transform ResultWindow;
    [SerializeField] protected GameObject PlayerAvatar;

    //창문 개폐 좌표
    private float closedWindow = 1f;
    private float openedWindow = 2f;
    //창문 개폐 속도
    private float closedSpeed = 2.5f;
    private float openedSpeed = 1.5f;
    //열고 닫을 때 연출
    private WaitForSeconds windowDelay; 

    private void Awake()
    {
        windowDelay = new WaitForSeconds(Time.deltaTime);

        openedSpeed *= Time.deltaTime;
        closedSpeed *= Time.deltaTime;
    }

    /// <summary>
    /// 창문 개방
    /// </summary>
    public void OpenWindow()
    {
        StartCoroutine(MoveOpenWindow());
    }
    private IEnumerator MoveOpenWindow()
    {
        while (ResultWindow.localPosition.y < openedWindow)
        {
            ResultWindow.Translate(Vector3.up * openedSpeed);
            yield return windowDelay;
        }

        MatchWindowLimit();
    }

    /// <summary>
    /// 창문 폐쇄
    /// </summary>
    public void CloseWindow()
    {
        StartCoroutine(MoveCloseWindow());
    }
    private IEnumerator MoveCloseWindow()
    {
        while(ResultWindow.localPosition.y > closedWindow)
        {
            ResultWindow.Translate(Vector3.down * closedSpeed);
            yield return windowDelay;
        }

        MatchWindowLimit();
    }

    //y좌표의 한계 이상으로 넘어가면 (세밀한 오차 방지)
    private void MatchWindowLimit()
    {
        float limitPos = Mathf.Clamp(ResultWindow.localPosition.y,
            closedWindow, openedWindow);
        Vector3 matchPos = ResultWindow.localPosition;
        matchPos.y = limitPos;
        ResultWindow.localPosition = matchPos;
    }
}

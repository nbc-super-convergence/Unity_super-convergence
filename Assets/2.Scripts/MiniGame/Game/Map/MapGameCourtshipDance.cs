using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGameCourtshipDance : MapBase
{
    [Header("Game Objects")]
    public List<Transform> spawnPosition;
    [SerializeField] private GameObject indicator;

    // -- Cached References -- //
    private Dictionary<int, Transform> prevTranform = new();

    // -- Animator Layer -- //
    private int danceLayerIndex = -1;
    private int baseLayerIndex = 0;


    public void TokenInit(MiniToken token)
    {
        if(danceLayerIndex == -1)
        {
            danceLayerIndex = token.GetAnimator().GetLayerIndex("Dance Layer");
        }

        prevTranform.Add(token.MyColor, token.transform);
        
        token.GetComponent<Rigidbody>().isKinematic = true;
        token.GetAnimator().SetLayerWeight(baseLayerIndex, 0);
        token.GetAnimator().SetLayerWeight(danceLayerIndex, 1);        
        AnimState.ChangePlayerAnimState(token.GetAnimator(), State.DanceWait);        
    }

    public void TokenReset(MiniToken token)
    {
        token.transform.position = prevTranform[token.MyColor].position;
        token.transform.rotation = prevTranform[token.MyColor].rotation;

        token.GetComponent<Rigidbody>().isKinematic = false;
        token.GetAnimator().SetLayerWeight(baseLayerIndex, 1);
        token.GetAnimator().SetLayerWeight(danceLayerIndex, 0);
        AnimState.ChangePlayerAnimState(token.GetAnimator(), State.Idle);
        token.InputHandler.DisableSimpleInput();
    }

    public void ShowIndicator()
    {
        Vector3 pos = MinigameManager.Instance.GetMyToken().transform.position;
        CoroutineIndicator(pos);
    }

    private void CoroutineIndicator(Vector3 targetPosition)
    {
        indicator.transform.position = targetPosition;
        indicator.gameObject.SetActive(true);
        
        // 주석 : 인디케이터가 첫 입력 후 사라지는 코드
        //yield return new WaitUntil(() => UIManager.Get<UICourtshipDance>().myBoard.isFirstInput);
        //indicator.gameObject.SetActive(false);
    }

    public void DanceCloseSocketNotification(string disconnectedSessionId, string replacementSessionId)
    {
        StartCoroutine(CoroutineCloseSocket(disconnectedSessionId, replacementSessionId));
    }

    private IEnumerator CoroutineCloseSocket(string disconnectedSessionId, string replacementSessionId)
    {
        // 처리되기까지 입력 막아서 오작동 회피
        var game = MinigameManager.Instance.GetMiniGame<GameCourtshipDance>();
        yield return new WaitUntil(() => game.isBoardReady);
        var myToken = MinigameManager.Instance.GetMyToken();
        myToken.InputHandler.isEnable = false;
        UIManager.Get<UICourtshipDance>().DisconnectNoti(disconnectedSessionId, replacementSessionId);
        myToken.InputHandler.isEnable = true;
    }
}
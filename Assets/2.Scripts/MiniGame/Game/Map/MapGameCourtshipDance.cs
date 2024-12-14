using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGameCourtshipDance : MapBase
{
    public List<Transform> spawnPosition;
    [SerializeField] private GameObject indicator;
    public Transform mySpawnPos;
    private int danceLayerIndex = -1;
    private int baseLayerIndex = 0;
    private Dictionary<int, Transform> prevTranform = new();

    private AudioSource bgm;

    private void Start()
    {
        bgm = GetComponent<AudioSource>();
    }

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
        StartCoroutine(CoroutineIndicator(pos));
    }

    private IEnumerator CoroutineIndicator(Vector3 targetPosition)
    {
        indicator.transform.position = targetPosition;
        indicator.gameObject.SetActive(true);

        yield return new WaitUntil(() => UIManager.Get<UICourtshipDance>().myBoard.isFirstInput);

        indicator.gameObject.SetActive(false);
    }
}
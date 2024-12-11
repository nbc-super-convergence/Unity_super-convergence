using System.Collections.Generic;
using UnityEngine;

public class MapGameCourtshipDance : MapBase
{
    public List<Transform> spawnPosition;

    private int danceLayerIndex = -1;
    private int baseLayerIndex = 0;

    private Dictionary<int, Transform> prevTranform = new();

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
        token.InputHandler.ChangeActionMap(eActionMap.SimpleInput.ToString());
    }

    public void TokenReset(MiniToken token)
    {
        token.transform.position = prevTranform[token.MyColor].position;
        token.transform.rotation = prevTranform[token.MyColor].rotation;

        token.GetComponent<Rigidbody>().isKinematic = false;
        token.GetAnimator().SetLayerWeight(baseLayerIndex, 1);
        token.GetAnimator().SetLayerWeight(danceLayerIndex, 0);
        AnimState.ChangePlayerAnimState(token.GetAnimator(), State.Idle);
        token.InputHandler.ChangeActionMap(eActionMap.MiniPlayerToken.ToString());
    }
}
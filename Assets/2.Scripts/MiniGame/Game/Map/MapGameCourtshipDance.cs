using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MapGameCourtshipDance : MapBase
{
    public List<Transform> spawnPosition;

    private int danceLayerIndex = -1;
    private int baseLayerIndex = 0;

    
    public void TokenInit(MiniToken token)
    {
        if(danceLayerIndex == -1)
        {
            danceLayerIndex = token.GetAnimator().GetLayerIndex("Dance Layer");
        }

        token.GetComponent<Rigidbody>().isKinematic = true;
        token.GetAnimator().SetLayerWeight(baseLayerIndex, 0);
        token.GetAnimator().SetLayerWeight(danceLayerIndex, 1);

        AnimState.ChangePlayerAnimState(token.GetAnimator(), State.DanceIdle);
    }

    public void TokenReset(MiniToken token)
    {
        token.GetComponent<Rigidbody>().isKinematic = false;
        token.GetAnimator().SetLayerWeight(baseLayerIndex, 1);
        token.GetAnimator().SetLayerWeight(danceLayerIndex, 0);

        AnimState.ChangePlayerAnimState(token.GetAnimator(), State.Idle);
    }
}
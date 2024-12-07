using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class MapGameCourtshipDance : MapBase
{
    public List<Transform> spawnPosition;

    [SerializeField] private AnimatorController danceAnimator;
    private AnimatorController prevAnimator;



    public void TokenInit(MiniToken token)
    {
        if (prevAnimator == null)
        {
            prevAnimator = (AnimatorController)token.GetAnimator().runtimeAnimatorController;
        }
        token.SetAnimator(danceAnimator);
        token.GetComponent<Rigidbody>().isKinematic = true;
    }
    
    public void TokenReset(MiniToken token)
    {
        if (prevAnimator == null) { return; }
        token.SetAnimator(prevAnimator);
        token.GetComponent<Rigidbody>().isKinematic = false;
    }
}
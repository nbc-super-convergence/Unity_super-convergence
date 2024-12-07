using System.Collections.Generic;
using UnityEngine;

public class MapGameCourtshipDance : MapBase
{
    public List<Transform> spawnPosition;

    [SerializeField] private Animator danceAnimator;
    private Animator prevAnimator;



    public void TokenInit(MiniToken token)
    {
        if (prevAnimator == null)
        {
            prevAnimator = token.GetAnimator();
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
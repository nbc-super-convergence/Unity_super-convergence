using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    IController curCtrl;
    private AddForceController addCtrl = new ();
    private VelocityController velCtrl = new ();
    private ButtonController btnCtrl = new ();
    //AnimState

    public void ChangeState(IController newCtrl)
    {

    }
}

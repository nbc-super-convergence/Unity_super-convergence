using UnityEngine;

public interface IController
{
    public void Move(Vector3 pos);
    public void Jump();
    public void Interaction();
}

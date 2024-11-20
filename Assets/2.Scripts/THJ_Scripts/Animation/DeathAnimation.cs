using UnityEngine;

public class DeathAnimation : PlayerBaseAnimation
{
    private WaitForSeconds destroyTime;
    private float deathDepth = 0.5f;

    public DeathAnimation(PlayerAnimState animState) : base(animState)
    {
        destroyTime = new WaitForSeconds(0.05f);
    }

    public override void Start()
    {
        HashCode = Animator.StringToHash("Death");
        base.Start();
    }

    public override void End()
    {
        base.End();
    }

    public override void Update()
    {
        base.Update();
        EndLoop();
    }

    //애니메이션이 끝나면 플래그 실행
    private void EndLoop()
    {
        AnimatorStateInfo stateInfo = animState.Player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == HashCode && stateInfo.normalizedTime > 1f)
        {
            //End();
            DeadDirect();
            Debug.Log("끝");
        }
    }

    /// <summary>
    /// 죽고 사라지는 연출
    /// </summary>
    private void DeadDirect()
    {
        Transform nowPlayerPos = animState.Player.gameObject.transform;

        //죽을 때 물리작용이 일어나지 않게
        animState.Player.playerCollide.enabled = false;
        animState.Player.playerRgdby.useGravity = false;
        animState.Player.playerRgdby.constraints = RigidbodyConstraints.FreezePosition;

        while (nowPlayerPos.position.y > deathDepth)
            nowPlayerPos.Translate(0, -0.01f, 0);
    }
}

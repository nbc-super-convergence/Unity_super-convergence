public class PlayerAnimState : AnimState
{
    public Player Player { get; }

    public IdleAnimation IdleAnim { get; }
    public RunAnimation RunAnim { get; }
    public JumpAnimation JumpAnim { get; }
    public DeathAnimation DeathAnim { get; }

    public PlayerAnimState(Player player)
    {
        Player = player;

        IdleAnim = new IdleAnimation(this);
        RunAnim = new RunAnimation(this);
        JumpAnim = new JumpAnimation(this);
        DeathAnim = new DeathAnimation(this);
    }
}

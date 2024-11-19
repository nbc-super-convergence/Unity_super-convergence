public class PlayerAnimState : AnimState
{
    public Player Player { get; }

    public IdleAnimation IdleAnim { get; }
    public RunAnimation RunAnim { get; }

    public PlayerAnimState(Player player)
    {
        Player = player;

        IdleAnim = new IdleAnimation(this);
        RunAnim = new RunAnimation(this);
    }
}

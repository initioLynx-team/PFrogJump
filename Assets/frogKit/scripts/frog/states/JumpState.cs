using UnityEngine;

public class JumpState : IFrogState
{
    private static readonly int JumpHash = Animator.StringToHash("jump");

    public void Enter(SFrogController frog)
    {
        frog.doubleJump = false;
        frog.Rb.gravityScale = frog.defaultGravity;
        frog.animator.SetBool(JumpHash, true);

        float finalForce = Mathf.Lerp(frog.minJumpForce, frog.maxJumpForce, frog.CurrentChargePct);
        frog.Rb.AddForce(frog.lookDirection * finalForce, ForceMode2D.Impulse);
    }

    public void Exit(SFrogController frog)
    {
        frog.animator.SetBool(JumpHash, false);
        frog.CurrentChargePct = 0;
    }

    public void FixedTick(SFrogController frog)
    {
    }

    public IFrogState Tick(SFrogController frog)
    {
        frog.HandleFacingDirection();
        return SFrogController.OnAir;
    }
}
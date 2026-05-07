using UnityEngine;

public class ThrowState : IFrogState
{
    private static readonly int ThrowHash = Animator.StringToHash("throw");

    public void Enter(SFrogController frog)
    {
        frog.throwCount -= 1;
        frog.PlaySFX(frog.throwSound);
    }

    public void Exit(SFrogController frog)
    {
        frog.animator.SetBool(ThrowHash, true);
        frog.Rb.gravityScale = 0;
        frog.Rb.linearVelocity = Vector2.zero;
    }

    public void FixedTick(SFrogController frog)
    {
    }

    public IFrogState Tick(SFrogController frog)
    {
        frog.HandleFacingDirection();

        Vector2? hitPoint = frog.Tongue.FlickTongue(frog.lookDirection);
        if (hitPoint == null) return SFrogController.Idle;
        frog.stickyTarget = hitPoint.Value;
        return SFrogController.Throwign;
    }
}
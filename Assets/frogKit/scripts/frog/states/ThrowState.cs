using UnityEngine;

public class ThrowState : IFrogState
{
    private static readonly int ThrowHash = Animator.StringToHash("throw");

    bool hit = false;
    public void Enter(SFrogController frog)
    {
        frog.throwCount -= 1;
        frog.PlaySFX(frog.throwSound);
        hit = false;
    }

    public void Exit(SFrogController frog)
    {
        if (hit)
        {
            frog.animator.SetBool(ThrowHash, true);
            frog.Rb.gravityScale = 0;
            frog.Rb.linearVelocity = Vector2.zero;
        }
        else
        {
            /* Failed throw animation ???*/
        }
    }

    public void FixedTick(SFrogController frog)
    {
    }

    public IFrogState Tick(SFrogController frog)
    {
        frog.HandleFacingDirection();

        Vector2? hitPoint = frog.Tongue.FlickTongue(frog.lookDirection);
        if (hitPoint == null)
        {
            return frog.Rb.linearVelocity.y < 0.3f && frog.CheckGroundLayer() 
            ? SFrogController.Idle 
            : SFrogController.OnAir;
        }
        hit = true;
        frog.stickyTarget = hitPoint.Value;
        return SFrogController.Throwign;
    }
}
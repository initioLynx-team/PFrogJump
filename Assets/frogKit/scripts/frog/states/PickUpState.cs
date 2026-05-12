using UnityEngine;

public class PickUpState : IFrogState
{
    private static readonly int ThrowHash = Animator.StringToHash("throw");
    private Vector2 launchDir;

    public void Enter(SFrogController frog)
    {
        launchDir = (frog.stickyTarget - (Vector2)frog.transform.position).normalized;
    }
    public void Exit(SFrogController frog)
    {
        frog.animator.SetBool(ThrowHash, false);
        frog.doubleJump = true;
        frog.Tongue.Visible(false);
        frog.Rb.gravityScale = frog.defaultGravity;

        frog.Rb.AddForce(launchDir * frog.releaseBoost, ForceMode2D.Impulse);
    }

    public void FixedTick(SFrogController frog)
    {
        frog.transform.position = Vector2.MoveTowards(
            frog.transform.position,
            frog.stickyTarget,
            frog.pickUpForce * Time.deltaTime
            );

    }

    public IFrogState Tick(SFrogController frog)
    {


        if (Vector2.Distance(frog.transform.position, frog.stickyTarget) < 0.05f)
        {
            return SFrogController.OnAir;
        }
        return this;
    }
}
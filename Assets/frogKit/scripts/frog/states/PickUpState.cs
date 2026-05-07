using UnityEngine;

public class PickUpState : IFrogState
{
    private static readonly int ThrowHash = Animator.StringToHash("throw");

    public void Enter(SFrogController frog)
    {

    }
    public void Exit(SFrogController frog)
    {
        frog.animator.SetBool(ThrowHash, false);
        frog.doubleJump = true;
        frog.Tongue.Visible(false);
        frog.Rb.gravityScale = frog.defaultGravity;
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
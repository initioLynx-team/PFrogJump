using UnityEngine;

public class OnAirState : IFrogState
{
    private static readonly int GroundedHash = Animator.StringToHash("grounded");

    public void Enter(SFrogController frog)
    {
        frog.animator.SetBool(GroundedHash,false);

    }

    public void Exit(SFrogController frog)
    {
    }

    public IFrogState Tick(SFrogController frog)
    {
        frog.HandleFacingDirection();

        if (frog.isTonguePressed && frog.throwCount > 0)
        {
            return SFrogController.Throw;
        }
        if (frog.isJumpHeld && frog.doubleJump)
        {
            frog.animator.SetBool(GroundedHash,true);
            return SFrogController.Jump;
        }
        if (frog.Rb.linearVelocity.y <= 0.2f && frog.IsGrounded())
        {
            return SFrogController.Idle;
        }

        return this;
    }

    public void FixedTick(SFrogController frog) { }

}
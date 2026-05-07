using UnityEngine;

public class IdleState : IFrogState
{
    private static readonly int GroundedHash = Animator.StringToHash("grounded");

    public void Enter(SFrogController frog)
    {
        if (!frog.isOnSlipperyFloor)
        {
            frog.Rb.linearVelocity = Vector2.zero;
        }
        frog.Rb.gravityScale = frog.defaultGravity;
        frog.throwCount = frog.maxThrows;
        frog.animator.SetBool(GroundedHash,true);
    }

    public void Exit(SFrogController frog)
    {

    }

    public void FixedTick(SFrogController frog) { /* No physics needed while stationary */ }

    public IFrogState Tick(SFrogController frog)
    {
        frog.HandleFacingDirection();

        if (frog.isTonguePressed)
        {
            return SFrogController.Throw;
        }
        else if (frog.isJumpHeld && frog.Rb.linearVelocity.magnitude < frog.speedThreshold)
        {
            return SFrogController.Charging;
        }
        else if (frog.movement.sqrMagnitude > 0.03f)
        {
            return SFrogController.Moving;
        }

        return this;
    }
}
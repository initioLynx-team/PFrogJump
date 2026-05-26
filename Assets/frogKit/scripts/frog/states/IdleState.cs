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
        frog.animator.SetBool(GroundedHash, true);
    }

    public void Exit(SFrogController frog)
    {
        frog.indicatorLook.SetVisible(false);
    }

    public void FixedTick(SFrogController frog) { /* No physics needed while stationary */ }

    public IFrogState Tick(SFrogController frog)
    {
        frog.HandleFacingDirection();
        frog.indicatorLook.UpdateLookDirection(frog.lookDirection);

        if (frog.isTonguePressed)
        {
            return SFrogController.Throw;
        }
        else if (frog.isJumpHeld && frog.Rb.linearVelocity.magnitude < frog.speedThreshold)
        {
            return SFrogController.Charging;
        }
        
        if (frog.Rb.linearVelocity.y > 0.2f && !frog.CheckGroundLayer())
        {
            return SFrogController.OnAir;
        } 
        if (frog.movement.sqrMagnitude > 0.03f)
        {
            return SFrogController.Moving;
        }

        return this;
    }
}
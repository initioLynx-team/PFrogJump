using UnityEngine;

public class MovingState : IFrogState
{
    private static readonly int MovingHash = Animator.StringToHash("isMoving");

    public void Enter(SFrogController frog)
    {
        frog.animator.SetBool(MovingHash, true);
        if (!frog.isOnSlipperyFloor)
        {
            frog.Rb.linearVelocityY = 0;
            frog.Rb.sharedMaterial = frog.normalPMat;
        }
    }

    public void Exit(SFrogController frog)
    {
        if (!frog.isOnSlipperyFloor)
        {
            frog.Rb.sharedMaterial = frog.ogPMat;
        }
        frog.animator.SetBool(MovingHash, false);
        frog.indicatorLook.SetVisible(false);
    }

    public IFrogState Tick(SFrogController frog)
    {
        frog.indicatorLook.UpdateLookDirection(frog.lookDirection);
        frog.spriteRenderer.flipX = frog.movement.x > 0.1f;
        if (frog.Rb.linearVelocity.y > 0.2f && !frog.CheckGroundLayer())
        {
            return SFrogController.OnAir;
        }
        if (frog.movement.sqrMagnitude <= 0.03f) return SFrogController.Idle;
        return this;
    }
    public void FixedTick(SFrogController frog)
    {
        frog.Rb.linearVelocity = new Vector2(frog.movement.x * frog.moveSpeed, frog.Rb.linearVelocity.y);
    }


}
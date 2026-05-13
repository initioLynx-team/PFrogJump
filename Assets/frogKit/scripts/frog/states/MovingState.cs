using UnityEngine;

public class MovingState : IFrogState
{
    private static readonly int MovingHash = Animator.StringToHash("isMoving");

    public void Enter(SFrogController frog)
    {
        frog.animator.SetBool(MovingHash, true);
    }

    public void Exit(SFrogController frog)
    {
        frog.animator.SetBool(MovingHash, false);

    }

    public IFrogState Tick(SFrogController frog)
    {
        if (frog.movement.sqrMagnitude <= 0.03f) return SFrogController.Idle;
        frog.spriteRenderer.flipX = frog.movement.x > 0.1f;
        if (!frog.IsOnGround())
        {
            return SFrogController.OnAir;
        }
        return this;
    }
    public void FixedTick(SFrogController frog)
    {
        frog.Rb.linearVelocity = new Vector2(frog.movement.x * frog.moveSpeed, frog.Rb.linearVelocity.y);
    }


}
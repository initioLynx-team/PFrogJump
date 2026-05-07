using UnityEngine;
public class ChargingState : IFrogState
{
    private readonly int ChargeHash = Animator.StringToHash("charge");
    private float _stateTimer; // Private variable, not in Inspector

    //public float CurrentChargePct => Mathf.Clamp01(chargeTimer / maxChargeTime);

    public void Enter(SFrogController frog)
    {
        _stateTimer = 0f;
        frog.animator.SetBool(ChargeHash, true);
        frog.Rb.linearVelocity = Vector2.zero;
    }

    public void Exit(SFrogController frog)
    {
        frog.animator.SetBool(ChargeHash, false);
    }

    public void FixedTick(SFrogController frog) { /* No physics needed while stationary */ }

    public IFrogState Tick(SFrogController frog)
    {
        frog.CurrentChargePct = Mathf.Clamp01(_stateTimer / frog.maxChargeTime);
        if (!frog.isJumpHeld) return SFrogController.Jump;
        frog.HandleFacingDirection();
        _stateTimer += Time.deltaTime;
        if (_stateTimer >= frog.maxChargeTime){
            _stateTimer = frog.maxChargeTime;
        }
        return this;
    }



}
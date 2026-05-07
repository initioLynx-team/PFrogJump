using UnityEngine;

public class ThrowingState : IFrogState
{

    public void Enter(SFrogController frog)
    {
        frog.progressTrowing = 0;

    }

    public void Exit(SFrogController frog)
    {
    }

    public void FixedTick(SFrogController frog)
    {
    }

    public IFrogState Tick(SFrogController frog)
    {
        float interpolation = (float)frog.progressTrowing / frog.throwingSegments;
        Vector2 currentPos = (Vector2)frog.transform.position;
        Vector2 targetPos = Vector2.Lerp(currentPos, frog.stickyTarget, interpolation);
        frog.Tongue.SetTongueData(targetPos);
        frog.progressTrowing += 1;
        if (frog.progressTrowing >= frog.throwingSegments)
        {
            return SFrogController.PickUp;
        }

        return this;
    }
}
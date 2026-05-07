public interface  IFrogState
{
    void Enter(SFrogController frog);
    IFrogState Tick(SFrogController frog);
    void FixedTick(SFrogController frog);
    void Exit(SFrogController frog);
}
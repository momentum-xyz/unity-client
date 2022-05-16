using Odyssey;

public class StateController : IState
{
    public StateController(IMomentumContext context)
    {
        _c = context;
    }

    public virtual void OnEnter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void OnExit()
    {
    }

    protected IMomentumContext _c;
}
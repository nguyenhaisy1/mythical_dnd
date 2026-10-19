namespace Game.Application.CommandPipeline
{
    /// <summary>Durable atomic compare-and-commit port. Failure requires reopening and reloading before another command.</summary>
    public interface ICommittedWorldStore
    {
        CommittedWorld Load();
        void Commit(CommittedWorld expected, CommittedWorld candidate);
    }
}

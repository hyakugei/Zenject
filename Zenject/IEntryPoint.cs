namespace Zenject
{
    public interface IEntryPoint
    {
        int InitPriority { get; }

        void Initialize();
    }
}

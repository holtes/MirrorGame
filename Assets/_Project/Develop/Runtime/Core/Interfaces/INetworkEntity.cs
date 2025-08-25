using R3;

namespace Core.Interfaces
{
    public interface INetworkEntity
    {
        public bool IsLocal { get; }
        public Observable<Unit> OnLocalPlayerStarted { get; }
        public Observable<Unit> OnLocalPlayerStopped { get; }
    }
}
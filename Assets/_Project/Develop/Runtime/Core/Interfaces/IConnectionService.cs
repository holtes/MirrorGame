using R3;

namespace Core.Interfaces
{
    public interface IConnectionService
    {
        public Observable<Unit> OnClientConnected { get; }
        public Observable<Unit> OnClientDisconnected { get; }

        public void Host(string nickname);

        public void Connect(string nickname);
    }
}
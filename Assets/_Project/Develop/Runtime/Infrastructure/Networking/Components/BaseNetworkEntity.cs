using Core.Interfaces;
using Mirror;
using R3;

namespace Infrastructure.Networking.Components
{
    public class BaseNetworkEntity : NetworkBehaviour, INetworkEntity
    {
        public bool IsLocal => isLocalPlayer;

        public Observable<Unit> OnLocalPlayerStarted => _onLocalPlayerStarted;
        public Observable<Unit> OnLocalPlayerStopped => _onLocalPlayerStopped;

        private Subject<Unit> _onLocalPlayerStarted = new();
        private Subject<Unit> _onLocalPlayerStopped = new();

        public override void OnStartLocalPlayer() => _onLocalPlayerStarted.OnNext(Unit.Default);
        public override void OnStopAuthority() => _onLocalPlayerStopped.OnNext(Unit.Default);
    }
}
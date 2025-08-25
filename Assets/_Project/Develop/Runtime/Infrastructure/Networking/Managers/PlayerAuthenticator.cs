using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Mirror;
using R3;
using Infrastructure.Networking.Messages;

namespace Infrastructure.Networking.Managers
{
    public class PlayerAuthenticator : NetworkAuthenticator
    {
        public Observable<Unit> OnClientStartAuthenticate => _onClientStartAuthenticate;

        private Dictionary<NetworkConnectionToClient, string> _pendingNicknames = new Dictionary<NetworkConnectionToClient, string>();
        private HashSet<NetworkConnectionToClient> _connectionsPendingDisconnect = new HashSet<NetworkConnectionToClient>();
        
        private Subject<Unit> _onClientStartAuthenticate = new();

        #region Server
        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
            NetworkServer.OnDisconnectedEvent += OnServerDisconnected;
        }

        public override void OnStopServer()
        {
            NetworkServer.UnregisterHandler<AuthRequestMessage>();
            NetworkServer.OnDisconnectedEvent -= OnServerDisconnected;

            _pendingNicknames.Clear();
        }

        private void OnServerDisconnected(NetworkConnectionToClient conn)
        {
            if (_pendingNicknames.TryGetValue(conn, out var nickname))
            {
                Debug.Log($"[Auth] Player disconnected, freeing nickname: {nickname}");
                _pendingNicknames.Remove(conn);
            }
        }

        public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            var cleanNickname = SanitizeNickname(msg.Nickname);

            Debug.Log($"Authentication Request: {cleanNickname}");

            if (_connectionsPendingDisconnect.Contains(conn)) return;

            if (!_pendingNicknames.ContainsValue(cleanNickname))
            {
                _pendingNicknames[conn] = cleanNickname;

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    Code = 100,
                    Message = "Success"
                };

                conn.Send(authResponseMessage);

                conn.authenticationData = cleanNickname;

                ServerAccept(conn);
            }
            else
            {
                _connectionsPendingDisconnect.Add(conn);

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    Code = 200,
                    Message = "Invalid Credentials"
                };

                conn.Send(authResponseMessage);

                conn.isAuthenticated = false;

                DelayedDisconnect(conn, 1f).Forget();
            }
        }

        private async UniTaskVoid DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime), ignoreTimeScale: false);

            ServerReject(conn);

            _connectionsPendingDisconnect.Remove(conn);
        }

        private string SanitizeNickname(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return $"Player_{Random.Range(1000, 9999)}";
            var t = s.Trim();
            if (t.Length > 20) t = t.Substring(0, 20);
            return t;
        }
        #endregion

        #region Client

        public override void OnStartClient()
        {
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

        public override void OnStopClient()
        {
            NetworkClient.UnregisterHandler<AuthResponseMessage>();
        }

        public override void OnClientAuthenticate()
        {
            _onClientStartAuthenticate.OnNext(Unit.Default);
        }

        public void OnAuthResponseMessage(AuthResponseMessage msg)
        {
            if (msg.Code == 100)
            {
                Debug.Log($"Authentication Response: {msg.Message}");

                ClientAccept();
            }
            else
            {
                Debug.LogError($"Authentication Response: {msg.Message}");

                ClientReject();
            }
        }
        #endregion
    }
}
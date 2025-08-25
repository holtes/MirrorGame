using UnityEngine;
using R3;

namespace Core.Interfaces
{
    public interface IPlayerService
    {
        public Observable<string> OnNicknameChanged { get; }

        public void SetNickname(string nickname);
        public void SendNickMessage();
        public void SpawnNetObj(int objId, Vector3 position);
    }
}
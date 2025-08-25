using UnityEngine;
using UnityEngine.InputSystem;
using R3;

namespace Domain.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

        public Observable<Vector2> OnMoveInput => _onMoveInput;
        public Observable<Unit> OnSendMessagePressed => _onSendMessagePressed;
        public Observable<Unit> OnSpawnNetObjPressed => _onSpawnNetObjPressed;

        private Subject<Vector2> _onMoveInput = new();

        private Subject<Unit> _onSendMessagePressed = new();
        private Subject<Unit> _onSpawnNetObjPressed = new();

        public void OnMove(InputValue value)
        {
            MoveInput = value.Get<Vector2>();
            _onMoveInput.OnNext(MoveInput);
        }

        public void OnSendMessage()
        {
            _onSendMessagePressed.OnNext(Unit.Default);
        }

        public void OnSpawnNetObj()
        {
            _onSpawnNetObjPressed.OnNext(Unit.Default);
        }
    }
}
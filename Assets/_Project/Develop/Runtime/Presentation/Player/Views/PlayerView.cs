using UnityEngine;
using TMPro;
using R3;
using R3.Triggers;

namespace Presentation.Player.Views
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Transform _nicknameTransform;
        [SerializeField] private TMP_Text _nicknameText;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _charAnimator;
        [SerializeField] private float _cameraRotationSpeed = 10f;

        private Camera _gameCamera => Camera.main;
        private float _playerGravity;
        private Vector3 _playerSpeed;
        private Vector3 _velocity;

        private void Awake()
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ => MovePlayer())
                .AddTo(this);

            this.LateUpdateAsObservable()
                .Subscribe(_ => RotateNicknameToCamera())
                .AddTo(this);
        }

        public void Init(float gravity)
        {
            _playerGravity = gravity;
        }

        public void SetNickname(string nickname)
        {
            _nicknameText.text = nickname;
        }

        public void SetPlayerSpeed(Vector3 speed)
        {
            _playerSpeed = speed;
            SetAnimationSpeed(speed.magnitude);
        }

        private void MovePlayer()
        {
            _characterController.Move(_playerSpeed * Time.deltaTime);

            if (_characterController.isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            _velocity.y += _playerGravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);

            if (_charAnimator.GetBool("IsRunning"))
            {
                var targetRot = Quaternion.LookRotation(_playerSpeed, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    Time.deltaTime * _cameraRotationSpeed
                );
            }
        }

        private void SetAnimationSpeed(float speed)
        {
            var isRunning = speed > 0.01f;
            _charAnimator.SetBool("IsRunning", isRunning);
        }

        private void RotateNicknameToCamera()
        {
            var dir = _nicknameTransform.position - _gameCamera.transform.position;

            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
                _nicknameTransform.rotation = Quaternion.LookRotation(dir);
        }
    }
}
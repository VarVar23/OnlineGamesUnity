using Fusion;
using UnityEngine;

public class AnimatorSelector1D : NetworkBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    private NetworkCharacterControllerPrototype _networkCharacterController;

    [Networked(OnChanged = nameof(OnSpeedChanged))]
    public float Speed { get; set; }

    private void Awake() =>
        _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;

        var velocity = _networkCharacterController.Velocity.normalized;
        var forward = transform.forward;
        var forwardVelocity = Vector3.Project(velocity, forward);
        var speedRatio = Vector3.Dot(forward, forwardVelocity);
        Speed = speedRatio;
    }

    public void AnimatorSpeedChange(float speed) =>
        _playerAnimator.SetFloat("Speed", speed);

    public static void OnSpeedChanged(Changed<AnimatorSelector1D> changed) =>
        changed.Behaviour.AnimatorSpeedChange(changed.Behaviour.Speed);
}
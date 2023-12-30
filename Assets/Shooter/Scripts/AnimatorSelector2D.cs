using Fusion;
using UnityEngine;

public class AnimatorSelector2D : NetworkBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    private NetworkCharacterControllerPrototype _networkCharacterController;

    [Networked(OnChanged = nameof(OnForwardChanged))]
    public float Forward { get; set; }

    [Networked(OnChanged = nameof(OnRightChanged))]
    public float Right { get; set; }


    private void Awake() =>
        _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;

        Vector3 forward = transform.forward;
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = transform.right;
        right.y = 0;
        right = right.normalized;

        var velocity = _networkCharacterController.Velocity;
        float forwardValue = Vector3.Dot(forward, velocity);
        float rightValue = Vector3.Dot(right, velocity);

        Forward = forwardValue;
        Right = rightValue;
    }

    public void AnimatorForwardChange(float forward) =>
        _playerAnimator.SetFloat("Forward", forward);

    public void AnimatorRightChange(float right) =>
        _playerAnimator.SetFloat("Right", right);

    public static void OnForwardChanged(Changed<AnimatorSelector2D> changed) =>
        changed.Behaviour.AnimatorForwardChange(changed.Behaviour.Forward);

    public static void OnRightChanged(Changed<AnimatorSelector2D> changed) =>
        changed.Behaviour.AnimatorRightChange(changed.Behaviour.Right);
}
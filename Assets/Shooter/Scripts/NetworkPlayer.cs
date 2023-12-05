using Fusion;

public class NetworkPlayer : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _networkCharacterController;

    private void Awake() =>
        _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            _networkCharacterController.Move(data.Direction);
        }
    }
}
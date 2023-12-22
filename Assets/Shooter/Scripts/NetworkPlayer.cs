using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnBombSpawned))]
    private NetworkBool _bombSpawned { get; set; }

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _shootDelay = 0.5f;

    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float _force = 10;

    [Networked] private TickTimer _delay { get; set; }
    private NetworkCharacterControllerPrototype _networkCharacterController;
    private Material _material;
    private Vector3 _forward;
    private Text _messages;


    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
        _material = GetComponentInChildren<MeshRenderer>().material;
    }

    private void Update()
    {
        if(Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Привет друг!");
        }
    }

    public override void Render()
    {
        _material.color = Color.Lerp(_material.color, Color.white, Time.deltaTime);
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out NetworkInputData data))
        {
            var direction = data.Direction;
            _networkCharacterController.Move(direction);

            if (direction.sqrMagnitude > 0)
                _forward = direction;

            if(_delay.ExpiredOrNotRunning(Runner))
            {
                _delay = TickTimer.CreateFromSeconds(Runner, _shootDelay);

                if((data.buttons & NetworkInputData.MouseButton0) != 0)
                {
                    Runner.Spawn(
                        prefab: _bulletPrefab,
                        position: transform.position + _forward,
                        rotation: Quaternion.LookRotation(_forward),
                        inputAuthority: Object.InputAuthority,
                        onBeforeSpawned: (_, networkObject) => { networkObject.GetComponent<Bullet>().Init(); });
                }
                else if ((data.buttons & NetworkInputData.MouseButton1) != 0)
                {
                    Runner.Spawn(
                        prefab: _bombPrefab,
                        position: transform.position + _forward,
                        rotation: Quaternion.LookRotation(_forward),
                        inputAuthority: Object.InputAuthority,
                        onBeforeSpawned: (_, networkObject) => { networkObject.GetComponent<Bomb>().Init(_force); });

                    _bombSpawned = !_bombSpawned;
                }
            }
        }
    }

    public static void OnBombSpawned(Changed<NetworkPlayer> player)
    {
        player.Behaviour._material.color = Color.green;
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if(_messages == null)
        {
            _messages = FindObjectOfType<Text>();
        }

        message = info.IsInvokeLocal ? "Ты сказал: " + message + "\n" : "Другой игрок сказал: " + message + "\n";
        _messages.text = message;
    }
}
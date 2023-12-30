using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnBombSpawned))]
    private NetworkBool _bombSpawned { get; set; }

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _shootDelay = 0.3f;
    [SerializeField] private float _bombDelay = 0.5f;
    [SerializeField] private float _powerThrow = 1.5f;
    

    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private float _force = 10;

    [Networked] private TickTimer _delay { get; set; }
    private TickTimer ShootDelay { get; set; }
    private TickTimer BombDelay { get; set; }

    private NetworkCharacterControllerPrototype _networkCharacterController;
    private Material _material;
    private Vector3 _forward;
    private Text _messages;


    private void Awake()
    {
        _networkCharacterController = GetComponent<NetworkCharacterControllerPrototype>();
        //_material = GetComponentInChildren<MeshRenderer>().material;
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
        //_material.color = Color.Lerp(_material.color, Color.white, Time.deltaTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false) return;

        if(GetInput(out NetworkInputData data))
        {
            var direction = data.Direction;
            _networkCharacterController.Move(direction);

            if((data.buttons & NetworkInputData.MouseButton0) != 0)
            {
                if (ShootDelay.ExpiredOrNotRunning(Runner))
                {
                    ShootDelay = TickTimer.CreateFromSeconds(Runner, _shootDelay);

                    Runner.Spawn(
                        prefab: _bulletPrefab,
                        position: transform.position + transform.forward,
                        rotation: Quaternion.LookRotation(transform.forward),
                        inputAuthority: Object.InputAuthority,
                        onBeforeSpawned: (_, networkObject) => { networkObject.GetComponent<Bullet>().Init(); }
                    );
                }
            }

            if ((data.buttons & NetworkInputData.MouseButton1) != 0)
            {
                if(BombDelay.ExpiredOrNotRunning(Runner))
                {
                    BombDelay = TickTimer.CreateFromSeconds(Runner, _bombDelay);

                    var position = transform.position + transform.forward + Vector3.up;
                    var dir = Quaternion.LookRotation(Vector3.up + transform.forward);
                    var force = (Vector3.up + transform.forward) * _powerThrow;

                    Runner.Spawn(
                        prefab: _bombPrefab,
                        position: position,
                        rotation: dir,
                        inputAuthority: Object.InputAuthority,
                        onBeforeSpawned: (_, networkObject) => { networkObject.GetComponent<Bomb>().Init(force); }
                    );
                }
            }
        }

        var colliders = new Collider[3];
        int count = Runner.GetPhysicsScene().OverlapSphere(
            position: transform.position,
            radius: 2f,
            results: colliders,
            layerMask: -1,
            QueryTriggerInteraction.Collide
        );

        for( int i = 0; i < count; i++ )
        {
            if (colliders[i].TryGetComponent<Item>(out var item))
            {
                item.PickUp(Object);
            }
        }
    }

    public static void OnBombSpawned(Changed<NetworkPlayer> player)
    {
        //player.Behaviour._material.color = Color.green;
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
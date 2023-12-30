using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private Transform[] _spawnPoints; // Homework 1
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private readonly Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = new();

    //private bool _mouseButton0;
    //private bool _mouseButton1;

    //private void Update()
    //{
    //    _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
    //    _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
    //}

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Joined");

        if(runner.IsServer)
        {
            Vector3 spawnPosition = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)].position; // Homework 1
            NetworkObject networkObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedPlayers.Add(player, networkObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Left");

        if(_spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedPlayers.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        var movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        data.Direction = movement;
        data.Direction.Normalize();

        data.buttons |= Input.GetMouseButton(0) ? NetworkInputData.MouseButton0 : (byte)0;
        data.buttons |= Input.GetMouseButton(1) ? NetworkInputData.MouseButton1 : (byte)0;

        input.Set(data);
    }

    public void OnConnectedToServer(NetworkRunner runner) => Debug.Log("On Connected To Server");
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) => Debug.Log("On Connected Failed");
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) => Debug.Log("On Connected Request");
    public void OnDisconnectedFromServer(NetworkRunner runner) => Debug.Log("On Disconnected From Server");
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) => Debug.Log("Shutdown");


    /////////////////////////////////////////////////////////////////////////////////////////////////////////


    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
}
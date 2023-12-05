using Fusion;
using Fusion.Sockets;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private NetworkRunner _networkRunner;

    private void Start()
    {
        var startGameTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient); // ������ ������������� ��������
        Debug.Log("Start Game");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode)
    {
        var sceneManager = runner.GetComponents<INetworkSceneManager>().FirstOrDefault(); // ���������, ����������� �� �������� �� ������� runner

        if(sceneManager == null) // ���� ���������� ���, �� ������� ���
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true; // �������������� ������ � �����
        
        var startGameArgs = new StartGameArgs // ��������� ����
        {
            GameMode = gameMode,
            Address = NetAddress.Any(),
            Scene = SceneManager.GetActiveScene().buildIndex,
            SessionName = "TestRoom",
            SceneManager = sceneManager
        };

        var startGameResult = runner.StartGame(startGameArgs); // ���������� ����������

        return startGameResult; // ���������� ���������
    }
}
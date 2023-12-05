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
        var startGameTask = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient); // Запуск инициализации нетворка
        Debug.Log("Start Game");
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode)
    {
        var sceneManager = runner.GetComponents<INetworkSceneManager>().FirstOrDefault(); // Проверяем, реализуется ли интерфес на объекте runner

        if(sceneManager == null) // Если интерфейса нет, то добавим его
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true; // Предоставление данных о вводе
        
        var startGameArgs = new StartGameArgs // Аргументы игры
        {
            GameMode = gameMode,
            Address = NetAddress.Any(),
            Scene = SceneManager.GetActiveScene().buildIndex,
            SessionName = "TestRoom",
            SceneManager = sceneManager
        };

        var startGameResult = runner.StartGame(startGameArgs); // Установить соединение

        return startGameResult; // Возвращаем результат
    }
}
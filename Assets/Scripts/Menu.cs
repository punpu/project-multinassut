using UnityEngine;
using UnityEngine.UI;
using FishNet.Managing;

public class Menu : MonoBehaviour
{
    public Button serverButton;
    public Button clientButton;
    private NetworkManager _networkManager;

    public void Start()
    {
        _networkManager = Object.FindFirstObjectByType<NetworkManager>();
        serverButton.onClick.AddListener(OnServerButtonClick);
        clientButton.onClick.AddListener(OnClientButtonClick);
    }

    public void OnServerButtonClick()
    {
        if (_networkManager == null) {
            return;
        }

        // if (_serverState != LocalConnectionState.Stopped)
        //     _networkManager.ServerManager.StopConnection(true);
        // else
        _networkManager.ServerManager.StartConnection();
    }


    public void OnClientButtonClick()
    {
        if (_networkManager == null)
        {
            return;
        }

        // if (_clientState != LocalConnectionState.Stopped)
        //     _networkManager.ClientManager.StopConnection();
        // else
        _networkManager.ClientManager.StartConnection();
    }
}

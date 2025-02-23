using FishNet.Object;
using FishNet.Connection;
using UnityEngine;
using FishNet.Object.Synchronizing;
using System;
using FishNet.Transporting;

public class GameManager : NetworkBehaviour
{
    public event Action<NetworkConnection, string> OnNameAdded;
    public event Action<NetworkConnection, string> OnNameUpdated;
    public event Action<NetworkConnection> OnNameRemoved;

    private struct PlayerData
    {
        public int connId;
        public string name;
        public GameObject player;
    }

    private readonly SyncDictionary<int, PlayerData> PlayerDatas = new();

    public override void OnSpawnServer(NetworkConnection connection)
    {
        base.OnSpawnServer(connection);
        AddPlayer(connection.ClientId, "Player" + connection.ClientId, gameObject);
    }

    public override void OnStartServer()
    {
        ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
    }

    public override void OnStopServer()
    {
        ServerManager.OnRemoteConnectionState -= ServerManager_OnRemoteConnectionState;
    }

    private void ServerManager_OnRemoteConnectionState(NetworkConnection arg1, RemoteConnectionStateArgs arg2)
    {
        if (arg2.ConnectionState == RemoteConnectionState.Stopped)
        {
            PlayerDatas.Remove(arg1.ClientId);
        } else if (arg2.ConnectionState == RemoteConnectionState.Started)
        {
            Debug.Log(arg1.ClientId + " connected" + arg1.Objects);
            AddPlayer(arg1.ClientId, "Player" + arg1.ClientId, null);
        }
    }

    [Server]
    private int RandomizeMorsoProperty()
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, PlayerDatas.Count);
        return PlayerDatas[randomIndex].connId;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGame()
    {

        int morsoedConnection = RandomizeMorsoProperty();
        //send message to all clients to start game
        RpcStartGame(morsoedConnection);
        Debug.Log("Game started");
    }

    [ObserversRpc]
    private void RpcStartGame(int morsoedConnection)
    {
        Debug.Log("Game started on client");
        // Add client-side logic to handle game start here
        var playerObject = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Player object count" + playerObject.Length);
        foreach (var player in playerObject)
        {
            if(morsoedConnection == player.GetComponent<NetworkObject>().OwnerId)
            {
                Debug.Log("Morsoing player");
                player.GetComponentInChildren<Light>().enabled = false;
                Destroy(player.GetComponent<Reukku>());
                // set morso health as 1000
                player.GetComponent<Health>().TakeDamage(-900);
            } else {
                Destroy(player.GetComponent<MorsoBehavior>());
            }
            
            Debug.Log("Teleporting player");
            player.GetComponent<Teleport>().TeleportPlayer();
        }
    }
    [Server]
    public void AddPlayer(int Id, string Name, GameObject Player)
    {
        Debug.Log("Add conn.ClientId:" + Id);

        PlayerData Pd = new PlayerData
        {
            connId = Id,
            name = Name,
            player = Player
        };
        if (PlayerDatas.ContainsKey(Id))
        {
            Debug.Log("Player already exists");
            return;
        } else {
            PlayerDatas[Id] = Pd;
        }
        Debug.Log(PlayerDatas.Count);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

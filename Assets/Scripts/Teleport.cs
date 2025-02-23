using FishNet.Component.Prediction;
using UnityEngine;

public class Teleport : MonoBehaviour
{

    [SerializeField] private Transform _teleportPosition;

    public void TeleportPlayer()
    {
        // Find morsoed player
        var playerObject = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerObject)
        {
            if(player.GetComponent<MorsoBehavior>().enabled)
            {
                Debug.Log("Teleporting Morso player");
                var morsoRespawns = GameObject.FindGameObjectsWithTag("MorsoSpawnFloor");
                //teleport player to a random location on one of the respawn points
                var morsoSpawnLocation = morsoRespawns[Random.Range(0, morsoRespawns.Length)];
                player.transform.position = morsoSpawnLocation.transform.position;
                return;
            }
        }

        var respawns = GameObject.FindGameObjectsWithTag("Respawn");
        //teleport player to a random location on one of the respawn points
        var spawnLocation = respawns[Random.Range(0, respawns.Length)];
        Debug.Log("Teleporting player to respawn");
        transform.position = spawnLocation.transform.position;
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

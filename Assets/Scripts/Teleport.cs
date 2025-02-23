using System;
using System.Xml.Serialization;
using UnityEngine;

public class Teleport : MonoBehaviour
{

    [SerializeField] private Transform _teleportPosition;


    public void TeleportPlayer()
    {
        var playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playerObjects)
        {
            if(player.GetComponent<MorsoBehavior>().enabled == true)     
            {
                var morsoRespawns = GameObject.FindGameObjectsWithTag("MorsoSpawnFloor");
               //teleport player to a random location on one of the respawn points
                var morsoSpawnLocation = morsoRespawns[UnityEngine.Random.Range(0, morsoRespawns.Length)];
                Debug.Log("Teleporting Morso player");
                transform.position = morsoSpawnLocation.transform.position;
            } else {
                var respawns = GameObject.FindGameObjectsWithTag("Respawn");
                //teleport player to a random location on one of the respawn points
                var spawnLocation = respawns[UnityEngine.Random.Range(0, respawns.Length)];
                Debug.Log("Teleporting player");
                transform.position = spawnLocation.transform.position;
            }
        }
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

using UnityEngine;

public class Teleport : MonoBehaviour
{

    [SerializeField] private Transform _teleportPosition;

    public void TeleportPlayer()
    {
        Debug.Log("Teleporting player");
        var respawns = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject respawn in respawns)
        {
            Debug.Log("Teleporting player to respawn");
          transform.position = respawn.transform.position;

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

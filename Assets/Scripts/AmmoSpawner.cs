using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class AmmoSpawner : NetworkBehaviour
{
    public GameObject ammoPrefab;
    private bool _ammoIsSpawned = false;
    private float _spawnTime;

    [Server]
    private void SpawnAmmo()
    {
        GameObject go = Instantiate(ammoPrefab, gameObject.transform);
        InstanceFinder.ServerManager.Spawn(go);
        RandomizeSpawnTime();
    }

    public override void OnSpawnServer(NetworkConnection connection)
    {
        SpawnAmmo();
    }

    private void RandomizeSpawnTime()
    {
        _spawnTime = Random.Range(5, 10);
    }

    [Server]
    public void DespawnAmmoPickup(GameObject ammoPickup)
    {
        if (ammoPickup == null) return;
        InstanceFinder.ServerManager.Despawn(ammoPickup); // Despawn for all clients
        Invoke("SpawnAmmo", _spawnTime);
    }
}

using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private float _maxHealth = 100f;

    private readonly SyncVar<float> _health = new SyncVar<float>();

    private PlayerUI _playerUI;
    private AudioManager _audioManager;

    private void Awake()
    {
        _health.OnChange += OnHealthChange;
        _playerUI = GetComponentInChildren<PlayerUI>();
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void Update()
    {
        if (_playerUI && base.IsOwner)
        {
            _playerUI.SetHealth(_health.Value);
        }
    }

    public override void OnStartServer()
    {
        _health.Value = _maxHealth;
    }

    private void OnHealthChange(float prev, float next, bool asServer)
    {
        if (asServer) {
            var objectName = gameObject.name;
            Debug.Log(objectName + " health changed to " + next);
            // TODO: handle player death differently
            if (next <= 0) {

                if(gameObject.tag == "Player") {
                    TakeDamage(-100);
                    Debug.Log("PLAYER DEATH");
                    gameObject.GetComponent<Teleport>().TeleportPlayer();
                }
                else {
                    Debug.Log(objectName + " died");
                    base.Despawn();
                }

            }
        }
    }

    [ServerRpc(RequireOwnership = false)] 
    public void TakeDamage(float damage) {
        _health.Value -= damage;
        var position = transform.position;
        _audioManager.PlaySfx("auh", position);
    }
   
}
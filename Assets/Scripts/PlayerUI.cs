using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Health _playerHealth;
    [SerializeField] private TextMeshProUGUI _healthUI;
    [SerializeField] private TextMeshProUGUI _ammoUI;

    public void SetHealth(float value)
    {
        _healthUI.SetText(value.ToString());
    }

    public void SetAmmo(int value)
    {
        if (_ammoUI)
        {
            Debug.Log($"SetAmmo, value: {value}");
            _ammoUI.SetText(value.ToString());
        }
    }
}

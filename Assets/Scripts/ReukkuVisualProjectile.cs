using UnityEngine;

public class ReukkuVisualProjectile : MonoBehaviour
{

    [SerializeField] private float _speed = 3000f;
    [SerializeField] private float _maxLifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, _maxLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.deltaTime * Vector3.forward);
    }

}

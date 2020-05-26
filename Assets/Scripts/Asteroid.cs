using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 3.0f;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    [SerializeField]
    private AudioSource _explosion;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null){
            Debug.LogError("The Spawn Manger is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * _rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Laser"){
            Destroy(other.gameObject);
            GameObject Explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(Explosion, 3.0f);
            _explosion.Play(0);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.5f);
        }
    }
}

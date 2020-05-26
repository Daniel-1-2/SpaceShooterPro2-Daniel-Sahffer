using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;
    private Animator _animator;
    private AudioSource _explosion;
    [SerializeField]
    private GameObject _laserPrefab;
    private bool _isDead = false;
    // Start is called before the first frame update
    void Start(){
        FireLaser();
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null){
            Debug.LogError("The Player is NULL.");
        }

        _animator = GetComponent<Animator>();

        if (_animator == null){
            Debug.LogError("The animator is NULL.");
        }

        _explosion = GameObject.Find("Explosion").GetComponent<AudioSource>();
        if(_explosion == null){
            Debug.LogError("The AudioSource on the Explosion is NULL.");
        }
    }

    // Update is called once per frame
    void Update(){
        CalculateMovement();
    }

    void CalculateMovement(){
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -6)
        {
            transform.position = new Vector3(Random.Range(-9, 9), 7, 0);
        }
    }

    void FireLaser(){
        StartCoroutine(FireLaserRoutine());
    }

    IEnumerator FireLaserRoutine(){
        while(true){
            yield return new WaitForSeconds(Random.Range(3, 7));
            if (_isDead == false){
                Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player"){
            Player player = GameObject.Find("Player").GetComponent<Player>();
            if (player != null){
                player.Damage();
            }
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _explosion.Play(0);
            StopCoroutine(FireLaserRoutine());
            _isDead = true;
            
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }

        if (other.gameObject.tag == "Laser"){
            _player.AddScore(10);
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(other.gameObject);
            _explosion.Play(0);
            StopCoroutine(FireLaserRoutine());
            _isDead = true;

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }
}

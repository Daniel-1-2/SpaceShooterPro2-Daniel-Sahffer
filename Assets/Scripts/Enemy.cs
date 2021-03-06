﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    [SerializeField]
    private float _dogeSpeed = 2.0f;
    private Player _player;
    private Animator _animator;
    private AudioSource _explosion;
    [SerializeField]
    private GameObject _laserPrefab;
    private bool _isDead = false;
    private GameObject playerLaser;
    [SerializeField]
    private float _ramSpeed = 1;
    private SpawnManager _spawnManager;
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
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(_spawnManager == null){
            Debug.LogError("The Spawn Manager is NULL.");
        }

    }

    // Update is called once per frame
    void Update(){
        CalculateMovement();
        if(this.gameObject.CompareTag("DogerEnemy") && _player.FiredLaser() != null){
            float movement = transform.position.x - _player.FiredLaser().transform.position.x;
            Vector3 moveDirection = new Vector3(movement, 0, 0);
            if(movement <= 3){
                transform.Translate(moveDirection * Time.deltaTime * _dogeSpeed);
            }
        }
        Vector3 ramDirection = transform.position - _player.transform.position;
        if(ramDirection.x <= 1 || ramDirection.y <= 1){
            transform.Translate(_player.transform.position * Time.deltaTime * _ramSpeed);
        }
        float distance = transform.position.x - _spawnManager.SpawnedPowerup().transform.position.x;
        if(distance <= 1 && transform.position.y < _spawnManager.SpawnedPowerup().transform.position.y){
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        }
        if(this.gameObject.CompareTag("XrayEnemy")){
            float distanceBetween = transform.position.x - _player.transform.position.x;
            Debug.Log(distanceBetween);
            if(transform.position.y < _player.transform.position.y && distanceBetween <= 2){
                Debug.Log("Duhh");
                Instantiate(_laserPrefab, transform.position, Quaternion.Inverse(Quaternion.identity));
            }
        }
    }

    void CalculateMovement(){
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y <= -6)
        {
            transform.position = new Vector3(Random.Range(-9, 9), 7, 0);
        }
        if (transform.position.x >= 10)
        {
            transform.position = new Vector3(-10, transform.position.y, 0);
        }
        else if (transform.position.x <= -10)
        {
            transform.position = new Vector3(10, transform.position.y, 0);
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

        if (other.gameObject.tag == "Laser" || other.gameObject.CompareTag("Laser_Homing")){
            _player.AddScore(10);
            _animator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            if (_player.PircingEnebled() != true){
                Destroy(other.gameObject);
            }
            _explosion.Play(0);
            StopCoroutine(FireLaserRoutine());
            _isDead = true;

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
        
        
    }
}

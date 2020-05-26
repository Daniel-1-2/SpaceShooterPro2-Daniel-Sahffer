using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    private Player _player;
    [SerializeField]
    private float _powerupId;
    private AudioSource _powerupSound;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null){
            Debug.LogError("Player is NULL.");
        }

        _powerupSound = GameObject.Find("PowerupSound").GetComponent<AudioSource>();
        if(_powerupSound == null){
            Debug.LogError("The Powerup Sound is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y <= -6){
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == "Player"){
            switch(_powerupId){
                case 0:
                _player.TripleShotActive();
                break;
                case 1:
                _player.SpeedBoostActive();
                break;
                case 2:
                _player.ShieldCollected();
                break;
                default:
                Debug.Log("Not a valid powerup id.");
                break;
            }
            _powerupSound.Play(0);
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    #region Declration Of Varibles
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private int _shieldLives = 0;
    [SerializeField]
    private GameObject _laserPrefab;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    [SerializeField]
    private GameObject _TripleShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1;
    [SerializeField]
    private int _score = 0;
    [SerializeField]
    private float _speedMultiplier = 2;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _ammo = 15;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldCollected = false;
    [SerializeField]
    private GameObject[] _engines;
    [SerializeField]
    private AudioClip _laserFireAudio;
    private AudioSource _audioSource;
    #endregion

    // Start is called before the first frame update
    void Start(){
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if(_spawnManager == null){
            Debug.LogError("The SpawnManager is NULL.");
        }
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null){
            Debug.LogError("The UIManager is NULL.");
        }

        _audioSource = GetComponent<AudioSource>();

        if(_audioSource == null){
            Debug.LogError("The Audio Source on the Player is NULL.");
        }
    }

    // Update is called once per frame
    void Update(){
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammo >= 1){
            FireLaser();
        }
        if (_shieldLives == 2)
        {
            _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if (_shieldLives == 1)
        {
            _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if(_shieldLives == 3){
            _shieldVisualizer.GetComponent<SpriteRenderer>().color = Color.white;
        }
        _uiManager.UpdateAmmoVisual(_ammo);
    }


    void CalculateMovement(){

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        /* 
        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
        transform.Translate(new Vector3(horizontalInput, verticalInput, 0) * _speed * Time.deltaTime);
        */

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if(_isSpeedBoostActive == true){
            transform.Translate(direction * (_speedMultiplier * _speed) * Time.deltaTime);
        }
        else{
            transform.Translate(direction * _speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)){
            _speed += 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)){
            _speed -= 2;
        }

        float yRange = Mathf.Clamp(transform.position.y, -4, 0);
        transform.position = new Vector3(transform.position.x, yRange, 0);

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
        Vector3 offset = new Vector3(0, 1.5f, 0);
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive == true){
            Instantiate(_TripleShotPrefab, transform.position + offset, Quaternion.identity);
        }
        else{
            Instantiate(_laserPrefab, transform.position + offset, Quaternion.identity);
        }
        _audioSource.clip = _laserFireAudio;
        _audioSource.Play(0);
        _ammo--;
    }

    public void Damage(){
        if(_shieldLives <= 3 && _shieldLives > 0){
            _shieldLives--;
        }
        if(_isShieldCollected == true && _shieldLives == 0){
            _shieldVisualizer.SetActive(false);
            _isShieldCollected = false;
            return;
        }
        if(_isShieldCollected == false){
            _lives--;
            if (_lives == 2)
            {
                _engines[Random.Range(0, 2)].SetActive(true);
            }
            else if (_lives == 1)
            {
                if (_engines[1].gameObject.activeInHierarchy == true)
                {
                    _engines[0].SetActive(true);
                }
                else if (_engines[0].gameObject.activeInHierarchy == true)
                {
                    _engines[1].SetActive(true);
                }
            }
            _uiManager.UpdateLives(_lives);
            if (_lives <= 0)
            {
                Destroy(this.gameObject);
                _spawnManager.OnPlayerDeath();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag  == "EnemyLaser"){
            Destroy(other.gameObject);
            Damage();
        }

    }

    public void TripleShotActive(){
        if(_isTripleShotActive == false){
            _isTripleShotActive = true;
            StartCoroutine(TripleShotPowerdownRoutine());
        }
    }

    IEnumerator TripleShotPowerdownRoutine(){
        while(_isTripleShotActive == true){
            yield return new WaitForSeconds(5);
            _isTripleShotActive = false;
        }
    }

    public void SpeedBoostActive(){
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerdownRoutine());
    }

    IEnumerator SpeedBoostPowerdownRoutine(){
        while(true){
            yield return new WaitForSeconds(5.0f);
            _isSpeedBoostActive = false;
        }
    }

    public void ShieldCollected(){
        _isShieldCollected = true;
        _shieldVisualizer.SetActive(true);
        _shieldLives = 3;
    }

    public void AddScore(int points){
        _score += points;
        _uiManager.UpdateScoreText(_score);
    }

    public void AddAmmo(){
        _ammo = 15;
    }
}

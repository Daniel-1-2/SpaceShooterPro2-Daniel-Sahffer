using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    #region Declration Of Varibles
    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _newSpeed = 5.5f;
    private bool _thrustersEnabled = false;
    [SerializeField]
    private int _shieldLives = 0;
    [SerializeField]
    private float _avalbleThrust = 10;
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
    [SerializeField]
    private bool _canPirce = false;
    [SerializeField]
    private bool _pircingEnabled;
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
        _uiManager.UpdateLives(_lives);
        _uiManager.UpdateThrust(_avalbleThrust);
        if(_avalbleThrust == 0){
            StartCoroutine(ThrustHeatingUp());
        }
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
            if(_thrustersEnabled == true){
                transform.Translate(direction * (_speedMultiplier * _newSpeed) * Time.deltaTime);
            }
            else{
                transform.Translate(direction * (_speedMultiplier * _speed) * Time.deltaTime);
            }
        }
        else if(_thrustersEnabled == true){
            transform.Translate(direction * _newSpeed * Time.deltaTime);
        }
        else{
            transform.Translate(direction * _speed * Time.deltaTime);
        }

        ThrustSystem();

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
#region Powerups
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

    public void CollectedHealthPowerup(){
        if(_lives < 3){
            _lives++;
        }
        if (_engines[1].gameObject.activeInHierarchy == true)
        {
            _engines[1].SetActive(false);
        }
        else if (_engines[0].gameObject.activeInHierarchy == true)
        {
            _engines[0].SetActive(false);
        }
    }

    public void PircePowerupCollected(){
        _canPirce = true;
        StartCoroutine(PircePowerdownRoutine());
    }

    IEnumerator PircePowerdownRoutine(){
        if (_canPirce == true){
            _pircingEnabled = true;
            yield return new WaitForSeconds(5.0f);
            _canPirce = false;
            _pircingEnabled = false;
        }
    }

    public bool PircingEnebled(){
        if (_pircingEnabled == true){
            return true;
        }
        return false;
    }
#endregion
    private void ThrustSystem(){
        if (_avalbleThrust >= 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _thrustersEnabled = true;
                StartCoroutine(ThrustCooldown());
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _thrustersEnabled = false;
                StopCoroutine(ThrustCooldown());
            }
        }
        _uiManager.UpdateThrust(_avalbleThrust);
        /*
        if thrust power != 0 && pressing down shift
        thrust power-- every second
        if thrust == 0
        Wait for 10 sec to full charge
        */ 
    }

    private IEnumerator ThrustCooldown(){
        if(_avalbleThrust >= 1){
            while (Input.GetKey(KeyCode.LeftShift) && _avalbleThrust != 0)
            {
                _avalbleThrust--;
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
    private IEnumerator ThrustHeatingUp(){
        if (_avalbleThrust == 0)
        {
            _thrustersEnabled = false;
            yield return new WaitForSeconds(10.0f);
            _avalbleThrust = 10;
        }
    }

    public int Score(){
        return _score;
    }

    public void NegativePowerup(){
        _score -= 30;
        _uiManager.UpdateScoreText(_score);
    }
}

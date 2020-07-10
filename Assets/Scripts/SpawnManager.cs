using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyPrefabs;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning = false;
    [SerializeField]
    private GameObject[] _powerups;
    private float _rareaty1;
    private int _wave = 1;
    private UIManager _uiManager;
    private Enemy _enemy;
    private Player _player;
    private int _oneTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uiManager == null){
            Debug.LogError("The UIManager is NULL.");
        }
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null){
            Debug.LogError("The Player is NULL.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        #region WaveSystem
        switch(_player.Score()){
            case 100:
            if(_oneTime == 0){
                _wave++;
                _oneTime++;
            }
            break;
            case 200:
            if (_oneTime == 1)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 300:
            if (_oneTime == 2)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 400:
            if (_oneTime == 3)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 500:
            if (_oneTime == 4)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 600:
            if (_oneTime == 5)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 700:
            if (_oneTime == 6)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 800:
            if (_oneTime == 7)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 900:
            if (_oneTime == 8)
            {
                _wave++;
                _oneTime++;
            }
            break;
            case 1000:
            if (_oneTime == 9)
            {
                _wave++;
                _oneTime++;
            }
            break;
        }
        #endregion
        EnemyKills();

    }

    public void StartSpawning(){
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine(){

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false){
            Vector3 posToSpawn = new Vector3(Random.Range(-9, 9), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefabs[Random.Range(0, 2)], posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f / _wave);
        }
    }

    IEnumerator SpawnPowerupRoutine(){

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false){
            Vector3 posToSpawn = new Vector3(Random.Range(-9, 9), 7, 0);
            Instantiate(_powerups[Random.Range(0, 7)], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3.0f, _rareaty1));
        }
    }

    public void OnPlayerDeath(){
        _stopSpawning = true;
    }

    public void RareatyReturn(float rareaty){
        _rareaty1 = rareaty;
    }

    private void EnemyKills(){
        _uiManager.UpdateWave(_wave);
    }
}

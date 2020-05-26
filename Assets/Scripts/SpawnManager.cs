using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning = false;
    [SerializeField]
    private GameObject[] _powerups;
    private float _rareaty1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartSpawning(){
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine(){

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false){
            Vector3 posToSpawn = new Vector3(Random.Range(-9, 9), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine(){

        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false){
            Vector3 posToSpawn = new Vector3(Random.Range(-9, 9), 7, 0);
            Instantiate(_powerups[Random.Range(0, 6)], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3.0f, _rareaty1));
        }
    }

    public void OnPlayerDeath(){
        _stopSpawning = true;
    }

    public void RareatyReturn(float rareaty){
        _rareaty1 = rareaty;
    }
}

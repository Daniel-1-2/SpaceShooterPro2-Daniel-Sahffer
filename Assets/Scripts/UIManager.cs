using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _livesSprite;
    [SerializeField]
    private Text _GameOverText;
    [SerializeField]
    private Text _restartText;
    private GameManager _gameManager;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Slider _thrust;
    // Start is called before the first frame update
    void Start()
    {
        _thrust.value = 10;
        _scoreText.text = "Score: " + 0;
        _GameOverText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null){
            Debug.LogError("The Game Manager is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreText(int score){
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int lives){
        _livesImage.sprite = _livesSprite[lives];

        if (lives == 0){
            GameOverSequence();
        }
    }

    void GameOverSequence(){
        _gameManager.GameOver();
        _GameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine(){
        while(true){
            _GameOverText.text = "Game Over";
            yield return new WaitForSeconds(0.75f);
            _GameOverText.text = " ";
            yield return new WaitForSeconds(0.75f);
        }
    }

    public void UpdateAmmoVisual(int ammo){
        _ammoText.text = "Ammo: " + ammo + "/" + 15;
    }

    public void UpdateThrust(float thrust){
        _thrust.value = thrust;
    }
}

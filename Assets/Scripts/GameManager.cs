using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public BoardManager boardManager;

    public float levelStartDelay = 2f;
    
    private int _level;
    private Text _levelText;
    private GameObject _levelImage;
    private List<Enemy> _enemies;
    private bool _enemimiesMoving;
    private bool _isDoingSetup;
    private GameObject tryAgain;
    private GameObject _maxDaysImage;
    
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;
    public float turnDelay = .1f;


    void Awake()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        _enemies = new List<Enemy>();
        boardManager = GetComponent<BoardManager>();
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {    
        _level++;

        int maxLevel = PlayerPrefs.GetInt("max_level", 1);

        if (maxLevel < _level)
        {
            PlayerPrefs.SetInt("max_level", _level);
            PlayerPrefs.Save();
        }
        
        InitGame();
    }
    

    private void InitGame()
    {
        _isDoingSetup = true;
        _levelImage = GameObject.Find("LevelImage");
        _levelText = GameObject.Find("LevelText").GetComponent<Text>();
        _levelText.text = "Day " + _level;
        _levelImage.SetActive(true);
        _maxDaysImage = GameObject.Find("MaxDaysText");
        _maxDaysImage.SetActive(false);
        tryAgain = GameObject.Find("TryAgain");
        tryAgain.GetComponent<Button>().onClick.AddListener(() => {Invoke(nameof(RestartGame), 1f);});
        tryAgain.SetActive(false);
        
        Invoke(nameof(HideLevelImage), levelStartDelay);
        _enemies.Clear();
        boardManager.SetupScene(_level);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
        _level = 0;
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<Player>().food = 100;
        SoundManager.instance.musicSource.Play();
    }
    

    private void HideLevelImage()
    {
        _levelImage.SetActive(false);
        _isDoingSetup = false;
    }

    public void GameOver()
    {
        _maxDaysImage.GetComponent<Text>().text = "Max days alive: " + PlayerPrefs.GetInt("max_level");
        _levelText.text = "After " + _level + " days you've starved.";
        _levelImage.SetActive(true);
        _maxDaysImage.SetActive(true);
        tryAgain.SetActive(true);
    }

    IEnumerator MoveEnemies()
    {
        _enemimiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if (_enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        
        foreach (var enemy in _enemies)
        {
            enemy.Move();
            yield return new WaitForSeconds(enemy.moveTime);
        }

        playerTurn = true;
        _enemimiesMoving = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn || _enemimiesMoving || _isDoingSetup)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy enemy)
    {
        _enemies.Add(enemy);
    }
}
using System;
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

    public int playerFoodPoints = 100;
    [HideInInspector] public bool playerTurn = true;
    public float turnDelay = .1f;


    void Awake()
    {
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
        InitGame();
    }
    

    private void InitGame()
    {
        _isDoingSetup = true;
        _levelImage = GameObject.Find("LevelImage");
        _levelText = GameObject.Find("LevelText").GetComponent<Text>();
        _levelText.text = "Day " + _level;
        _levelImage.SetActive(true);
        Invoke(nameof(HideLevelImage), levelStartDelay);
        _enemies.Clear();
        boardManager.SetupScene(_level);
    }

    private void HideLevelImage()
    {
        _levelImage.SetActive(false);
        _isDoingSetup = false;
    }

    public void GameOver()
    {
        _levelText.text = "After " + _level + " days you've starved.";
        _levelImage.SetActive(true);
        enabled = false;
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
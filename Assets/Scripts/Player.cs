using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int wallDamage = 1;

    public int pointsPerFood = 10;

    public int pointsPerSoda = 20;

    public float restartLevelDelay = 1f;

    public Text foodText;

    private Animator animator;

    public int food;

    public AudioClip moveSound1;
    public AudioClip moverSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Vector2 touchOrigin = -Vector2.one;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        SetFoodText();
        base.Start();
    }

    private void SetFoodText(int points = 0)
    {
        string text = "";

        if (points > 0)
        {
            text += "+ " + points + " ";
        }
        else if (points < 0)
        {
            text += points + " ";
        }

        text += "Food: " + food;

        foodText.text = text;
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    public void LoseFood(int loss)
    {
        SetFoodText(-loss);
        animator.SetTrigger("PlayerHit");
        food -= loss;
        CheckIfGameOver();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playerTurn)
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

#if UNITY_EDITOR|| UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");


        if (horizontal != 0)
        {
            vertical = 0;
        }

#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                touchOrigin = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = touch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }

            }
        }
#endif

        transform.localRotation = Quaternion.Euler(0, horizontal == -1 ? 180 : 0, 0);


        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;

        if (hitWall != null)
        {
            food--;
            hitWall.DamageWall(wallDamage);
            animator.SetTrigger("PlayerChop");
        }
    }

    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        bool canMove = base.AttemptMove<T>(xDir, yDir);

        if (canMove)
        {
            food--;
            SoundManager.instance.RandomizeSFX(moveSound1, moverSound2);
        }

        CheckIfGameOver();

        GameManager.instance.playerTurn = false;

        SetFoodText();

        return canMove;
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.RandomizeSFX(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            Invoke(nameof(Restart), restartLevelDelay);
            enabled = false;
        }
        else if (other.CompareTag("Food"))
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
            SetFoodText(pointsPerFood);
            SoundManager.instance.RandomizeSFX(eatSound1, eatSound2);
        }
        else if (other.CompareTag("Soda"))
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
            SetFoodText(pointsPerSoda);
            SoundManager.instance.RandomizeSFX(drinkSound1, drinkSound2);
        }
    }
}
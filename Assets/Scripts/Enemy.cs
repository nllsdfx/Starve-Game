using UnityEngine;

public class Enemy : MovingObject
{

    public int playerDamage;

    private Animator _animator;

    private Transform target;

    private bool skipMove;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        _animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void OnCantMove<T>(T component)
    {
        Player player = component as Player;

        if (player != null)
        {
            _animator.SetTrigger("EnemyAttack");
            player.LoseFood(playerDamage);
            SoundManager.instance.RandomizeSFX(enemyAttack1, enemyAttack2);
        }
    }

    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return false;
        }
        
        skipMove = true;

        return base.AttemptMove<T>(xDir, yDir);
    }

   

    public void Move()
    {
        int xDir = 0;
        int yDir = 0;
        
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
        
        AttemptMove<Player>(xDir, yDir);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;

    public int hp = 3;

    private SpriteRenderer _spriteRenderer;

    public AudioClip chop1;
    public AudioClip chop2;
    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        _spriteRenderer.sprite = dmgSprite;
        hp -= loss;
        SoundManager.instance.RandomizeSFX(chop1, chop2);

        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

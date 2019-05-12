using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    private static float moveTime = .1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;
    private float _inversMoveTime;
    // keeps final move position so object don't stuck inside each other at the end of move animation
    private static HashSet<Vector2> _ends = new HashSet<Vector2>();
    
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        _inversMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        
        Vector2 end = start + new Vector2(xDir, yDir);
        
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        
        if (hit.transform == null && !_ends.Contains(end))
        {
            StartCoroutine(SmoothMovement(end));
            _ends.Add(end);
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement(Vector2 end)
    {
        Vector2 position = transform.position;

        float sqrRemainingDistance = (position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector2 moveTowards = Vector2.MoveTowards(rb2d.position, end, _inversMoveTime * Time.deltaTime);
            rb2d.MovePosition(moveTowards);
            sqrRemainingDistance = (rb2d.position - end).sqrMagnitude;
            yield return null;
        }

        _ends.Remove(end);
    }

    protected virtual bool AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
        {
            return true;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
        
        return canMove;
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
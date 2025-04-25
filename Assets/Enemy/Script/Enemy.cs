using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rig;
    public BoxCollider2D box;
    public Transform headPoint;
    public Transform collPoint;

    [Header("Stats")]
    public float speed;
    public int health;

    [Header("Hit Settings")]
    public float headArea;
    public LayerMask playerLayer;
    public float throwPlayerForce;

    private bool isRight;
    private Vector2 direction;


    void Update()
    {
        // Decide direção e espelha sprite
        if (isRight)
        {
            direction = Vector2.right;
            transform.eulerAngles = new Vector2(0, 180);
        }
        else
        {
            direction = -Vector2.right;
            transform.eulerAngles = new Vector2(0, 0);
        }

        CheckDeath();
    }

    void FixedUpdate()
    {
        // Move o inimigo
        rig.MovePosition(rig.position + direction * speed * Time.deltaTime);
        HandleHit();
    }

    void HandleHit()
    {
        // Verifica se o Player colidiu com a "cabeça" (acima)
        Collider2D hit = Physics2D.OverlapCircle(headPoint.position, headArea, playerLayer);
        // Verifica se colidiu de frente/por trás
        Collider2D hitPlayer = Physics2D.OverlapCircle(collPoint.position, headArea, playerLayer);

        if (hit != null)
        {
            var playerComp = hit.GetComponent<Player>();
            if (playerComp != null && !playerComp.vulnerable)
            {
                health--;
                anim.SetTrigger("hit");
                hit.GetComponent<Rigidbody2D>()
                   .AddForce(Vector2.up * throwPlayerForce, ForceMode2D.Impulse);
            }
        }

        if (hitPlayer != null)
        {
            var playerComp = hitPlayer.GetComponent<Player>();
            if (playerComp != null)
            {
                // Chama o método com o nome correto (case-sensitive)
                playerComp.GenerateDamage();
            }
        }
    }

    void CheckDeath()
    {
        if (health <= 0)
        {
            anim.SetTrigger("die");
            speed = 0f;
            Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Inverte direção ao colidir com obstáculo (layer 9)
        if (collision.gameObject.layer == 9)
        {
            isRight = !isRight;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(headPoint.position, headArea);
        Gizmos.DrawWireSphere(collPoint.position, headArea);
    }
}


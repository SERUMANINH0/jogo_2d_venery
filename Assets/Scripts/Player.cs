using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D    rig;
    public Animator       anim;
    public SpriteRenderer spriteRenderer;
    private GameController gc;

    [Header("Stats")]
    public float speed     = 5f;
    public float powerJump = 12f;
    public int   health    = 10;
    public bool  vulnerable;

    // estado interno de pulo
    bool isJumping;
    float timeBlink = 0.2f;

    void Start()
    {
        gc = FindObjectOfType<GameController>();
        // Inicializamos a barra com a vida cheia
        //gc.LoseHealth(health);
    }

    void Update()
    {
        // --- Movimento horizontal
        float direction = Input.GetAxisRaw("Horizontal");
        rig.velocity = new Vector2(direction * speed, rig.velocity.y);

        // vira o sprite
        spriteRenderer.flipX = (direction < 0f);

        // --- Animações de corrida/idle
        if (!isJumping)
        {
            if (direction != 0f) anim.SetInteger("transition", 1);
            else                 anim.SetInteger("transition", 0);
        }

        // --- Pulo
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rig.AddForce(Vector2.up * powerJump, ForceMode2D.Impulse);
            isJumping = true;
            anim.SetInteger("transition", 2);
        }
    }

    // Reseta o isJumping quando colidir com o chão (layer 8)
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 8)
            isJumping = false;
    }

    // Chamado externamente quando tomar dano
    public void GenerateDamage(int amount = 1)
    {
        if (vulnerable) return;

        health -= amount;                  // reduz vida
        gc.LoseHealth(health);             // atualiza barra
        vulnerable = true;
        StartCoroutine(RespawnBlink());
    }

    IEnumerator RespawnBlink()
    {
        // piscadas
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(timeBlink);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(timeBlink);
        }
        vulnerable = false;
    }
}

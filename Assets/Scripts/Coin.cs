using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Value")]
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameController.Instance != null)
        {
            GameController.Instance.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}



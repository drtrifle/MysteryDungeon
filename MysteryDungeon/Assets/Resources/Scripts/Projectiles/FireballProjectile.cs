using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FireballProjectile : Projectile {

    private Rigidbody2D rb2d;
    public float moveSpeed;

    new void Start() {
        base.Start();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        rb2d.velocity = -transform.right * moveSpeed;
    }

    protected override void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Enemy")) {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(1);
        }

        Destroy(gameObject); 
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.TakeDamage(1);
        }

        Destroy(gameObject);    
    }
}

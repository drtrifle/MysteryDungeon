using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float destroyAfterSeconds;

    // Use this for initialization
    protected void Start() {
        StartCoroutine("SelfDestructSequence");
    }

    IEnumerator SelfDestructSequence() {
        yield return new WaitForSecondsRealtime(destroyAfterSeconds);
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider) {
        Destroy(gameObject);

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        Destroy(gameObject);
    }

}
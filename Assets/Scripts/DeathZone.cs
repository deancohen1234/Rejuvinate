using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeathZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggering");
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            Debug.Log("Played Died");
            HealthComponent healthComponent = other.gameObject.GetComponent<HealthComponent>();
            healthComponent.DealDamage(1000.0f);
        }
    }
}

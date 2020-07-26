using System;
using CompleteProject;
using UnityEngine;


public class Grenade : MonoBehaviour
{
    [SerializeField] private float timeToExplode = 3.0f;
    [SerializeField] private int damage = 20;

    [SerializeField] private int shootableLayer;
    
    private SphereCollider _sphereCollider;

    private float _timer = 0.0f;

    private bool destroyNextFrame = false;
    
    private void Start()
    {
        foreach (var sphereCollider in GetComponents<SphereCollider>())
        {
            if (!sphereCollider.isTrigger) continue;
            _sphereCollider = sphereCollider;
            break;
        }

        _sphereCollider.enabled = false;
    }

    private void Update()
    {
        if (destroyNextFrame) Destroy(gameObject);
        
        _timer += Time.deltaTime;

        if (_timer < timeToExplode) return;
        
        _sphereCollider.enabled = true;
        destroyNextFrame = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var g = other.gameObject;
        if (g.layer != shootableLayer) return;
        
        
        var enemyHealth = g.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage, g.transform.position);
        }
    }
}

using System;
using CompleteProject;
using UnityEngine;


public class Grenade : MonoBehaviour
{
    [SerializeField] private float timeToExplode = 3.0f;
    [SerializeField] private int damage = 20;

    [SerializeField] private int shootableLayer;

    [SerializeField] private Material level0Material;
    [SerializeField] private Material level5Material;
    [SerializeField] private Material level10Material;
    
    private SphereCollider _sphereCollider;

    private int _level = 0;
    
    private float _timer = 0.0f;

    private bool _destroyNextFrame = false;

    public void Initialize(int dmg, int level)
    {
        damage = dmg;
        _level = level;
    }
    
    private void Start()
    {
        foreach (var sphereCollider in GetComponents<SphereCollider>())
        {
            if (!sphereCollider.isTrigger) continue;
            _sphereCollider = sphereCollider;
            break;
        }

        _sphereCollider.enabled = false;

        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            if (_level == 10)
            {
                meshRenderer.material = level10Material;
            }
            else if (_level < 5)
            {
                meshRenderer.material = level0Material;
            }
            else
            {
                meshRenderer.material = level5Material;
            }
        }
    }

    private void Update()
    {
        if (_destroyNextFrame) Destroy(gameObject);
        
        _timer += Time.deltaTime;

        if (_timer < timeToExplode) return;
        
        _sphereCollider.enabled = true;
        _destroyNextFrame = true;
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

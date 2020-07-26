using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
        [SerializeField] private int damagePerShot = 20;                  // The damage inflicted by each bullet.
        [SerializeField] private float timeBetweenBullets = 0.15f;        // The time between each shot.
        [SerializeField] private float range = 100f;                      // The distance the gun can fire.
        [SerializeField] private float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

        
        [SerializeField] private float grenadeSpeed = 500.0f;
        [SerializeField] private float playerGrenadeSpeed = 10.0f;
        [SerializeField] private float timeBetweenGrenades = 10.0f;


        [SerializeField] private LayerMask shootableMask;
        
        [SerializeField] private Light faceLight;								// Duh
        
        [SerializeField] private Material gunLineLevel5;
        [SerializeField] private Material gunLineLevel10;
        
        
        [SerializeField] private Rigidbody playerRigidbody;
        [SerializeField] private GameObject grenade;
        
        
        private int _grenadeDamage = 20;
        
        private Transform _transform;

        private Vector3 _lastPos;

        private float _grenadeTimer = 0.0f;
        private int _grenadeLevel = 0;
        
        private float _gunTimer;                                    // A timer to determine when to fire.
        private Ray _shootRay = new Ray();                       // A ray from the gun end forwards.
        private RaycastHit _shootHit;                            // A raycast hit to get information about what was hit.
        private int _shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        private ParticleSystem _gunParticles;                    // Reference to the particle system.
        private LineRenderer _gunLine;                           // Reference to the line renderer.
        private AudioSource _gunAudio;                           // Reference to the audio source.
        private Light _gunLight;                                 // Reference to the light component. 
        
        public void UpgradeGunFirerate(int level)
        {
            timeBetweenBullets /= 2.0f;
        }

        public void UpgradeGunDamage(int level)
        {
            damagePerShot *= 2;
            switch (level)
            {
                case 5:
                    _gunLine.material = gunLineLevel5;
                    _gunLight.color = faceLight.color = Color.red;
                    break;
                case 10:
                    _gunLine.material = gunLineLevel10;
                    _gunLight.color = faceLight.color = Color.blue;
                    break;
            }
        }

        public void UpgradeGrenadeFirerate(int level)
        {
            timeBetweenGrenades /= 2.0f;
            
            Debug.Log("Upgraded grenade!");
        }

        public void UpgradeGrenadeDamage(int level)
        {
            _grenadeDamage *= 2;
            _grenadeLevel = level;
        }

        private void Awake ()
        {
            // Create a layer mask for the Shootable layer.
            _shootableMask = shootableMask.value;

            // Set up the references.
            _gunParticles = GetComponent<ParticleSystem> ();
            _gunLine = GetComponent <LineRenderer> ();
            _gunAudio = GetComponent<AudioSource> ();
            _gunLight = GetComponent<Light> ();
			//faceLight = GetComponentInChildren<Light> ();

            _transform = GetComponent<Transform>();

            _lastPos = playerRigidbody.position;
        }


        private void Update ()
        {
            // Add the time since Update was last called to the timer.
            _gunTimer += Time.deltaTime;
            _grenadeTimer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
			if(Input.GetButton ("Fire1") && _gunTimer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot ();
            }

            if (Input.GetButton("Fire2") && _grenadeTimer >= timeBetweenGrenades) 
            {
                ThrowGrenade();
            }
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(_gunTimer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }

            _lastPos = playerRigidbody.position;
        }


        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            _gunLine.enabled = false;
			faceLight.enabled = false;
            _gunLight.enabled = false;
        }


        private void ThrowGrenade()
        {
            _grenadeTimer = 0.0f;
            
            var g = Instantiate(grenade, _transform.position, _transform.rotation);
            var rigid = g.GetComponent<Rigidbody>();
            var gs = g.GetComponent<Grenade>();

            gs.Initialize(_grenadeDamage, _grenadeLevel);


            var playerVelocity = _lastPos - playerRigidbody.position;
            
            rigid.velocity = (-(playerVelocity * playerGrenadeSpeed)) + (g.transform.forward * grenadeSpeed);

        }
        
        private void Shoot ()
        {
            // Reset the timer.
            _gunTimer = 0f;

            // Play the gun shot audioclip.
            _gunAudio.Play ();

            // Enable the lights.
            _gunLight.enabled = true;
			faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            _gunParticles.Stop ();
            _gunParticles.Play ();

            // Enable the line renderer and set it's first position to be the end of the gun.
            _gunLine.enabled = true;
            _gunLine.SetPosition (0, transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            _shootRay.origin = _transform.position;
            _shootRay.direction = _transform.forward;
            
            
            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if(Physics.Raycast (_shootRay, out _shootHit, range, _shootableMask))
            {
                //LayerMask.
                
                //_shootHit.collider.gameObject.layer.
                
                // Try and find an EnemyHealth script on the gameobject hit.
                var enemyHealth = _shootHit.collider.GetComponent<EnemyHealth>();

                // If the EnemyHealth component exist...
                if(enemyHealth != null)
                {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage (damagePerShot, _shootHit.point);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                _gunLine.SetPosition (1, _shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                _gunLine.SetPosition (1, _shootRay.origin + _shootRay.direction * range);
            }
        }
    }
}
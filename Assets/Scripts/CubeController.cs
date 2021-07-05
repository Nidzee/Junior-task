using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public static CubeController Instance;       // Static reference. Means we have only 1 cube and 1 cube-controller 
    
    private Rigidbody _rb;                       // RigidBody component reference
    private float _moveSpeed;                    // Speed of movement
    private float _maxSpeed;                     // Speed of movement
    private float _minSpeed;                     // Speed of movement
    private float _rotSpeed;                     // Speed of rotation
    private float _distanceToDecreaseSpeed;
    private Transform _target;                   // Current target to move to
    private List<Transform> _targets;            // List of targets on plane
    
    [SerializeField]
    private ParticleSystem _particles;           // Particles - to create after sphere collecting

    private delegate void _PlayerCollectsSphere();                        // Delegate to fire event of collecting sphere
    private event _PlayerCollectsSphere _OnSphereCollected = delegate{ };  // Event of collecting sphere


    private void Start()
    {
        Instance = this;                  // Setting single reference to this instance
        _rb = GetComponent<Rigidbody>();  // Cashing physic component
        _moveSpeed = 0;                   // Setting current move speed "Cube"
        _rotSpeed = 3f;                   // Setting rotation speed for "Cube"
        _maxSpeed = 6f;                   // Setting maximum speed for "Cube"
        _minSpeed = 3f;                   // Setting minimum speed for "Cube"
        _targets = new List<Transform>(); // Setting new list of targets (empty)
        _OnSphereCollected += ScoreManager.Instance.IncreaseAndUpdateScore; // Subscribe to function of incrementing UI score
    }

    private void Update()
    {
        RotateCubeTowardsTarget();
    }

    private void FixedUpdate()
    {
        MoveCubeTowardsTarget();
    }

    private void RotateCubeTowardsTarget()
    {
        // If we have target - rotate towards it
        if (_target)
        {
            Vector3 lookDirection = Vector3.RotateTowards(transform.forward, _target.position - transform.position, _rotSpeed * Time.deltaTime, 0.0f);  // Set direction to look
            transform.rotation = Quaternion.LookRotation(lookDirection);                                                                              // Turn "Cube" towards _target
        }
    }

    private void MoveCubeTowardsTarget()
    {
        // If we have target - move towards it
        if (_target)
        {
            CalculateSpeedChange();

            _rb.velocity = transform.forward * _moveSpeed;
        }
    }

    private void CalculateSpeedChange()
    {
        // Speed will increase till maximumSpeed value on path to target in 2/3 of the particular path
        // After 2/3 of the path - speed begins to decrease to reach minimum speed value

        float distance = Vector3.Distance(transform.position, _target.position); // Calculate distance between objects

        // Accelerate - if we are on first 2/3 of the path
        if (distance > _distanceToDecreaseSpeed)
        {
            _moveSpeed += 0.1f; // Increasing speed 

            if (_moveSpeed > _maxSpeed) // Clamp speed 
            {
                _moveSpeed = _maxSpeed;
            }
        }
        // Decelerate - if we are on last 1/3 of the path
        else
        {
            _moveSpeed -= 0.1f; // Decrease speed

            if (_moveSpeed < _minSpeed) // Clamp speed
            {
                _moveSpeed = _minSpeed;
            }
        }
    }

    public void AddNewTarget(Transform newTarget)
    {
        _targets.Add(newTarget); // Add new target to list of targets
        
        GetNearestTarget();      // Set nearest target to be current one
    }

    private void GetNearestTarget()
    {
        if (_targets.Count != 0)
        {
            // If we had target but we get new one in our list - we must compare distances between current target and new one
            if (_target)
            {
                // If we already had target - compare distances between nearest (current) target and "new target" (last in list)
                float currentDistance = Vector3.Distance(transform.position, _target.position);                       // Set current distance between "Cube" and current target
                float newTargetDistance = Vector3.Distance(transform.position, _targets[_targets.Count-1].position);  // Set distance between "Cube" and new unit in targets list

                if (newTargetDistance < currentDistance)                // If distance to new target is lower than to current target
                {
                    _target = _targets[_targets.Count-1];               // Set new target as current
                    _distanceToDecreaseSpeed = newTargetDistance / 3;   // Set distance to start decreasing speed
                }
            }
            // If target is null: 
            // (1) we reached previous target
            // (2) we didnt have target at all
            else
            {
                // If we reached previous target - we must find new one which is nearest from all list
                if (_targets.Count > 1)
                {
                    float distance = float.MaxValue;

                    float tempDistance = 0; // Variable to store distance between "Cube" and particular target from list of targets

                    foreach(var i in _targets)
                    {
                        tempDistance = Vector3.Distance(transform.position, i.position); // Distance between cube and particular target from list
                     
                        // If distance between current target is greater then between particular target from list
                        if (tempDistance < distance)
                        {
                            _target = i;                             // Set current target to sphere which is closer
                            distance = tempDistance;                 // Set temp variable for calculations
                            _distanceToDecreaseSpeed = distance / 3; // Set distance to begin decreasing speed
                        }
                    }   
                }
                // List count is 1 - means we just got first target in list or list has last target to go to
                else
                {
                    float newTargetDistance = Vector3.Distance(transform.position, _targets[0].position);  // Set new target distance
                    _target = _targets[0];                                                                 // Set target
                    _distanceToDecreaseSpeed = newTargetDistance / 3;                                      // Set distance to start decreasing speed
                }
            }
        }
        // If the list is empty now - stop moving
        else
        {
            _target = null;                      // Set target to null to stop moving and rotating
            _rb.velocity = Vector3.zero;         // Stop moving the "Cube"
            _moveSpeed = 0;                      // Set speed value to 0
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Sphere"))
        {
            _OnSphereCollected();                // Increments UI score
            
            // If we suddenly hit sphere which wasn't our target - check if it was target to prevent from errors
            if (other.transform == _target)
            {
                _target = null;                  // Set target to null
            }

            _targets.Remove(other.transform);    // Remove current target from list

            Destroy(other.gameObject);           // Destroy sphere

            Instantiate(_particles, transform);  // Instantiate particles
            
            GetNearestTarget();                  // Set new target to move to
        }
    }
}
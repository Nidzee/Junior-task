using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager Instance;  // Reference to this manager

    [SerializeField]
    private GameObject _objectToSpawnOn;      // Plane reference to get size of it
    
    [SerializeField]
    private GameObject _prefabToSpawn;        // Prefab to spawn on plane ("Sphere")

    private float _sizeOfPlaneX;              // Size of plane to calculate random position on plane
    private float _sizeOfPlaneZ;              // Size of plane to calculate random position on plane

    private delegate void _SphereSpawned(Transform sphere);       // Delegate to fire off when sphere is created
    private event _SphereSpawned _OnSphereSpawned = delegate{ };   // Event which fires off when sphere is created


    private void Start()
    {
        Instance = this; // Setting reference to this instance

        // Cashing size of plane to spawn prefab on
        _sizeOfPlaneX = _objectToSpawnOn.transform.localScale.x;
        _sizeOfPlaneZ = _objectToSpawnOn.transform.localScale.z;

        _OnSphereSpawned += CubeController.Instance.AddNewTarget;
    }

    private void Update()
    {
        // Detecting mouse clicking
        if (Input.GetMouseButtonDown(0))
        {
            // Spawning sphere in random position
            SpawnSphereInRandomPosition();
        }
    }

    private void SpawnSphereInRandomPosition()
    {
        // Calculating random position
        Vector3 randomPosition = new Vector3(Random.Range(-_sizeOfPlaneX/2, _sizeOfPlaneX/2), 0, Random.Range(-_sizeOfPlaneZ/2, _sizeOfPlaneZ/2));

        // Instantiating prefab on calculated position
        GameObject sphere = Instantiate(_prefabToSpawn, randomPosition, Quaternion.identity); // Can be added sphere-pooler if necessary

        // Add new sphere to Cube's list of targets
        _OnSphereSpawned(sphere.transform);
    }
}
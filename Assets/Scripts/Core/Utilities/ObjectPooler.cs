using System.Collections.Generic;
using UnityEngine;

namespace Core.Utilities
{
    /// <summary>
    /// Manages pooling of GameObjects to optimize performance by reusing objects.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        public static ObjectPooler Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        #region Inspector Variables

        [Header("Pools")]
        [SerializeField] private List<Pool> pools;

        #endregion

        #region Private Variables

        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<string, List<GameObject>> activePools;

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Persist across scenes if needed
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializePools();
        }

        #endregion

        #region Pool Initialization

        /// <summary>
        /// Initializes the object pools.
        /// </summary>
        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            activePools = new Dictionary<string, List<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                List<GameObject> activeList = new List<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
                activePools.Add(pool.tag, activeList);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves an object from the pool with the specified tag.
        /// </summary>
        /// <param name="tag">Tag identifying the pool.</param>
        /// <param name="position">Position to spawn the object.</param>
        /// <param name="rotation">Rotation to spawn the object.</param>
        /// <returns>Reused GameObject from the pool.</returns>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"ObjectPooler: Pool with tag {tag} doesn't exist.");
                return null;
            }

            if (poolDictionary[tag].Count == 0)
            {
                Debug.LogWarning($"ObjectPooler: Pool with tag {tag} is empty. Consider increasing the pool size.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Optional: Reset object state here if needed

            activePools[tag].Add(objectToSpawn);

            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
            pooledObj?.OnObjectSpawn();

            return objectToSpawn;
        }

        /// <summary>
        /// Returns an object back to its pool.
        /// </summary>
        /// <param name="tag">Tag identifying the pool.</param>
        /// <param name="objectToReturn">GameObject to return to the pool.</param>
        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"ObjectPooler: Pool with tag {tag} doesn't exist.");
                return;
            }

            objectToReturn.SetActive(false);
            poolDictionary[tag].Enqueue(objectToReturn);
            activePools[tag].Remove(objectToReturn);
        }

        #endregion
    }

    /// <summary>
    /// Interface for pooled objects to implement custom behavior upon spawning.
    /// </summary>
    public interface IPooledObject
    {
        void OnObjectSpawn();
    }
}

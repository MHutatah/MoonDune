public interface IPooledObject
{
    void OnObjectSpawn();   // Invoked when the object is retrieved from the pool
    void OnObjectReturn();  // Invoked when the object is returned to the pool
}

namespace BlApi;

/// <summary>
/// This interface provides actions to register (add) and unregister (remove) observers
/// for changes in a list of entities and in a speecific entity
/// </summary>
public interface IObservable //stage 5
{
    /// <summary>
    /// Register observer for changes in a list of entities
    /// </summary>
    /// <param name="listObserver">the observer method to be registered</param>
    void AddObserver(Action listObserver);
    /// <summary>
    /// Register observer for changes in a specific entity instance
    /// </summary>
    /// <param name="id">the identifier of the entity instance to be observed</param>
    /// <param name="observer">the observer method to be registered</param>
    void AddObserver(int id, Action observer);
    /// <summary>
    /// Unregister observer for changes in a list of entities
    /// </summary>
    /// <param name="listObserver">the observer method to be unregistered</param>
    void RemoveObserver(Action listObserver);
    /// <summary>
    /// Unregister observer for changes in a specific entity instance
    /// </summary>
    /// <param name="id">the identifier of the entity instance that was observed</param>
    /// <param name="observer">the observer method to be unregistered</param>
    void RemoveObserver(int id, Action observer);
}
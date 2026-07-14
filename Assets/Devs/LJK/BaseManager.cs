using UnityEngine;

public abstract class BaseManager<T>: MonoBehaviour where T : BaseManager<T>
{
    public abstract void Initialize();
}

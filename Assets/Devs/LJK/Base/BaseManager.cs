using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseManager<T>: MonoBehaviour where T : BaseManager<T>
{
    public abstract UniTask InitializeAsync();
}

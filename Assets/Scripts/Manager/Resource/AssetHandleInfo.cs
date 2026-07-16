using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetHandleInfo
{
    public AsyncOperationHandle Handle { get; }
    public int ReferenceCount { get; private set; }

    public AssetHandleInfo(AsyncOperationHandle handle)
    {
        Handle = handle;
        ReferenceCount = 0;
    }

    public void AddReferenceCount()
    {
        ReferenceCount++;
    }

    public void RemoveReferenceCount()
    {
        if (ReferenceCount > 0)
        {
            ReferenceCount--;
        }
    }
}
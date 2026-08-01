using UnityEngine;

public class StageMonsterPartyBillboard : MonoBehaviour
{
    private Camera _camera;

    private void LateUpdate()
    {
        Camera targetCamera = GetTargetCamera();

        if (null == targetCamera)
        {
            return;
        }

        Vector3 direction = targetCamera.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private Camera GetTargetCamera()
    {
        if (null != _camera && _camera.isActiveAndEnabled)
        {
            return _camera;
        }

        _camera = Camera.main;

        return _camera;
    }
}

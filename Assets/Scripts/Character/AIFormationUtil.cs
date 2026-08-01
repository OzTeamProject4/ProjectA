using UnityEngine;
using UnityEngine.AI;

public static class AIFormationUtil
{
    private const int MaxPartySize = 3;
    private const float NavMeshSampleRadius = 5.0f;

    public static Vector3 GetFormationPosition(Vector3 targetPosition, int slotIndex, float radius)
    {
        if (radius <= 0f)
        {
            return targetPosition;
        }

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit originHit, NavMeshSampleRadius, NavMesh.AllAreas) == false)
        {
            return targetPosition;
        }

        Vector3 origin = originHit.position;

        float angle = slotIndex * (360f / MaxPartySize);
        Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * radius;
        Vector3 desiredPosition = origin + offset;

        if (NavMesh.Raycast(origin, desiredPosition, out NavMeshHit blockHit, NavMesh.AllAreas))
        {
            return targetPosition;
        }

        if (NavMesh.SamplePosition(desiredPosition, out NavMeshHit sampleHit, NavMeshSampleRadius, NavMesh.AllAreas))
        {
            return sampleHit.position;
        }

        return targetPosition;
    }
}
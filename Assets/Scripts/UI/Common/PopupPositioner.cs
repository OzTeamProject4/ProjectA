using UnityEngine;

public static class PopupPositioner
{
    public static void ClampInsideCanvas(RectTransform popupRect)
    {
        if (popupRect == null)
        {
            Debug.LogError($"[{nameof(PopupPositioner)}:{nameof(ClampInsideCanvas)}] popupRect가 null입니다.");
            return;
        }

        if (!TryGetCanvasRect(popupRect, out RectTransform canvasRect))
        {
            return;
        }

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        Vector3[] popupCorners = new Vector3[4];
        popupRect.GetWorldCorners(popupCorners);

        float offsetX = CalculateOffset(popupCorners[0].x, popupCorners[2].x, canvasCorners[0].x, canvasCorners[2].x);
        float offsetY = CalculateOffset(popupCorners[0].y, popupCorners[1].y, canvasCorners[0].y, canvasCorners[1].y);

        popupRect.position += new Vector3(offsetX, offsetY, 0f);
    }

    private static bool TryGetCanvasRect(RectTransform popupRect, out RectTransform canvasRect)
    {
        canvasRect = null;

        Canvas canvas = popupRect.GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning($"[{nameof(PopupPositioner)}:{nameof(TryGetCanvasRect)}] 상위에 Canvas가 없습니다. name={popupRect.name}");
            return false;
        }

        canvasRect = canvas.rootCanvas.transform as RectTransform;

        return canvasRect != null;
    }

    private static float CalculateOffset(float popupMin, float popupMax, float canvasMin, float canvasMax)
    {
        float offset = 0f;

        if (popupMax > canvasMax)
        {
            offset = canvasMax - popupMax;
        }

        if (popupMin + offset < canvasMin)
        {
            offset = canvasMin - popupMin;
        }

        return offset;
    }
}

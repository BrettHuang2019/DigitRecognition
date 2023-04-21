using UnityEngine;


public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    public void SetPosition(Vector2 pos)
    {
        if (CanAppend(pos))
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
        }
    }

    private bool CanAppend(Vector2 pos)
    {
        if (lineRenderer.positionCount == 0) return true;
        return Vector2.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), pos) >
               LineDrawer.RESOLUTION;
    }
}

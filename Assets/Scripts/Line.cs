using UnityEngine;


public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float resolution;
    [SerializeField] private float minAngle;

    public void AddPosition(Vector2 pos)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);
    }

    public bool CanAppend(Vector2 pos)
    {
        if (lineRenderer.positionCount == 0) return true;
        
        
        if(lineRenderer.positionCount == 1)
        {
            return Vector2.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), pos) >
                   resolution;
        }

        return Vector2.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), pos) >
            resolution && !IsAcuteAngle(pos);
    }

    public bool IsAcuteAngle(Vector2 pos)
    {
        if(lineRenderer.positionCount < 2) return false;
    
        Vector2 v1 = lineRenderer.GetPosition(lineRenderer.positionCount - 2) -
                     lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        Vector2 v2 = pos - (Vector2)lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        float angle = Vector2.Angle(v1, v2);
        return angle > 0 && angle < minAngle;
    }
}

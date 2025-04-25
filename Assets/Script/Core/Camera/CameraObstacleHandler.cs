using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleHandler : MonoBehaviour
{
    private LayerMask _obstacleMask;
    private List<TransparentObstacle> _previous = new();
    private List<TransparentObstacle> _current = new();

    private void Start()
    {
        _obstacleMask = LayerMask.GetMask("Obstacle");
    }

    public void CheckObstacles(Transform target)
    {
        _current.Clear();
        
        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir.normalized, dist, _obstacleMask);
        foreach (var hit in hits)
        {
            TransparentObstacle obstacle = hit.collider.GetComponent<TransparentObstacle>();
            if (obstacle != null)
            {
                obstacle.SetTransparent();
                _current.Add(obstacle);
            }
        }

        foreach (var prev in _previous)
        {
            if (!_current.Contains(prev))
                prev.Restore();
        }

        _previous = new List<TransparentObstacle>(_current);
    }
}

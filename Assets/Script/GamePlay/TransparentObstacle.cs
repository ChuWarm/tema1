using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentObstacle : MonoBehaviour
{
    private Material _mat;
    private Color _originalColor;

    private void Start()
    {
        _mat = GetComponent<MeshRenderer>().material;
        _originalColor = _mat.color;
    }

    public void SetTransparent()
    {
        Color c = _mat.color;
        c.a = 0.3f;
        _mat.color = c;
    }

    public void Restore()
    {
        _mat.color = _originalColor;
    }
}

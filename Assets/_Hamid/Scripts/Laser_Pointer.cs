using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Pointer : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Gradient _colorValid;
    [SerializeField] private Gradient _colorDenied;
    // Start is called before the first frame update


    private List<Vector3> _pointList = new List<Vector3>();
    private int _resolution = 24;

    public void Activate(bool activate)
    {

        _lineRenderer.gameObject.SetActive(activate);
    }

    public void DrawCurveValid(Vector3 start, Vector3 end)
    {
        DrawCurve(start, end, _colorValid);
    }
    public void DrawCurveDenied(Vector3 start, Vector3 end)
    {
        DrawCurve(start, end, _colorDenied);

    }

    public void DrawLine(Vector3 start, Vector3 end, Gradient color)
    {
        ClearCurve();

        for (float i = 0; i <= 1; i += 1.0f / _resolution)
        {

            Vector3 _beizerpoint = Vector3.Lerp(start, end, i);

            _pointList.Add(_beizerpoint);
        }

        _lineRenderer.positionCount = _pointList.Count;
        _lineRenderer.SetPositions(_pointList.ToArray());
        _lineRenderer.colorGradient = color;

    }


    public void DrawCurve(Vector3 start, Vector3 end, Gradient color)
    {
        _pointList.Clear();
        for (float i = 0; i <= 1; i += 1.0f / _resolution)
        {
            Vector3 _middlePoint = new Vector3((start.x + end.x) / 2, start.y, (start.z + end.z) / 2);
            Vector3 _tangantStart = Vector3.Lerp(start, _middlePoint, i);
            Vector3 _tangentEnd = Vector3.Lerp(_middlePoint, end, i);
            Vector3 _beizerpoint = Vector3.Lerp(_tangantStart, _tangentEnd, i);

            _pointList.Add(_beizerpoint);
        }

        _lineRenderer.positionCount = _pointList.Count;
        _lineRenderer.SetPositions(_pointList.ToArray());
        _lineRenderer.colorGradient = color;

    }

    private void ClearCurve()
    {
        _pointList.Clear();
    }


}
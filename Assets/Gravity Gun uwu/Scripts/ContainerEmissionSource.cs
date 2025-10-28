using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ContainerEmissionSource : MonoBehaviour
{
    [Header("Configuración de Emisión")]
    [SerializeField, Tooltip("Número de puntos de emisión por lado")]
    [Range(1, 5)]
    private int _emissionPointsPerSide = 3;
    
    [SerializeField, Tooltip("Desplazamiento desde el borde superior (0-1)")]
    [Range(0f, 0.5f)]
    private float _topOffset = 0.1f;
    
    [SerializeField, Tooltip("Mostrar gizmos de puntos de emisión")]
    private bool _showEmissionPoints = true;

    private BoxCollider _boxCollider;
    
    public BoxCollider BoxCollider => _boxCollider;
    public int EmissionPointsPerSide => _emissionPointsPerSide;
    public float TopOffset => _topOffset;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public Vector3 GetEmissionPoint(Vector3 tiltDirection)
    {
        if (_boxCollider == null) return transform.position;

        Bounds bounds = _boxCollider.bounds;
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        float dotX = Vector3.Dot(tiltDirection, Vector3.right);
        float dotZ = Vector3.Dot(tiltDirection, Vector3.forward);
        
        Vector3 emissionPoint = center;
        
        if (Mathf.Abs(dotX) > Mathf.Abs(dotZ))
        {
            emissionPoint.x += Mathf.Sign(dotX) * size.x * 0.5f;
            
            if (_emissionPointsPerSide > 1)
            {
                float zVariation = Random.Range(-size.z * 0.3f, size.z * 0.3f);
                emissionPoint.z += zVariation;
            }
        }
        else
        {
            emissionPoint.z += Mathf.Sign(dotZ) * size.z * 0.5f;
            
            if (_emissionPointsPerSide > 1)
            {
                float xVariation = Random.Range(-size.x * 0.3f, size.x * 0.3f);
                emissionPoint.x += xVariation;
            }
        }
        
        emissionPoint.y = bounds.max.y - (size.y * _topOffset);
        
        return emissionPoint;
    }

    public Vector3[] GetMultipleEmissionPoints(Vector3 tiltDirection)
    {
        Vector3[] points = new Vector3[_emissionPointsPerSide];
        
        if (_boxCollider == null)
        {
            for (int i = 0; i < points.Length; i++)
                points[i] = transform.position;
            return points;
        }

        Bounds bounds = _boxCollider.bounds;
        Vector3 center = bounds.center;
        Vector3 size = bounds.size;

        float dotX = Vector3.Dot(tiltDirection, Vector3.right);
        float dotZ = Vector3.Dot(tiltDirection, Vector3.forward);
        
        for (int i = 0; i < _emissionPointsPerSide; i++)
        {
            Vector3 point = center;
            
            if (Mathf.Abs(dotX) > Mathf.Abs(dotZ))
            {
                point.x += Mathf.Sign(dotX) * size.x * 0.5f;
                
                float t = _emissionPointsPerSide > 1 ? (float)i / (_emissionPointsPerSide - 1) : 0.5f;
                point.z += Mathf.Lerp(-size.z * 0.4f, size.z * 0.4f, t);
            }
            else
            {
                point.z += Mathf.Sign(dotZ) * size.z * 0.5f;
                
                float t = _emissionPointsPerSide > 1 ? (float)i / (_emissionPointsPerSide - 1) : 0.5f;
                point.x += Mathf.Lerp(-size.x * 0.4f, size.x * 0.4f, t);
            }
            
            point.y = bounds.max.y - (size.y * _topOffset);
            points[i] = point;
        }
        
        return points;
    }

    public bool IsValidEmissionPoint(Vector3 point)
    {
        if (_boxCollider == null) return false;
        
        return _boxCollider.bounds.Contains(point);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_showEmissionPoints || _boxCollider == null) return;

        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawCube(_boxCollider.bounds.center, _boxCollider.bounds.size);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(_boxCollider.bounds.center, _boxCollider.bounds.size);

        if (Application.isPlaying)
        {
            Vector3[] directions = { Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
            Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
            
            for (int d = 0; d < directions.Length; d++)
            {
                Vector3[] points = GetMultipleEmissionPoints(directions[d]);
                Gizmos.color = colors[d];
                
                foreach (Vector3 point in points)
                {
                    Gizmos.DrawSphere(point, 0.05f);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_boxCollider == null) return;
        
        Bounds bounds = _boxCollider.bounds;
        
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(bounds.center + Vector3.up * (bounds.size.y * 0.6f), 
            $"Emission Source\nPoints per side: {_emissionPointsPerSide}\nTop offset: {_topOffset:F2}");
    }
#endif
}
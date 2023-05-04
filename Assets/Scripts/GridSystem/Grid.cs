using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    public float size = 2.1f;
    [SerializeField]
    private float minValue = 0.3f;

    [SerializeField]
    private float maxValue = 0.7f;
    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        float decimalValue(float numero)
        {
            
            return (Mathf.Abs(numero) - Mathf.Abs((int)numero));
        }        
        var xCount = position.x / size;
        var yCount = position.y / size;
        var zCount = position.z / size;

        if (minValue < decimalValue(xCount) && decimalValue(xCount) > maxValue)
        {
            xCount = 99999f;
            //Debug.Log(xCount);
        }
        else{
            xCount = Mathf.Round(position.x / size) * size;
        }
        if (minValue < decimalValue(yCount) && decimalValue(yCount) < maxValue)
        {
            yCount = 99999f;
            //Debug.Log(yCount);
        }else{
            yCount = Mathf.Round(position.y / size) * size;
        }
        if (minValue < decimalValue(zCount) && decimalValue(zCount) < maxValue)
        {
            zCount = 99999f;
            //Debug.Log(zCount);
        }else{
            zCount = Mathf.Round(position.z / size) * size;
        }

        return new Vector3(xCount, yCount, zCount);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float x = -20; x < 20; x += size)
        {
            for (float z = -20; z < 20; z += size)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0f, z));
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}

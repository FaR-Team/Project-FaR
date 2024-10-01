using UnityEngine;
using UnityEngine.InputSystem;
using FaRUtils.Systems.GridSystem;

public class GridTestScript : MonoBehaviour
{
    public PlaceableGrid grid;
    public LayerMask interactMask;

    private Camera cam;

    public Transform cube;
    public float gridSize;

    public int heightSnap;
    public int gridSearchRange = 5;
    
    public Color GridRangeColor;
    public bool show3DRangeGrid = false;

    public enum GridOrientation { XY, XZ, YZ }

    public GridOrientation Orientation;

    private void OnValidate()
    {
        cube.localScale = Vector3.one * gridSize;
    }

    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.scroll.ReadValue().y > 0.1f)
        {
            heightSnap++;
        }

        if (Mouse.current.scroll.ReadValue().y < -0.1f)
        {
            heightSnap--;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 pos = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0);

            Ray ray = cam.ScreenPointToRay(pos);
            
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 10f, Color.red, 4f);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, interactMask, QueryTriggerInteraction.Collide))
            {
                var offsetPos = new Vector3(hitInfo.point.x, hitInfo.point.y + heightSnap, hitInfo.point.z);
                cube.position = WorldGrid.PositionXZFromWorldPoint2D(offsetPos, gridSize);
            }
        }
  
    }

    private void OnDrawGizmos()
    {
        switch (Orientation)
        {
            case GridOrientation.XY:
                foreach (var point in WorldGrid.GetXYGridPositionsWithinRange2D(cube.position, gridSize, gridSearchRange))
                {
                    Gizmos.color = GridRangeColor;
                    Gizmos.DrawWireCube(point, new Vector3(gridSize, gridSize, 0f));
                }
                break;
            case GridOrientation.XZ:
                foreach (var point in WorldGrid.GetXZGridPositionsWithinRange2D(cube.position, gridSize, gridSearchRange))
                {
                    Gizmos.color = GridRangeColor;
                    Gizmos.DrawWireCube(point, new Vector3(gridSize, 0f, gridSize));
                }
                break;
            case GridOrientation.YZ:
                foreach (var point in WorldGrid.GetYZGridPositionsWithinRange2D(cube.position, gridSize, gridSearchRange))
                {
                    Gizmos.color = GridRangeColor;
                    Gizmos.DrawWireCube(point, new Vector3(0f, gridSize, gridSize));
                }
                break;
            default:
                break;
        }
    }
}

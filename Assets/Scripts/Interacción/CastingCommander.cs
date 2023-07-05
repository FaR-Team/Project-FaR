using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class CastingCommander : MonoBehaviour
{
    public static NativeArray<RaycastCommand> rayCastCommands;
    public static NativeArray<RaycastHit> rayCastHits;

    public static NativeArray<SpherecastCommand> sphereCastCommands;
    public static NativeArray<RaycastHit> sphereCastHits;

    public static JobHandle jobHandle;

    [SerializeField] private float maxGrabDistance = 10f;
    static float _maxGrabDistance => instance.maxGrabDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layerDirt;
    static LayerMask _layerDirt => instance.layerDirt;
    
    [SerializeField] private GameObject cameraGo;
    static CastingCommander instance;
    static GameObject _camera => instance.cameraGo;
    

    void Awake()
    {
        instance = this;
        cameraGo = FindObjectOfType<Camera>().gameObject;
        rayCastCommands = new NativeArray<RaycastCommand>(3, Allocator.Persistent);
        rayCastHits = new NativeArray<RaycastHit>(3, Allocator.Persistent);
    }
    void OnDestroy()
    {
        jobHandle.Complete();
        rayCastCommands.Dispose();
        rayCastHits.Dispose();
    }
    void FixedUpdate()
    {
        jobHandle.Complete();

        rayCastCommands[0] = new RaycastCommand(cameraGo.transform.position, cameraGo.transform.forward, maxGrabDistance, layerMask);
        jobHandle = RaycastCommand.ScheduleBatch(rayCastCommands, rayCastHits, 1);
    }

    public static bool IsLookingAtDirt()
    {
        jobHandle.Complete();
        
        bool result = rayCastHits[1].collider != null;
        
        rayCastCommands[1] = new RaycastCommand(_camera.transform.position, _camera.transform.forward, _maxGrabDistance, _layerDirt);
        jobHandle = RaycastCommand.ScheduleBatch(rayCastCommands, rayCastHits, 1);
        
        return result;
    }
}

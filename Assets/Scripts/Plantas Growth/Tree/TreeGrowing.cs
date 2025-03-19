using System.Collections;
using UnityEngine;
using Utils;

public class TreeGrowing : GrowingTreeAndPlant //Crecimiento del árbol
{
    [SerializeField]
    private GameObject SkinnedTree;
    protected override void Start()
    {
        base.Start();
        CheckGrowState();
    }
    
    public void StartReGrowTree() // TODO: Podriamos moverlo a la clase padre, si funciona igual que el BushGrowing
    {
        ResetSpawnPoints();
        daysWithoutHarvest = 0;
        _reGrowCounter++;
        fruits.Clear();
        gameObject.layer = 3;
    }

    protected override void UpdateState(int state = 0)
    {
        this.Log("UpdateState");
        meshFilter.mesh = currentState.mesh;
        meshRenderer.material = currentState.material;
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = currentState.mesh;
        }
        if (currentState.isLastPhase)
        {
            if (meshRenderer != null)
                meshRenderer.enabled = false;
                
            if (SkinnedTree != null)
                SkinnedTree.gameObject.SetActive(true);
            
            if (fruits.Count > 0 && FruitsAreReady()) SetInteractable();
        }
        
        // Notify subscribers about the state change
        GrowthEventManager.Instance.NotifyGrowthStateChanged(this, currentState);
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        Destroy(gameObject);
        yield return new WaitForSeconds(0.5f);
    }

}

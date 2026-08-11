using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using FaRUtils.Systems.GridSystem;
using FaRUtils.Systems.Debris;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/ToolItem")]
public class ToolItemData : InventoryItemData
{
    [Header("Tool Specific")]
    public int energyCost = 1;
    
    public override ItemCategory Category => ItemCategory.Tool;
    
    public ToolType ToolType 
    { 
        get 
        {
            return typeOfItem switch
            {
                TypeOfItem.Hoe => ToolType.Hoe,
                TypeOfItem.Axe => ToolType.Axe,
                TypeOfItem.Bucket => ToolType.Bucket,
                TypeOfItem.Shovel => ToolType.Shovel,
                _ => ToolType.Hoe
            };
        }
    }

    public override ItemUseResult UseItem(ItemUseContext ctx)
    {
        bool toolUsedSuccessfully = UseItem();
        if (toolUsedSuccessfully)
        {
            return new ItemUseResult
            {
                Success = true,
                LockMovementDuration = 1f,
                TriggerPlowAnim = IsHoe() && ctx.HoeAnimator != null
            };
        }
        return default;
    }

    public override bool UseItem()
    {
        if (!PauseMenu.GameIsPaused)
        {
            if (IsHoe())
            {
                return UseHoe();
            }

            if (IsAxe())
            {
                //return UseAxe();
            }

            if (IsBucket())
            {
                return UseBucket();
            }

            if (IsShovel())
            {
                return UseShovel();
            }
        }
        return true;
    }

    private bool UseHoe()
    {
        if (GridGhost.instance.CheckDirt(GridGhost.instance.finalPosition, 0.1f) == null && 
            GridGhost.instance.CheckCrop(GridGhost.instance.finalPosition, 0.1f) == true)
        {
            if (Energy.RemainingEnergy < energyCost)
            {
                Energy.instance?.ShowNoEnergyFeedback();
                return false;
            }

            bool dirtPlanted = GridGhost.instance.PlantDirt();
            if (dirtPlanted)
            {
                Energy.instance?.TryUseAndAnimateEnergy(energyCost, 2f);
                return true;
            }
        }
        return false;
    }

    private bool UseBucket()
    {
        Dirt _dirt = GridGhost.instance.CheckDirt(GridGhost.instance.FinalPosition, 0.1f);
        if (_dirt != null)
        {
            if (Energy.RemainingEnergy < energyCost)
            {
                Energy.instance?.ShowNoEnergyFeedback();
                return false;
            }

            _dirt.DirtIsWet();
            Energy.instance?.TryUseAndAnimateEnergy(energyCost, 2f);
            return true;
        }
        return false;
    }

    private bool UseShovel()
    {
        Dirt dirt = GridGhost.instance.CheckDirt(GridGhost.instance.FinalPosition, 0.1f);
        
        if (dirt != null)
        {
            if (Energy.RemainingEnergy < energyCost)
            {
                Energy.instance?.ShowNoEnergyFeedback();
                return false;
            }

            List<GameObject> targets = new List<GameObject>();
            targets.Add(dirt.gameObject);
            if (dirt.currentCrop != null)
            {
                targets.Add(dirt.currentCrop.gameObject);
            }
            Vector3 explosionPos = dirt.transform.position + Vector3.up * 0.35f;
            VoxelExplosionFX.Spawn(targets, explosionPos, 28);

            dirt.DestroyDirtAndCrop();
            Energy.instance?.TryUseAndAnimateEnergy(energyCost, 2f);
            return true;
        }
        else
        {
            Debris debris = GridGhost.instance.CheckDebris(GridGhost.instance.FinalPosition, 0.1f);
            if (debris != null && debris.Category != DebrisCategory.Wood)
            {
                if (Energy.RemainingEnergy < energyCost)
                {
                    Energy.instance?.ShowNoEnergyFeedback();
                    return false;
                }

                Vector3 explosionPos = debris.transform.position + Vector3.up * 0.35f;
                VoxelExplosionFX.Spawn(debris.gameObject, explosionPos, 28);

                Destroy(debris.gameObject);
                Energy.instance?.TryUseAndAnimateEnergy(energyCost, 2f);
                return true;
            }
        }
        
        return false;
    }
}

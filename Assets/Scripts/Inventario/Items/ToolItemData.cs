using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/ToolItem")]
public class ToolItemData : InventoryItemData
{
    public int energyCost = 1;
   // private PauseMenu _pauseMenu;

    public override bool UseItem()
    {
        //_pauseMenu = PauseMenu.Instance;
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
            if(Energy.instance.TryUseAndAnimateEnergy(energyCost, 2f))
            {
                bool dirtPlanted = GridGhost.instance.PlantDirt();
                return dirtPlanted;
            }
            else 
            {
                return false;
            }
        }
        else 
        {
            return false;
        }
    }

    private bool UseBucket()
    {
        Dirt _dirt = GridGhost.instance.CheckDirt(GridGhost.instance.FinalPosition, 0.1f);
        if (_dirt != null)
        {
            _dirt.DirtIsWet();
            return true;
        }
        else return false;
    }

    private bool UseShovel()
    {
        Dirt dirt = GridGhost.instance.CheckDirt(GridGhost.instance.FinalPosition, 0.1f);
        
        // Check if dirt has rotten crop.
        if (dirt != null && dirt.currentCrop && dirt.currentCrop.IsDead)
        {
            dirt.DestroyDirtAndCrop();
            return true;
        }

        // Check if dirt doesn't have crop.
        if (dirt != null && dirt.currentCrop == null)
        {
            dirt.DestroyDirtAndCrop();
            return true;
        }
        
        return false;
    }
}

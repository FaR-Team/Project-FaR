using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                GridGhost.instance.PlantDirt();
                return true;
            }
            else return false;
        }
        else return false;
    }

    private bool UseBucket()
    {
        Dirt _dirt = GridGhost.instance.CheckDirt(GridGhost.instance.finalPosition, 0.1f);
        if (_dirt != null)
        {
            _dirt.DirtIsWet();
            return true;
        }
        else return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/ToolItem")]
public class ToolItemData : InventoryItemData
{

   // private PauseMenu _pauseMenu;
    private GridGhost _gridGhost()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<GridGhost>();
    }

    public override bool UseItem()
    {
        //_pauseMenu = PauseMenu.Instance;
        if (!PauseMenu.GameIsPaused)
        {
            if (IsHoe)
            {
                return UseHoe();
            }

            if (IsAxe)
            {
                //return UseAxe();
            }

            if (IsBucket)
            {
                return UseBucket();
            }
        }
        return true;
    }

    private bool UseBucket()
    {
        Dirt _dirt = _gridGhost().CheckDirt(_gridGhost().finalPosition, 0.1f);
        if (_dirt != null)
        {
            _dirt._isWet = true;

            _dirt.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Dirt.wetDirtColor;
            return true;
        }
        else return false;
    }

    private bool UseHoe()
    {
        if (_gridGhost().CheckDirt(_gridGhost().finalPosition, 0.1f) == null)
        {
            _gridGhost().PlantDirt();
            return true;
        }
        else return false;
    }
}

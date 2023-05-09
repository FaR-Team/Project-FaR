using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Jueguito Granjil/Inventario/ToolItem")]
public class ToolItemData : InventoryItemData
{

    private GridGhost _gridGhost;
    private PauseMenu _pauseMenu;

    public override bool UseItem()
    {
        _pauseMenu = PauseMenu.Instance;
        if (!PauseMenu.GameIsPaused)
        {
            if (IsHoe)
            {
                _gridGhost = GameObject.FindGameObjectWithTag("Player").GetComponent<GridGhost>();

                if (_gridGhost.CheckDirt(_gridGhost.finalPosition, 0.1f) == null)
                {
                    _gridGhost.PlantDirt();
                    return true;
                }
                else return false;                
            }

            if (IsAxe)
            {
                //TODO: Funcionamiento del Hacha.
            }

            if (IsBucket)
            {
                //TODO: Funcionamiento de la regadera/balde.
            }

            if (IsUnknown1)
            {
                //TODO: Funcionamiento de ???.
            }

            if (IsUnknown2)
            {
                //TODO: Funcionamiento de ???.
            }
        }
        return true;
    }
}

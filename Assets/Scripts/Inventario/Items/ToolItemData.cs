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
        if (PauseMenu.GameIsPaused == false)
        {
            if (IsHoe == true)
            {
                _gridGhost = GameObject.FindGameObjectWithTag("Player").GetComponent<GridGhost>();

                if (_gridGhost.CheckDirt(_gridGhost.finalPosition, 0.1f) == true)
                {
                    _gridGhost.PlantDirt();
                }
                else
                {
                    return false;
                }
                return true;
            }

            if (IsAxe == true)
            {
                //TODO: Funcionamiento del Hacha.
            }

            if (IsBucket == true)
            {
                //TODO: Funcionamiento de la regadera/balde.
            }

            if (IsUnknown1 == true)
            {
                //TODO: Funcionamiento de ???.
            }

            if (IsUnknown2 == true)
            {
                //TODO: Funcionamiento de ???.
            }
        }
        return true;
    }
}

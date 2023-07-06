using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class GrowingFruitsTree : GrowingCrop //CrecimientoFruta
{
    
    void Awake(){
        isFruit = true;
    }
}
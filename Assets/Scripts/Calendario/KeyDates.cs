using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;

[CreateAssetMenu(menuName = "Jueguito Granjil/Calendario/Fecha Especial")]
public class KeyDates : ScriptableObject {
    
    public DateTime KeyDate;
    public bool Yearly;
    public Sprite thumbnail;
    public string Description;
}

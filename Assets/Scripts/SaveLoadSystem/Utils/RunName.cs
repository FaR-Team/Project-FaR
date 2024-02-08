using UnityEngine;

public class RunName : MonoBehaviour
{
    public static RunName instance;
    public string currentRunName { get; private set; }

    private void Awake()
    {
        instance= this;
        currentRunName = "Run Test"; 
        //Hardcodeado, despues esto debe estar puesto como algo dado por el nombre de run que le ponga el jugador.
    }
}

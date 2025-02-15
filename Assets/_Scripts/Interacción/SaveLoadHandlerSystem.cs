using System.Threading.Tasks;
using UnityEngine.Events;

public static class SaveLoadHandlerSystem
{
    
    
    #region Save

    private static readonly UnityEvent<bool> SaveDataEvent = new();


    //ESTO SE UNE, TODOS LOS OBJETOS LOS CUALES HEREDEN DE SAVER. ES DECIR TODOS LOS QUE SE GUARDAN.
    public static void AddListener(UnityAction<bool> action)
    {
        SaveDataEvent.AddListener(action);
    }

    //LA CAMA LO LLAMA EN FALSE.
    //Y EL CAMBIAR ESCENA EN TRUE.
    public static void Invoke(bool isTemporary)
    {
        SaveDataEvent.Invoke(isTemporary);

    }
    #endregion

    #region Load

    public static async Task ForceLoad()
    {
        ChestsManager.instance.Reload();
        await DirtSpawnerPooling.instance.Reload();
    }

    #endregion
}
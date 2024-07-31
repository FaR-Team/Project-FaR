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

        //la escena actual, tiene que cambiar su estado en base a !isTemporary.
        //  SetTemporary(isTemporary);
    }
    #endregion

    #region Load
    public enum GameSaveStatus
    {

        isTemporary,
        notTemporary
    }
    public static bool isCurrentTemp = false;

    private static GameSaveStatus gameStatus = GameSaveStatus.notTemporary;

    public static void SetTemporary(bool isTemporary)
    {
        gameStatus = isTemporary ?
            GameSaveStatus.isTemporary :
            GameSaveStatus.notTemporary;
    }

    public static bool IsTemporary()
    {
        return gameStatus.Equals(GameSaveStatus.isTemporary);
    }
    #endregion
}
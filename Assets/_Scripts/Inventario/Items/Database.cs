using System.Collections;
using System.Collections.Generic;
using FaRUtils.Systems.ItemSystem;
using UnityEngine;

public static class Database
{
    public static DatabaseSO itemDatabase = Resources.Load<DatabaseSO>("Database");
    public static GrowingStateDatabaseSO growingStateDatabase = Resources.Load<GrowingStateDatabaseSO>("GrowingStateDatabase");
}

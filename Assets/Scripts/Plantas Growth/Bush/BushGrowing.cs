using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using FaRUtils.Systems.DateTime;
public class BushGrowing : GrowingTreeAndPlant //Crecimiento del arbusto
{
    public Dirt Tierra = null;
    public GameObject TierraTexture = null;

    public override void Start()
    {
        base.Start();
        
        DateTime.OnHourChanged.AddListener(OnHourChanged);

        Tierra = transform.parent.gameObject.GetComponent<Dirt>();
        TierraTexture = transform.parent.GetChild(0).gameObject;
    }
    public override void OnHourChanged(int hour)
    {
        if (hour != 5) return;
        if ((Dia < meshes.Length) && Tierra._isWet)
        {
            Dia++;
        }
        CheckDayGrow();

        if ((Dia == meshes.Length) && Tierra._isWet)
        {
            DiaM += 1;
            if (DiaM == ExpectedInt)
            {
                gameObject.layer = 7;
            }
        }

        if (Dia == meshes.Length && fruits.Count == 0)
        {
            PonerFruto(2, 5);
        }
    }

    public override IEnumerator BushCedeLaPresidencia()
    {
        TierraTexture.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(Tierra.transform.parent.gameObject);
    }
}

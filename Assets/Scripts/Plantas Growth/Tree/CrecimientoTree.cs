using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;

public class CrecimientoTree : MonoBehaviour
{
    [Header("Prefabs de Fases")]
    public GameObject Fase1, Fase2, Fase3, Fase4, Fase5, Fase6, Fase7, Fase8, Fase9, Fase10, FaseFinal;

    [Header("Misc.")]
    public GameObject Reloj;
    public int Dia;
    public int DiaM;
    public bool yacrecio = false;

    [Header("Días para cambiar de fase")]
    public int Int1, Int2, Int3, Int4, Int5, Int6, Int7, Int8, Int9;

    [Header("Puntos de Aparición")]
    public GameObject SelectedSpawn = null;
    public GameObject Spawnpoint1, Spawnpoint2, Spawnpoint3, Spawnpoint4, Spawnpoint5, Spawnpoint6, Spawnpoint7, Spawnpoint8, Spawnpoint9, Spawnpoint10, Spawnpoint11, Spawnpoint12, Spawnpoint13, Spawnpoint14, Spawnpoint15, Spawnpoint16, Spawnpoint17, Spawnpoint18, Spawnpoint19, Spawnpoint20;

    public GameObject Tierra = null;

    public GameObject Frut1, Frut2, Frut3, Frut4, Frut5, Frut6, Frut7, Frut8, Frut9, Frut10, Frut11, Frut12, Frut13, Frut14, Frut15, Frut16, Frut17, Frut18, Frut19, Frut20;
    public int RandInt;
    public int ExpectedInt;
    public int ReGrow;
    public int ReGrowTimes;
    public GameObject Prefab;
    
    public bool[] boolList = new bool [20];
    public bool _alreadyRe = false;
    public bool yaeligio = false;
    public bool yaeligioCh = false;

    void Start()
    {
        
        Fase1.SetActive(true);
        Reloj = GameObject.FindGameObjectWithTag("Reloj");
        Dia = 0;
        Tierra = this.transform.root.gameObject;
        yacrecio = false;
    }

    public void ClearBools()
    {
        for (int i = 0; i < 20; i++) 
        {
            boolList[i] = false;     
        }
    }
    void Update()
    {
        if(Reloj.GetComponent<ClockManager>().Time.text == "05:00 AM" && yacrecio == false)
        {
            Dia += 1;
            yacrecio = true;
            CheckDayGrow();
            if (yaeligioCh == true)
            {
                yaeligio = false;
                yaeligioCh = false;
                _alreadyRe = false;
            }

            if(FaseFinal.activeInHierarchy)
            {
                DiaM += 1;
                if(DiaM == ExpectedInt)
                {
                    FaseFinal.layer = 7;
                }
            }
        }

        if (Reloj.GetComponent<ClockManager>().Time.text == "06:00 AM" && yacrecio == true)
        {
            yacrecio = false;
        }

        if (FaseFinal.activeInHierarchy && yaeligio == false) 
        {
            PonerFruto();
        }
    }

    public void PonerFruto()
    {
        if (yaeligio == false && ReGrow != ReGrowTimes) {
            
            RandInt = Random.Range(10,15);
            
            while(RandInt > 0)
            {
                var rand = Random.Range(1,20);
                if (rand == 1 && boolList[0] == false) 
                {
                    SelectedSpawn = Spawnpoint1;
                    var frut1 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut1 = frut1.transform.GetChild(2).gameObject;
                    boolList[0] = true;
                } else if (rand == 1 && boolList[0] == true){
                    rand = 2;
                }
                if (rand == 2 && boolList[1] == false) 
                {
                    SelectedSpawn = Spawnpoint2;
                    var frut2 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);       
                    Frut2 = frut2.transform.GetChild(2).gameObject;
                    boolList[1] = true;
                } else if (rand == 2 && boolList[1] == true){
                    rand = 3;
                }
                if (rand == 3 && boolList[2] == false) 
                {
                    SelectedSpawn = Spawnpoint3;
                    var frut3 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut3 = frut3.transform.GetChild(2).gameObject;
                    boolList[2] = true;
                } else if (rand == 3 && boolList[2] == true){
                    rand = 4;
                }
                if (rand == 4 && boolList[3] == false) 
                {
                    SelectedSpawn = Spawnpoint4;
                    var frut4 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut4 = frut4.transform.GetChild(2).gameObject;
                    boolList[3] = true;
                } else if (rand == 4 && boolList[3] == true){
                    rand = 5;
                }
                if (rand == 5 && boolList[4] == false) 
                {
                    SelectedSpawn = Spawnpoint5;
                    var frut5 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut5 = frut5.transform.GetChild(2).gameObject;
                    boolList[4] = true;
                } else if (rand == 5 && boolList[4] == true){
                    rand = 6;
                }
                if (rand == 6 && boolList[5] == false) 
                {
                    SelectedSpawn = Spawnpoint6;
                    var frut6 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut6 = frut6.transform.GetChild(2).gameObject;
                    boolList[5] = true;
                } else if (rand == 6 && boolList[5] == true){
                    rand = 7;
                }
                if (rand == 7 && boolList[6] == false) 
                {
                    SelectedSpawn = Spawnpoint7;
                    var frut7 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut7 = frut7.transform.GetChild(2).gameObject;
                    boolList[6] = true;
                } else if (rand == 7 && boolList[6] == true){
                    rand = 8;
                }
                if (rand == 8 && boolList[7] == false) 
                {
                    SelectedSpawn = Spawnpoint8;
                    var frut8 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut8 = frut8.transform.GetChild(2).gameObject;
                    boolList[7] = true;
                } else if (rand == 8 && boolList[7] == true){
                    rand = 9;
                }
                if (rand == 9 && boolList[8] == false) 
                {
                    SelectedSpawn = Spawnpoint9;
                    var frut9 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut9 = frut9.transform.GetChild(2).gameObject;
                    boolList[8] = true;
                } else if (rand == 9 && boolList[8] == true){
                    rand = 10;
                }
                if (rand == 10 && boolList[9] == false) 
                {
                    SelectedSpawn = Spawnpoint10;
                    var frut10 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);       
                    Frut10 = frut10.transform.GetChild(2).gameObject;
                    boolList[9] = true;
                } else if (rand == 10 && boolList[9] == true){
                    rand = 11;
                }
                if (rand == 11 && boolList[10] == false) 
                {
                    SelectedSpawn = Spawnpoint11;
                    var frut11 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut11 = frut11.transform.GetChild(2).gameObject;
                    boolList[10] = true;
                } else if (rand == 11 && boolList[10] == true){
                    rand = 12;
                }
                if (rand == 12 && boolList[11] == false) 
                {
                    SelectedSpawn = Spawnpoint12;
                    var frut12 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);       
                    Frut12 = frut12.transform.GetChild(2).gameObject;
                    boolList[11] = true;
                } else if (rand == 12 && boolList[11] == true){
                    rand = 13;
                }
                if (rand == 13 && boolList[12] == false) 
                {
                    SelectedSpawn = Spawnpoint13;
                    var frut13 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut13 = frut13.transform.GetChild(2).gameObject;
                    boolList[12] = true;
                } else if (rand == 13 && boolList[12] == true){
                    rand = 14;
                }
                if (rand == 14 && boolList[13] == false) 
                {
                    SelectedSpawn = Spawnpoint14;
                    var frut14 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut14 = frut14.transform.GetChild(2).gameObject;
                    boolList[13] = true;
                } else if (rand == 14 && boolList[13] == true){
                    rand = 15;
                }
                if (rand == 15 && boolList[14] == false) 
                {
                    SelectedSpawn = Spawnpoint15;
                    var frut15 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut15 = frut15.transform.GetChild(2).gameObject;
                    boolList[14] = true;
                } else if (rand == 15 && boolList[14] == true){
                    rand = 16;
                }
                if (rand == 16 && boolList[15] == false) 
                {
                    SelectedSpawn = Spawnpoint16;
                    var frut16 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut16 = frut16.transform.GetChild(2).gameObject;
                    boolList[15] = true;
                } else if (rand == 16 && boolList[15] == true){
                    rand = 17;
                }
                if (rand == 17 && boolList[16] == false) 
                {
                    SelectedSpawn = Spawnpoint17;
                    var frut17 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut17 = frut17.transform.GetChild(2).gameObject;
                    boolList[16] = true;
                } else if (rand == 17 && boolList[16] == true){
                    rand = 18;
                }
                if (rand == 18 && boolList[17] == false) 
                {
                    SelectedSpawn = Spawnpoint18;
                    var frut18 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut18 = frut18.transform.GetChild(2).gameObject;
                    boolList[17] = true;
                } else if (rand == 18 && boolList[17] == true){
                    rand = 19;
                }
                if (rand == 19 && boolList[18] == false) 
                {
                    SelectedSpawn = Spawnpoint19;
                    var frut19 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut19 = frut19.transform.GetChild(2).gameObject;
                    boolList[18] = true;
                } else if (rand == 19 && boolList[18] == true){
                    rand = 20;
                }
                if (rand == 20 && boolList[19] == false) 
                {
                    SelectedSpawn = Spawnpoint20;
                    var frut20 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);       
                    Frut20 = frut20.transform.GetChild(2).gameObject;
                    boolList[19] = true;
                } else if (rand == 20 && boolList[19] == true){
                    rand = 1;
                }
                RandInt--;
            }
            DiaM = 1;
            yaeligio = true;
        }
    }

    public void CheckDayGrow()
    {
        #region IfSpagetti
        if(Dia == Int1 && yacrecio == true)
        {
            Fase1.SetActive(false);
            Fase2.SetActive(true);
        }

        if (Fase3 == null)
        {
            return;
        }
        
        if(Dia == Int2 && yacrecio == true)
        {
            Destroy(Fase1);
            Fase2.SetActive(false);
            Fase3.SetActive(true);
        }
        
        if (Fase4 == null)
        {
            return;
        }

        if(Dia == Int3 && yacrecio == true)
        {
            Destroy(Fase2);
            Fase3.SetActive(false);
            Fase4.SetActive(true);
        }

        if (Fase5 == null)
        {
            return;
        } //TODO: Preguntarle a Sasha cómo se dice frutilla en Swahili.
        
        if(Dia == Int4 && yacrecio == true)
        {
            Destroy(Fase3);
            Fase4.SetActive(false);
            Fase5.SetActive(false);
        }

        if (Fase6 == null)
        {
            return;
        }

        if(Dia == Int5 && yacrecio == true)
        {
            Destroy(Fase4);
            Fase5.SetActive(false);
            Fase6.SetActive(false);
        }

        if (Fase7 == null)
        {
            return;
        }

        if(Dia == Int6 && yacrecio == true)
        {
            Destroy(Fase5);
            Fase6.SetActive(false);
            Fase7.SetActive(false);
        }

        if (Fase8 == null)
        {
            return;
        }

        if(Dia == Int7 && yacrecio == true)
        {
            Destroy(Fase6);
            Fase7.SetActive(false);
            Fase8.SetActive(false);
        }

        if (Fase9 == null)
        {
            return;
        }

        if(Dia == Int8 && yacrecio == true)
        {
            Destroy(Fase7);
            Fase8.SetActive(false);
            Fase9.SetActive(false);
        }

        if (Fase10 == null)
        {
            return;
        }

        if(Dia == Int9 && yacrecio == true)
        {
            Destroy(Fase8);
            Fase9.SetActive(false);
            Fase10.SetActive(false);
        }
        #endregion
    }

    public IEnumerator BushCedeLaPresidencia()
    {
        Tierra.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(0.5f);
        Destroy(Tierra.transform.parent.gameObject);
    }
}

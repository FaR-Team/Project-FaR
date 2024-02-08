using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils.Systems.DateTime;

public class CrecimientoBush : MonoBehaviour
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
    public GameObject Spawnpoint1, Spawnpoint2, Spawnpoint3, Spawnpoint4, Spawnpoint5, Spawnpoint6, Spawnpoint7, Spawnpoint8;

    public GameObject Tierra = null;

    public GameObject Frut1, Frut2, Frut3, Frut4, Frut5, Frut6, Frut7, Frut8;
    private bool bool1, bool2, bool3, bool4, bool5, bool6, bool7, bool8;

    public int RandInt;
    public int ExpectedInt;
    public int ReGrow;
    public int ReGrowTimes;
    public GameObject Prefab;

    public bool _alreadyRe = false;
    public bool yaeligio = false;
    public bool yaeligioCh = false;

    void Start()
    {
        Fase1.SetActive(true);
        Reloj = GameObject.FindGameObjectWithTag("Reloj");
        Dia = 0;
        Tierra = this.transform.root.GetChild(0).gameObject;
        yacrecio = false;
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
            
            RandInt = Random.Range(3,5);
            
            while(RandInt > 0)
            {
                var rand = Random.Range(1,8);
                if (rand == 1 && bool1 == false) 
                {
                    SelectedSpawn = Spawnpoint1;
                    var frut1 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut1 = frut1.transform.GetChild(2).gameObject;
                    bool1 = true;
                } else if (rand == 1 && bool1 == true){
                    rand = 2;
                }
                if (rand == 2 && bool2 == false) 
                {
                    SelectedSpawn = Spawnpoint2;
                    var frut2 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);       
                    Frut2 = frut2.transform.GetChild(2).gameObject;
                    bool2 = true;
                } else if (rand == 2 && bool2 == true){
                    rand = 3;
                }
                if (rand == 3 && bool3 == false) 
                {
                    SelectedSpawn = Spawnpoint3;
                    var frut3 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut3 = frut3.transform.GetChild(2).gameObject;
                    bool3 = true;
                } else if (rand == 3 && bool3 == true){
                    rand = 4;
                }
                if (rand == 4 && bool4 == false) 
                {
                    SelectedSpawn = Spawnpoint4;
                    var frut4 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut4 = frut4.transform.GetChild(2).gameObject;
                    bool4 = true;
                } else if (rand == 4 && bool4 == true){
                    rand = 5;
                }
                if (rand == 5 && bool5 == false) 
                {
                    SelectedSpawn = Spawnpoint5;
                    var frut5 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut5 = frut5.transform.GetChild(2).gameObject;
                    bool5 = true;
                } else if (rand == 5 && bool5 == true){
                    rand = 6;
                }
                if (rand == 6 && bool6 == false) 
                {
                    SelectedSpawn = Spawnpoint6;
                    var frut6 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut6 = frut6.transform.GetChild(2).gameObject;
                    bool6 = true;
                } else if (rand == 6 && bool6 == true){
                    rand = 7;
                }
                if (rand == 7 && bool7 == false) 
                {
                    SelectedSpawn = Spawnpoint7;
                    var frut7 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut7 = frut7.transform.GetChild(2).gameObject;
                    bool7 = true;
                } else if (rand == 7 && bool7 == true){
                    rand = 8;
                }
                if (rand == 8 && bool8 == false) 
                {
                    SelectedSpawn = Spawnpoint8;
                    var frut8 = Instantiate(Prefab, SelectedSpawn.transform.position, SelectedSpawn.transform.rotation, SelectedSpawn.transform);
                    Frut8 = frut8.transform.GetChild(2).gameObject;
                    bool8 = true;
                } else if (rand == 8 && bool8 == true){
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

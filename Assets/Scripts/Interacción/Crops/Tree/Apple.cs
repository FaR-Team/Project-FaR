using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Apple : MonoBehaviour, IInteractable
{
    public GameObject Crop;
    public GameObject Energia;
    public CrecimientoTree crecimientoTree;
    public Animation anim;

    [SerializeField] private GameObject _prompt; 
    
    public GameObject InteractionPrompt => _prompt;

    private void Awake() {
        _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
    }


    public void Interact(Interactor interactor,  out bool interactSuccessful)
    {
        if (Energia.GetComponent<Energía>().EnergiaActual >= 1){
            InteractOut();
            interactSuccessful = true;
        }else{
            interactSuccessful = true;
            return;
        }
    }

    public void InteractOut()
    {
        if (crecimientoTree._alreadyRe == false)
        {
            crecimientoTree._alreadyRe = true;
            Energia = GameObject.FindWithTag("Energia");
            if (Energia.GetComponent<Energía>().EnergiaActual >= 1){
                if (Energia.GetComponent<Energía>()._ContadorActivo == false)
                {
                    Energia.GetComponent<Animation>().Play("Entrar uwuw");
                    Energia.GetComponent<Energía>()._ContadorActivo = true;
                    Energia.GetComponent<Energía>().timer = 5;
                    Energia.GetComponent<Energía>()._yaAnimo = false;
                }
                else if (Energia.GetComponent<Energía>()._ContadorActivo == true)
                {
                    Energia.GetComponent<Energía>().timer = 5;
                }
                CheckOutline();
                Energia.GetComponent<Energía>().EnergiaActual -= 1;
                Energia.GetComponent<Energía>().UpdateEnergy();
            }
            StartCoroutine(Wait());
            //anim.Play();
        }
    }

    public void CheckOutline()
    {
        #region IfSpagetti
        if (crecimientoTree.Frut1 != null)
        {
            crecimientoTree.Frut1.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut2 != null)
        {
            crecimientoTree.Frut2.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut3 != null)
        {
            crecimientoTree.Frut3.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut4 != null)
        {
            crecimientoTree.Frut4.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut5 != null)
        {
            crecimientoTree.Frut5.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut6 != null)
        {
            crecimientoTree.Frut6.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut7 != null)
        {
            crecimientoTree.Frut7.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut8 != null)
        {
            crecimientoTree.Frut8.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut9 != null)
        {
            crecimientoTree.Frut9.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut10 != null)
        {
            crecimientoTree.Frut10.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut11 != null)
        {
            crecimientoTree.Frut11.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut12 != null)
        {
            crecimientoTree.Frut12.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut13 != null)
        {
            crecimientoTree.Frut13.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut14 != null)
        {
            crecimientoTree.Frut14.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut15 != null)
        {
            crecimientoTree.Frut15.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut16 != null)
        {
            crecimientoTree.Frut16.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut17 != null)
        {
            crecimientoTree.Frut17.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut18 != null)
        {
            crecimientoTree.Frut18.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut19 != null)
        {
            crecimientoTree.Frut19.gameObject.AddComponent<Outline>();
        }
        if (crecimientoTree.Frut20 != null)
        {
            crecimientoTree.Frut20.gameObject.AddComponent<Outline>();
        }
        #endregion
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        #region IfSpagetti2
        if (crecimientoTree.Frut1 != null)
        {
            crecimientoTree.Frut1.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut1);
        }
        if (crecimientoTree.Frut2 != null)
        {
            crecimientoTree.Frut2.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut2);
        }
        if (crecimientoTree.Frut3 != null)
        {
            crecimientoTree.Frut3.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut3);
        }
        if (crecimientoTree.Frut4 != null)
        {
            crecimientoTree.Frut4.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut4);
        }
        if (crecimientoTree.Frut5 != null)
        {
            crecimientoTree.Frut5.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut5);
        }
        if (crecimientoTree.Frut6 != null)
        {
            crecimientoTree.Frut6.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut6);
        } 
        if (crecimientoTree.Frut7 != null)
        {
            crecimientoTree.Frut7.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut7);
        }
        if (crecimientoTree.Frut8 != null)
        {
            crecimientoTree.Frut8.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut8);
        }
        if (crecimientoTree.Frut9 != null)
        {
            crecimientoTree.Frut9.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut9);
        }
        if (crecimientoTree.Frut10 != null)
        {
            crecimientoTree.Frut10.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut10);
        }
        if (crecimientoTree.Frut11 != null)
        {
            crecimientoTree.Frut11.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut11);
        }
        if (crecimientoTree.Frut12 != null)
        {
            crecimientoTree.Frut12.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut12);
        }
        if (crecimientoTree.Frut13 != null)
        {
            crecimientoTree.Frut13.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut13);
        }
        if (crecimientoTree.Frut14 != null)
        {
            crecimientoTree.Frut14.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut14);
        }
        if (crecimientoTree.Frut15 != null)
        {
            crecimientoTree.Frut15.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut15);
        }
        if (crecimientoTree.Frut16 != null)
        {
            crecimientoTree.Frut16.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut16);
        }
        if (crecimientoTree.Frut17 != null)
        {
            crecimientoTree.Frut17.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut17);
        }
        if (crecimientoTree.Frut18 != null)
        {
            crecimientoTree.Frut18.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut18);
        }
        if (crecimientoTree.Frut19 != null)
        {
            crecimientoTree.Frut19.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut19);
        }
        if (crecimientoTree.Frut20 != null)
        {
            crecimientoTree.Frut20.gameObject.GetComponent<CropExplodeTree>().Chau(crecimientoTree.Frut20);
        }
        #endregion
            crecimientoTree.ReGrow++;
            crecimientoTree.DiaM = 0;
            crecimientoTree.FaseFinal.layer = 0;
            if (crecimientoTree.ReGrow == crecimientoTree.ReGrowTimes)
            {
                StartCoroutine(crecimientoTree.BushCedeLaPresidencia());
            }
            crecimientoTree.yaeligioCh = true;
            crecimientoTree._alreadyRe = true;
            crecimientoTree.ClearBools();
    }

    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción con Manzana");
    }
}

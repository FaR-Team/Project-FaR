using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Strawberry : MonoBehaviour, IInteractable
{
    public GameObject Crop;
    public GameObject Energia;
    public CrecimientoBush crecimientoBush;
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
        if (crecimientoBush._alreadyRe == false)
        {
            crecimientoBush._alreadyRe = true;
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
        if (crecimientoBush.Frut1 != null)
        {
            crecimientoBush.Frut1.gameObject.AddComponent<Outline>();
        }
        if (crecimientoBush.Frut2 != null)
        {
            crecimientoBush.Frut2.gameObject.AddComponent<Outline>();
        }
        if (crecimientoBush.Frut3 != null)
        {
            crecimientoBush.Frut3.gameObject.AddComponent<Outline>();
        }
        if (crecimientoBush.Frut4 != null)
        {
            crecimientoBush.Frut4.gameObject.AddComponent<Outline>();
        }
        if (crecimientoBush.Frut5 != null)
        {
            crecimientoBush.Frut5.gameObject.AddComponent<Outline>();
        }
        if (crecimientoBush.Frut6 != null)
        {
            crecimientoBush.Frut6.gameObject.AddComponent<Outline>();
        }
        if (crecimientoBush.Frut7 != null)
        {
            crecimientoBush.Frut7.gameObject.AddComponent<Outline>();
        }
        if (crecimientoBush.Frut8 != null)
        {
            crecimientoBush.Frut8.gameObject.AddComponent<Outline>();
        }
        #endregion
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        #region IfSpagetti2
        if (crecimientoBush.Frut1 != null)
        {
            crecimientoBush.Frut1.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut1);
        }
        if (crecimientoBush.Frut2 != null)
        {
            crecimientoBush.Frut2.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut2);
        }
        if (crecimientoBush.Frut3 != null)
        {
            crecimientoBush.Frut3.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut3);
        }
        if (crecimientoBush.Frut4 != null)
        {
            crecimientoBush.Frut4.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut4);
        }
        if (crecimientoBush.Frut5 != null)
        {
            crecimientoBush.Frut5.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut5);
        }
        if (crecimientoBush.Frut6 != null)
        {
            crecimientoBush.Frut6.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut6);
        }
        if (crecimientoBush.Frut7 != null)
        {
            crecimientoBush.Frut7.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut7);
        }
        if (crecimientoBush.Frut8 != null)
        {
            crecimientoBush.Frut8.gameObject.GetComponent<CropExplodeBush>().Chau(crecimientoBush.Frut8);
        }
        #endregion
            crecimientoBush.ReGrow++;
            crecimientoBush.DiaM = 0;
            crecimientoBush.FaseFinal.layer = 0;
            if (crecimientoBush.ReGrow == crecimientoBush.ReGrowTimes)
            {
                StartCoroutine(crecimientoBush.BushCedeLaPresidencia());
            }
            crecimientoBush.yaeligioCh = true;
            crecimientoBush._alreadyRe = true;
    }
    

    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción con Frutilla");
    }
}

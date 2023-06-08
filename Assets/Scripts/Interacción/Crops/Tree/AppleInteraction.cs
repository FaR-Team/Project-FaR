using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class AppleInteraction : MonoBehaviour, IInteractable
{
    public GameObject Crop;
    public GameObject EnergiaGO;
    public AppleTreeGrowing appleTree;
    public GameObject appleTreeGO;
    public Animation anim;

    [SerializeField] private GameObject _prompt; 
    
    public GameObject InteractionPrompt => _prompt;

    private void Awake() {
        _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
        EnergiaGO = GameObject.FindWithTag("Energia");
        appleTree = GetComponent<AppleTreeGrowing>();
    }

    private Energy EnergiaComponent()
    {
        return EnergiaGO.GetComponent<Energy>();
    }
    public void Interact(Interactor interactor,  out bool interactSuccessful)
    {
        if (EnergiaComponent().EnergiaActual >= 1){
            InteractOut();
            interactSuccessful = true;
        }
        else{
            interactSuccessful = true;
            return;
        }
    }

    public void InteractOut()
    {
        if (appleTree._alreadyRe != false)
        {
            return;
            //anim.Play();
        }

        appleTree._alreadyRe = true;

        if (EnergiaComponent().EnergiaActual >= 1)
        {
            if (EnergiaComponent()._ContadorActivo == false)
            {
                EnergiaGO.GetComponent<Animation>().Play("Entrar uwuw");

                EnergiaComponent()._ContadorActivo = true;
                EnergiaComponent().timer = 5;
                EnergiaComponent()._yaAnimo = false;
            }
            else if (EnergiaComponent()._ContadorActivo == true)
            {
                EnergiaComponent().timer = 5;
            }
            AddOutline();
            EnergiaComponent().EnergiaActual -= 1;
            EnergiaComponent().UpdateEnergy();
        }
        StartCoroutine(Wait());
    }

    public void AddOutline()
    {
        foreach(GameObject fruit in appleTree.fruits)
        {
            fruit.AddComponent<Outline>();
        }
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        
        foreach (GameObject fruit in appleTree.fruits)
        {
            fruit.GetComponent<CropExplodeTree>().Chau(fruit);
        }
            appleTree.ReGrow++;
            appleTree.DiaM = 0;
            appleTree.gameObject.layer = 0;
            if (appleTree.ReGrow == appleTree.ReGrowTimes)
            {
                StartCoroutine(appleTree.BushCedeLaPresidencia());
            }
            appleTree.yaeligioCh = true;
            appleTree._alreadyRe = true;
    }

    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción con Manzana");
    }
}

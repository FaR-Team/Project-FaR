using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Carrot : MonoBehaviour, IInteractable
{
    public GameObject Crop;
    public GameObject CropOut;
    public GameObject CropParent;
    public GameObject Energia;
    public AnimationClip anim;

    public GameObject Tierra = null;
    public Transform tierraAnim;

    private bool JustInter;

    [SerializeField] private GameObject _prompt; 
    
    public GameObject InteractionPrompt => _prompt;


    private void Start() {
        _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
        Energia = GameObject.FindWithTag("Energia");
        Tierra = transform.root.GetChild(0).gameObject;
        //InteractionPromptUI._uiPanel =  InteractionPromptUI._Panel;
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
        Tierra = transform.root.GetChild(0).gameObject;
        if (JustInter == false)
        {
            JustInter = true;
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

                for (int i = 0; i < Tierra.gameObject.transform.childCount; i++)
                {
                    if(Tierra.gameObject.transform.GetChild(i).gameObject.activeSelf == true && Tierra.gameObject.transform.GetChild(i).gameObject.GetComponent<Animation>() != null)
                    {
                        tierraAnim = Tierra.gameObject.transform.GetChild(i);
                    }
                }

                CropOut.gameObject.AddComponent<Outline>();
                if (CropParent.GetComponent<Animation>() != null)
                {
                    CropParent.GetComponent<Animation>().clip = anim;
                    CropParent.GetComponent<Animation>().Play();
                }

                if (tierraAnim != null)
                {
                    tierraAnim.gameObject.GetComponent<Animation>().Play();
                }


                foreach(Transform t in CropParent.transform)
                {
                    t.gameObject.layer = 0;
                }
                CropParent.layer = 0;
                Energia.GetComponent<Energía>().EnergiaActual -= 1;
                Energia.GetComponent<Energía>().UpdateEnergy();
                StartCoroutine(Wait());
            }
        }
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        Crop.gameObject.GetComponent<CropExplode>().Chau();
    }
    

    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción con Zanahoria");
    }
}

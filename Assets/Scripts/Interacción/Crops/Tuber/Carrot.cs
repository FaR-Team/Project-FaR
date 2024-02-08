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
    public bool hasInteracted = false;

    [SerializeField] private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;


    private void Start()
    {
        _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
        Energia = GameObject.FindWithTag("Energia");
        Tierra = transform.root.gameObject;
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (Energia.GetComponent<Energía>().EnergiaActual >= 1)
        {
            Tierra.GetComponent<Dirt>().CreateBox();
            InteractOut();
            interactSuccessful = true;
        }
        else
        {
            interactSuccessful = true;
            return;
        }
    }

    public void InteractOut()
    {
        Tierra = transform.root.gameObject;
        JustInteracted();
    }

    private void JustInteracted()
    {
        Energía energia = Energia.GetComponent<Energía>(); 

        if (JustInter || energia.EnergiaActual < 1) return;


        JustInter = true;

        if (energia._ContadorActivo) energia.timer = 5;

        else
        {
            Energia.GetComponent<Animation>().Play("Entrar uwuw");
            
            energia._ContadorActivo = true;
            energia.timer = 5;
            energia._yaAnimo = false;
        }

        for (int i = 0; i < Tierra.gameObject.transform.childCount; i++)
        {
            if (Tierra.gameObject.transform.GetChild(i).gameObject.activeSelf &&
            Tierra.gameObject.transform.GetChild(i).gameObject.GetComponent<Animation>() != null)
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


        foreach (Transform transform in CropParent.transform)
        {
            transform.gameObject.layer = 0;
        }
        CropParent.layer = 0;
        Energia.GetComponent<Energía>().EnergiaActual -= 1;
        Energia.GetComponent<Energía>().UpdateEnergy();
        StartCoroutine(Wait());
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        Crop.gameObject.GetComponent<CropExplode>().Chau();
    }


    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción con Zanahoria.");
    }
}

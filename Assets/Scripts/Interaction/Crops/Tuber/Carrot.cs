using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils;

public class Carrot : MonoBehaviour, IInteractable
{
    public AnimationClip anim; //CarrotGo.

    [SerializeField] GameObject Dirt;
    Transform tierraAnim;

    private bool JustInter;
    public bool hasInteracted = false;

    [SerializeField] private GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    private void Start()
    {
       // _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
        Dirt = this.transform.parent.gameObject;
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (Energy.RemainingEnergy >= 1)
        {
            Dirt.GetComponent<DirtAreaHarvest>().CreateAreaForHarvest();
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
        JustInteracted();
    }

    private void JustInteracted()
    {

        if (JustInter || Energy.RemainingEnergy < 1) return;


        JustInter = true;

        if (Energy._ContadorActivo) 
        {
            Energy.timer = 5;
        }
        else
        {
            Energy._animationComp.Play("Entrar uwuw");
            Energy._ContadorActivo = true;
            Energy.timer = 5;
            Energy._yaAnimo = false;
        }

        for (int i = 0; i < Dirt.transform.childCount; i++)
        {
            if (Dirt.transform.GetChild(i).gameObject.activeSelf &&
            Dirt.transform.GetChild(i).gameObject.GetComponent<Animation>() != null)
            {
                tierraAnim = Dirt.transform.GetChild(i);
            }
        }
        gameObject.AddComponent<Outline>();
        if (GetComponent<Animation>() != null)
        {
            GetComponent<Animation>().clip = anim;
            GetComponent<Animation>().Play();
        }

        if (tierraAnim != null)
        {
            tierraAnim.GetComponent<Animation>().Play();
        }

        gameObject.layer = 0;

        Energy.RemainingEnergy -= 1;
        Energy.UpdateEnergy();
        StartCoroutine(Wait());
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<FallingFruit>().FallTuber();
        GetComponent<CropExplode>().Chau();
    }


    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción con Zanahoria.");
    }
}


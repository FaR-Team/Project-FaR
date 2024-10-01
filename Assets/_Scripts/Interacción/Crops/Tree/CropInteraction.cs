using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils;

public class CropInteraction : MonoBehaviour, IInteractable
{
    public GameObject Crop;
    private GameObject dirt;
    public Animation anim;
    public GameObject _prompt;

    public bool already;
    public bool isTree;

    public GameObject InteractionPrompt => _prompt;

    public virtual void Awake()
    {
        _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
        if (isTree) return;
        dirt = GetComponentInParent<Dirt>().gameObject;
    }

    public virtual List<GameObject> Fruits()
    {
        return null;
    }

    public virtual void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (Energy.RemainingEnergy >= 1)
        {
            if (!isTree)
            {
                dirt.GetComponent<DirtAreaHarvest>().CreateAreaForHarvest();
            }
            InteractOut();
            interactSuccessful = true;
        }
        else
        {
            interactSuccessful = true;
            return;
        }
    }

    public virtual void InteractOut()
    {
        if (already) return;

        already = true;

        if (Energy.instance.TryUseAndAnimateEnergy(1, 5f))
        {
            Harvest(); //TODO: todas deberían de usar harvest, no wait.
            // TODO: Ver como mejorar y que el Harvest sea funcion del Growing, o llame a una funcion de ahi para reiniciar variables
            StartCoroutine(Wait());
        }
    }

    public void AddOutline()
    {
        foreach (GameObject fruit in Fruits())
        {
            fruit.AddComponent<Outline>();
        }
    }

    public virtual void Harvest(){ }

    public virtual IEnumerator Wait()
    {
        yield return null;
    }

    public virtual void DoEnumeratorIfMaxRegrows()
    {

    }

    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción.");
    }
}

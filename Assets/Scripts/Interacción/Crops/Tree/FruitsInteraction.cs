using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class FruitsInteraction : MonoBehaviour, IInteractable
{
    public GameObject Crop;
    public GameObject EnergiaGO;

    // public AppleTreeGrowing appleTree;

    public GameObject appleTreeGO;
    public Animation anim;
    public GameObject _prompt;

    public GameObject InteractionPrompt => _prompt;

    public bool already;
    public int ReGrowMaxTime;
    public int ReGrow;
    public int DiaM;
    public bool yaEligioCh;

    public virtual void Awake()
    {
        _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
        EnergiaGO = GameObject.FindWithTag("Energia");
    }

    public virtual List<GameObject> Fruits()
    {
        return null;
    }

    public virtual GameObject GetGameObject()
    {
        return null;
    }
    public virtual IEnumerator Enumerator()
    {
        yield return null;
    }
    public Energy EnergiaComponent()
    {
        return EnergiaGO.GetComponent<Energy>();
    }
    public virtual void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (EnergiaComponent().EnergiaActual >= 1)
        {
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
        if (already != false) return;

        already = true;

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
            AddOutline(Fruits());
            EnergiaComponent().EnergiaActual -= 1;
            EnergiaComponent().UpdateEnergy();
        }
        StartCoroutine(Wait(Fruits()));
    }

    public void AddOutline(List<GameObject> listOfFruits)
    {
        foreach (GameObject fruit in listOfFruits)
        {
            fruit.AddComponent<Outline>();
        }
    }

    public IEnumerator Wait(List<GameObject> listOfFruits)
    {
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject fruit in Fruits())
        {
            fruit.GetComponent<CropExplodeTree>().Chau(fruit);
        }

        ReGrow++;
        DiaM = 0;
        GetGameObject().layer = 0;
        if (ReGrow == ReGrowMaxTime)
        {
            StartCoroutine(Enumerator());
        }
        yaEligioCh = true;
        already = true;
    }



    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción.");
    }
}

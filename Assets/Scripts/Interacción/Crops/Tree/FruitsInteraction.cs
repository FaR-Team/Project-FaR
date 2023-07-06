using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class FruitsInteraction : MonoBehaviour, IInteractable
{
    public GameObject Crop;
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

    public virtual void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (Energy.RemainingEnergy >= 1)
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

        if (Energy.RemainingEnergy >= 1)
        {
            if (Energy._ContadorActivo == false)
            {
                Energy._animationComp.Play("Entrar uwuw");

                Energy._ContadorActivo = true;
                Energy.timer = 5;
                Energy._yaAnimo = false;
            }// CAMBIAR LO DE LA ENERGÍA
            else if (Energy._ContadorActivo == true)
            {
                Energy.timer = 5;
            }
            AddOutline();
            Energy.RemainingEnergy -= 1;
            Energy.UpdateEnergy();
        }
        StartCoroutine(Wait());
    }

    public void AddOutline()
    {
        foreach (GameObject fruit in Fruits())
        {
            fruit.AddComponent<Outline>();
        }
    }

    public virtual IEnumerator Wait()
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

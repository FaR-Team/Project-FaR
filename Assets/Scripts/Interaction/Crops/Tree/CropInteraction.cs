using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FaRUtils;

public class CropInteraction : MonoBehaviour, IInteractable
{
    public GameObject Crop;
    public Animation anim;
    public GameObject _prompt;

    public bool already;
    public GameObject InteractionPrompt => _prompt;

    public virtual void Awake()
    {
        _prompt = GameObject.FindGameObjectWithTag("CropInteraction");
    }

    public virtual List<GameObject> Fruits()
    {
        return null;
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
        if (already) return;

        already = true;

        if (Energy.instance.TryUseAndAnimateEnergy(1, 5f))
        {
            AddOutline();
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

    public virtual IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (GameObject fruit in Fruits())
        {
            GetComponent<CropExplodeBush>().Chau(fruit);
        }
        gameObject.layer = 0;
        DoEnumeratorIfMaxRegrows();
        already = false;
    }

    public virtual void DoEnumeratorIfMaxRegrows()
    {

    }

    public void EndInteraction()
    {
        Debug.Log("Terminando Interacción.");
    }
}

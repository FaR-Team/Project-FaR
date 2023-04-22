using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtConnector : MonoBehaviour
{
    [Header("Puntos Cardinales")]
    public GameObject Sur;
    public GameObject Oeste;
    public GameObject Norte;
    public GameObject Este;

    [Header("Tierras")]
    public GameObject OShape;
    public GameObject PlusShape;
    public GameObject IShapeNorte;
    public GameObject IShapeSur;
    public GameObject IShapeEste;
    public GameObject IShapeOeste;
    public GameObject PitoShapeNorteSur;
    public GameObject PitoShapeEsteOeste;
    public GameObject TShapeSurEsteOeste;
    public GameObject TShapeNorteOesteSur;
    public GameObject TShapeNorteEsteSur;
    public GameObject TShapeNorteEsteOeste;
    public GameObject LShapeSurOeste;
    public GameObject LShapeSurEste;
    public GameObject LShapeNorteOeste;
    public GameObject LShapeNorteEste;

    [Header("Booleanos")]
    public bool isSur = false;
    public bool isOeste = false;
    public bool isNorte = false;
    public bool isEste = false;

    
    void Update()
    {
        //
    }

    public void UpdateDirtPrefab()
    {
        if (isSur && isOeste && isNorte && isEste)
        {
            PlusShape.SetActive(true);
            OShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false); 
        }

        if (!isSur && isOeste && !isNorte && !isEste) 
        {
            IShapeOeste.SetActive(true);
            PlusShape.SetActive(false);
            OShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }
        else if (!isSur && !isOeste && isNorte && !isEste) 
        {
            IShapeNorte.SetActive(true);
            PlusShape.SetActive(false);
            OShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }
        else if (!isSur && !isOeste && !isNorte && isEste) 
        {
            IShapeEste.SetActive(true);
            PlusShape.SetActive(false);
            OShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }
        else if (isSur && !isOeste && !isNorte && !isEste) 
        {
            IShapeSur.SetActive(true);
            PlusShape.SetActive(false);
            OShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }

        if (isSur && isNorte && !isOeste && !isEste)
        {
            PitoShapeNorteSur.SetActive(true);
            PlusShape.SetActive(false);
            OShape.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }
        else if (!isSur && !isNorte && isOeste && isEste)
        {
            PitoShapeEsteOeste.SetActive(true);
            PlusShape.SetActive(false);
            OShape.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }

        if (isSur && isEste && !isOeste && !isNorte)
        {
            LShapeSurEste.SetActive(true);
            OShape.SetActive(false);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
            
        }
        else if (!isSur && isEste && !isOeste && isNorte)
        {
            LShapeNorteEste.SetActive(true);
            OShape.SetActive(false);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }
        else if (!isSur && !isEste && isOeste && isNorte)
        {
            LShapeNorteOeste.SetActive(true);
            OShape.SetActive(false);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }
        else if (isSur && !isEste && isOeste && !isNorte)
        {
            LShapeSurOeste.SetActive(true);
            OShape.SetActive(false);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false);
        }

        if (isSur && isEste && isOeste && !isNorte)
        {
            TShapeSurEsteOeste.SetActive(true);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
        }
        else if (isSur && !isEste && isOeste && isNorte)
        {
            TShapeNorteOesteSur.SetActive(true);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
        }
        else if (isSur && isEste && !isOeste && isNorte)
        {
            TShapeNorteEsteSur.SetActive(true);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
        }
        else if (!isSur && isEste && isOeste && isNorte)
        {
            TShapeNorteEsteOeste.SetActive(true);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
        }

        if (!isSur && !isEste && !isOeste && !isNorte)
        {
            OShape.SetActive(true);
            PlusShape.SetActive(false);
            PitoShapeEsteOeste.SetActive(false);
            PitoShapeNorteSur.SetActive(false);
            LShapeSurOeste.SetActive(false);
            LShapeSurEste.SetActive(false);
            LShapeNorteOeste.SetActive(false);
            LShapeNorteEste.SetActive(false);
            IShapeEste.SetActive(false);
            IShapeSur.SetActive(false);
            IShapeOeste.SetActive(false);
            IShapeNorte.SetActive(false);
            TShapeSurEsteOeste.SetActive(false);
            TShapeNorteOesteSur.SetActive(false);
            TShapeNorteEsteSur.SetActive(false);
            TShapeNorteEsteOeste.SetActive(false); 
        }
    }
}

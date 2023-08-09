using System;
using UnityEngine;


public class DirtConnectorNew : MonoBehaviour
{

    [Header("Tierra")]
    public GameObject DirtModel;

    void OnEnable()
    {
        SkinnedMeshRenderer gameObjectMeshRenderer = DirtModel.GetComponentInChildren<SkinnedMeshRenderer>();
        
        gameObjectMeshRenderer.material = dirtInformation.material_O_Shape;
        gameObjectMeshRenderer.sharedMesh = dirtInformation.mesh_O_Shape;

        isSur = false;
        isOeste = false;
        isNorte = false;
        isEste = false;
    }

    [Serializable]
    public class DirtInformation
    {
        public Mesh mesh_O_Shape;
        public Mesh mesh_I_Shape;
        public Mesh mesh_P_Shape;
        public Mesh mesh_L_Shape;
        public Mesh mesh_T_Shape;
        public Mesh mesh_F_Shape;

        public Material material_O_Shape;
        public Material material_I_Shape;
        public Material material_P_Shape;
        public Material material_L_Shape;
        public Material material_T_Shape;
        public Material material_F_Shape;

        /*public AnimationClip animO;
        public AnimationClip animI;
        public AnimationClip animP;
        public AnimationClip animL;
        public AnimationClip animT;
        public AnimationClip animF; */
    }

    [SerializeField]
    DirtInformation dirtInformation; //Almaceno las variables en una clase para poder nombrar cada material y textura.
                                     //y no tener en el inspector cada mesh y material solo sin poder comprimirlos a la vista

    [Header("Booleanos")]
    public bool isSur = false;
    public bool isOeste = false;
    public bool isNorte = false;
    public bool isEste = false;

    Quaternion Rotation(int number)
    {
        if (number is 0 or 8 or 10 or 14 or 15)
        {
            return Quaternion.Euler(0, 0, 0);
        }
        if (number is 1 or 5 or 9 or 13)
        {
            return Quaternion.Euler(0, -90, 0);
        }
        if (number is 2 or 3 or 11)
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }
        if (number is 4 or 6 or 7)
        {
            return Quaternion.Euler(0f, 90f, 0f);
        }
        else return Quaternion.Euler(0f, 0f, 0f);

    }

    void SetMaterialAndMesh(int number, SkinnedMeshRenderer gameObjectMeshRenderer)
    {
        if (number is 0)
        {
            gameObjectMeshRenderer.material = dirtInformation.material_O_Shape;
            gameObjectMeshRenderer.sharedMesh = dirtInformation.mesh_O_Shape;

        }
        if (number is 1 or 2 or 4 or 8)
        {
            gameObjectMeshRenderer.material = dirtInformation.material_I_Shape;
            gameObjectMeshRenderer.sharedMesh = dirtInformation.mesh_I_Shape;
        }
        if (number is 3 or 6 or 9 or 12)
        {
            gameObjectMeshRenderer.material = dirtInformation.material_L_Shape;
            gameObjectMeshRenderer.sharedMesh = dirtInformation.mesh_L_Shape;
        }
        if (number is 5 or 10)
        {
            gameObjectMeshRenderer.material = dirtInformation.material_P_Shape;
            gameObjectMeshRenderer.sharedMesh = dirtInformation.mesh_P_Shape;
        }
        if (number is 7 or 11 or 13 or 14)
        {
            gameObjectMeshRenderer.material = dirtInformation.material_T_Shape;
            gameObjectMeshRenderer.sharedMesh = dirtInformation.mesh_T_Shape;
        }
        if (number is 15)
        {
            gameObjectMeshRenderer.material = dirtInformation.material_F_Shape;
            gameObjectMeshRenderer.sharedMesh = dirtInformation.mesh_F_Shape;
        }
    }

    public void SetSelectedBool(GameObject gameObject, int numberOfPosition)
    {
        //Animation gameObjectAnimation= gameObject.GetComponent<Animation>();

        Transform gameObjecTransform = gameObject.GetComponent<Transform>();

        SkinnedMeshRenderer gameObjectMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        SetMaterialAndMesh(numberOfPosition, gameObjectMeshRenderer);

        gameObjecTransform.rotation = Rotation(numberOfPosition);

        if (GetComponentInParent<Dirt>()._isWet)
        {
            gameObjectMeshRenderer.material.color = Dirt.wetDirtColor;
        }
    }

    public void UpdateDirtPrefab()
    {
        int value = Convert.ToInt32(isSur) << 3 | Convert.ToInt32(isOeste) << 2 | Convert.ToInt32(isNorte) << 1 | Convert.ToInt32(isEste);
        SetSelectedBool(DirtModel, value);
    }
}

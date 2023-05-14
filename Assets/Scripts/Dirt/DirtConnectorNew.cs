using System;
using UnityEngine;


public class DirtConnectorNew : MonoBehaviour
{

    [Header("Tierra")]
    public GameObject DirtModel;

    [Serializable]
    public class DirtInformation
    {
        public Mesh meshOShape;
        public Mesh meshIShape;
        public Mesh meshPShape;
        public Mesh meshLShape;
        public Mesh meshTShape;
        public Mesh meshFShape;

        public Material materialOShape;
        public Material materialIShape;
        public Material materialPShape;
        public Material materialLShape;
        public Material materialTShape;
        public Material materialFShape;

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

    public void SetSelectedBool(GameObject gameObject, int numberOfPosition)
    {
        Transform gameObjecTransform = gameObject.GetComponent<Transform>();
        //Animation gameObjectAnimation= gameObject.GetComponent<Animation>();
        SkinnedMeshRenderer gameObjectMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        switch (numberOfPosition)
        {
            case 0:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animO;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshOShape;
                gameObjectMeshRenderer.material= dirtInformation.materialOShape;
                break;
            case 1:
                gameObjecTransform.rotation = Quaternion.Euler(0f, -90f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animI;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshIShape;
                gameObjectMeshRenderer.material = dirtInformation.materialIShape;
                break;            
            case 2:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animI;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshIShape;
                gameObjectMeshRenderer.material = dirtInformation.materialIShape;
                break;            
            case 3:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animL;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshLShape;
                gameObjectMeshRenderer.material = dirtInformation.materialLShape;

                break;            
            case 4:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 90f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animI;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshIShape;
                gameObjectMeshRenderer.material = dirtInformation.materialIShape;

                break;            
            case 5:
                gameObjecTransform.rotation = Quaternion.Euler(0f, -90f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animP;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshPShape;
                gameObjectMeshRenderer.material = dirtInformation.materialPShape;

                break;            
            case 6:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 90f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animL;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshLShape;
                gameObjectMeshRenderer.material = dirtInformation.materialLShape;

                break;           
            case 7:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 90f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animT;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshTShape;
                gameObjectMeshRenderer.material = dirtInformation.materialTShape;

                break;          
            case 8:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animI;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshIShape;
                gameObjectMeshRenderer.material = dirtInformation.materialIShape;

                break;           
            case 9:
                gameObjecTransform.rotation = Quaternion.Euler(0f, -90f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animL;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshLShape;
                gameObjectMeshRenderer.material = dirtInformation.materialLShape;

                break;          
            case 10:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animP;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshPShape;
                gameObjectMeshRenderer.material = dirtInformation.materialPShape;

                break;          
            case 11:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 180f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animT;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshTShape;
                gameObjectMeshRenderer.material = dirtInformation.materialTShape;

                break;         
            case 12:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animL;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshLShape;
                gameObjectMeshRenderer.material = dirtInformation.materialLShape;

                break;          
            case 13:
                gameObjecTransform.rotation = Quaternion.Euler(0f, -90f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animT;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshTShape;
                gameObjectMeshRenderer.material = dirtInformation.materialTShape;

                break;          
            case 14:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animT;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshTShape;
                gameObjectMeshRenderer.material = dirtInformation.materialTShape;

                break;          
            case 15:
                gameObjecTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
                //gameObjectAnimation.clip = dirtInformation.animF;
                gameObjectMeshRenderer.sharedMesh = dirtInformation.meshFShape;
                gameObjectMeshRenderer.material = dirtInformation.materialFShape;

                break;             
        }
        
    }

    public void UpdateDirtPrefab()
    {
        int value = Convert.ToInt32(isSur) << 3 | Convert.ToInt32(isOeste) << 2 | Convert.ToInt32(isNorte) << 1 | Convert.ToInt32(isEste);
        SetSelectedBool(DirtModel, value);
    }
}

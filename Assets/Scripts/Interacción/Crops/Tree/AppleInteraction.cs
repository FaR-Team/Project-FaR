using System.Collections;
using UnityEngine;
using FaRUtils;
using System.Collections.Generic;
using System.Linq;

public class AppleInteraction : CropInteraction
{
    public TreeGrowing appleTree;
    [SerializeField]
    private Animator treeAnimator;
    
    [SerializeField]
    private GameObject interactionAnimationPrefab;
    [SerializeField]
    private float animationDuration = 1.5f;

    Vector3 playerForward;
    Vector3 spawnPosition;
    GameObject player;

    public override void Awake()
    {
        base.Awake();

        appleTree = GetComponent<TreeGrowing>();
    }
    
    private void PlayInteractionAnimation()
    {
        if (interactionAnimationPrefab != null)
        {  
            GameObject animationObject = Instantiate(interactionAnimationPrefab, spawnPosition, Quaternion.LookRotation(playerForward));
                
            Destroy(animationObject, animationDuration);
        }
    }

    public override IEnumerator Wait()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        { 
            playerForward = player.transform.forward;
            spawnPosition = player.transform.position + playerForward * 2f;
        }
        treeAnimator.SetTrigger("Harvest");
        yield return new WaitForSeconds(0.5f);

        foreach (var fruit in appleTree.fruits)
        {
            fruit.GetComponent<FallingFruit>().FallFruit();
        }

        yield return new WaitForSeconds(0.4f);
        PlayInteractionAnimation();
        
        appleTree.StartReGrowTree();

        already = false;
    }
}
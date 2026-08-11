using UnityEngine;

public struct ItemUseContext
{
    public Interactor Interactor;
    public Dirt DirtToTest;
    public GridGhost GridGhost;
    public Animator HoeAnimator;
    public bool IsHolding;
    public bool IsHoldingCtrl;
    public bool IsLookingAtStore;
}

public struct ItemUseResult
{
    public bool Success;
    public bool ShouldConsume;
    public bool ShouldSellSingle;
    public bool ShouldSellAll;
    public bool PlaySound;
    public float LockMovementDuration;
    public bool TriggerPlowAnim;
}

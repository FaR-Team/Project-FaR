using UnityEngine;

public class AvoidCollisionWPlayer : MonoBehaviour
{
    private GameObject _player;

    private void OnEnable()
    {
        SetCollisionIgnore(true);
    }

    private void OnDestroy()
    {
        SetCollisionIgnore(false);
    }

    private void SetCollisionIgnore(bool ignore)
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        if (_player == null) return;

        Collider[] playerColliders = _player.GetComponentsInChildren<Collider>();
        Collider[] objectColliders = GetComponentsInChildren<Collider>();

        if (playerColliders == null || objectColliders == null) return;

        foreach (var pCol in playerColliders)
        {
            if (pCol == null) continue;
            foreach (var oCol in objectColliders)
            {
                if (oCol == null) continue;
                Physics.IgnoreCollision(pCol, oCol, ignore);
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    public bool active = false;
    public Portal otherPortal;
    public PortalActivator button;

    private bool teleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        if (teleporting) return;
        if (!other.CompareTag("Player")) return;
        if (otherPortal == null) return;

        StartCoroutine(Teleport(other));
    }

    private IEnumerator Teleport(Collider2D player)
    {
        teleporting = true;
        otherPortal.teleporting = true;

        player.transform.position = otherPortal.transform.position;

       
        active = false;
        otherPortal.active = false;

        if (button != null)
            button.DeactivateButton();

        yield return new WaitForSeconds(0.2f);

        teleporting = false;
        otherPortal.teleporting = false;
    }
}
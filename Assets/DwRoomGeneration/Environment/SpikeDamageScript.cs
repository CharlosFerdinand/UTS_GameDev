using UnityEngine;

public class SpikeDamageScript : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 0.5f;
    private bool damageReady = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "hero" && other.gameObject.GetComponent<DwInterfaceDamageAble>() != null)
        {
            if (damageReady)
            {
                damageReady = false;
                Invoke("DamagePrepared", damageCooldown);
                other.gameObject.GetComponent<DwInterfaceDamageAble>().takeDamage(transform.parent.gameObject.GetComponent<SpikeTrapScript>().GetDamage(), this.gameObject);
            }
        }
    }

    private void DamagePrepared()
    {
        damageReady = true;
    }
}

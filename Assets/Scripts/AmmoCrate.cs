using UnityEngine;
using TMPro;

public class AmmoCrate : MonoBehaviour
{
    public int ammo;
    public TMP_Text ammoText;

    private void Update()
    {
        ammoText.text = ammo.ToString();
    }
}
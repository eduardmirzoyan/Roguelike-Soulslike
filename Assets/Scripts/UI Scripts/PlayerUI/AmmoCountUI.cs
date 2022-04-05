using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AmmoCountUI : MonoBehaviour
{
    [SerializeField] private Text ammoCountText;
    [SerializeField] private Inventory ammoInventory;
    [SerializeField] private ItemType ammType;

    [SerializeField] private bool isTrackingAmmo;

    private void Start() {
        ammType = ItemType.Ammo;

        // Set inventory
        ammoInventory = GameManager.instance.GetPlayer().GetComponentInChildren<Inventory>();

        ammoCountText = GetComponentInChildren<Text>();
        ammoCountText.text = "";
    }

    private void Update() {
        if (isTrackingAmmo) {
            var item = ammoInventory.getItemOfType(ammType);
            if (item != null) {
                ammoCountText.text = "x" + ((AmmoItem)item).count;
            }
            else {
                ammoCountText.text = "x0";
            }
            return;
        }
        ammoCountText.text = "";
    }

    public void trackAmmo(bool enable) {
        isTrackingAmmo = enable;
    }
}

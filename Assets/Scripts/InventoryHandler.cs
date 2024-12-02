using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    //inventory params
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject jumpPu;
    [SerializeField] private GameObject dashPu;
    [SerializeField] private GameObject timePu;
    [SerializeField] private GameObject hpPu;
    private PlatformPlayer player;

    private bool inInv;
    public bool hasJump;
    public bool hasDash;
    public bool hasTime;
    public bool hasHp;

    private GameObject lastItemHovered;

    //slowdown params
    private float fixedDeltaTime;

    void Start()
    {
        inInv = false;
        hasJump = false;
        hasDash = false;
        hasTime = false;
        hasHp = false;

        inventory.SetActive(false);
        jumpPu.SetActive(false);
        dashPu.SetActive(false);
        timePu.SetActive(false);
        hpPu.SetActive(false);

        this.fixedDeltaTime = Time.fixedDeltaTime;
        Cursor.lockState = CursorLockMode.Locked;
        player = GetComponent<PlatformPlayer>();
    }

    void Update()
    {
        if(inInv) {
            inventory.SetActive(true);
            jumpPu.SetActive(hasJump);
            dashPu.SetActive(hasDash);
            timePu.SetActive(hasTime);
            hpPu.SetActive(hasHp);

            Time.timeScale = 0.3f;
            Cursor.lockState = CursorLockMode.None;
        }
        
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    private void OnTriggerEnter(Collider col) {
        switch(col.gameObject.tag) {
            case "JumpPu":
                hasJump = true;
                Destroy(col.gameObject);
                break;
            case "DashPu":
                hasDash = true;
                Destroy(col.gameObject);
                break;
            case "TimePu":
                hasTime = true;
                Destroy(col.gameObject);
                break;
            case "HealthPu":
                hasHp = true;
                Destroy(col.gameObject);
                break;
            default:
                break;
        }
    }

    public void Inventory(InputAction.CallbackContext context) {
        if(context.performed) {
            inInv = true;
        }
        if(context.canceled) {
            if(lastItemHovered != null) {
                switch(lastItemHovered.name) {
                    case "JumpPu":
                        hasJump = false;
                        jumpPu.SetActive(false);
                        break;
                    case "DashPu":
                        hasDash = false;
                        dashPu.SetActive(false);
                        break;
                    case "TimePu":
                        hasTime = false;
                        timePu.SetActive(false);
                        break;
                    case "HealthPu":
                        hasHp = false;
                        hpPu.SetActive(false);
                        break;
                    default:
                        break;
                }

                player.EffectHandler(lastItemHovered.name);
            }
            
            inventory.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;

            inInv = false;
            lastItemHovered = null;
        }
    }

    public void HoverInvItem(GameObject invItem) {
        lastItemHovered = invItem;
        lastItemHovered.transform.localScale = new Vector3(1.2f,1.2f,1f);
        Debug.Log("item select: " + lastItemHovered.name);
    }

    public void UnHoverInvItem() {
        lastItemHovered.transform.localScale = new Vector3(1f,1f,1f);
        lastItemHovered = null;
        Debug.Log("item unhovered");
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    [Header("Town Panel UI References")]
    [SerializeField] private Button openBuildPanelBtn;
    [SerializeField] private Image warehouseImage;
    [SerializeField] private TMP_Text centerTurnText;

    [Header("Crafting Slots (โชว์ตอนสร้างเสร็จ)")]
    [SerializeField] private GameObject inputSlotObj;
    

    [Header("Build Selection Panel")]
    [SerializeField] private GameObject buildListPanel;
    [SerializeField] private Button selectWarehouseBtn;
    [SerializeField] private TMP_Text warehousePriceText;

    [Header("Sprites (Empty -> Finish)")]
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite finishSprite;

    [Header("Construction Settings")]
    [SerializeField] private int buildCost = 200;
    [SerializeField] private int turnsToBuild = 2;

    private HashSet<Town> completedTowns = new HashSet<Town>();
    private Dictionary<Town, int> buildingProgress = new Dictionary<Town, int>();
    private int lastCheckedYear;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (GameManager.instance != null)
        {
            lastCheckedYear = GameManager.instance.Year;
        }

        SafeSetActive(buildListPanel, false);
        if (centerTurnText != null) SafeSetActive(centerTurnText.gameObject, false);
        ToggleCraftingSlots(false);
    }

    void Update()
    {
        if (GameManager.instance == null) return;

        if (GameManager.instance.Year > lastCheckedYear)
        {
            lastCheckedYear = GameManager.instance.Year;
            AdvanceConstructionTurns();
        }

        if (UIManager.instance != null && UIManager.instance.TownPanel.activeInHierarchy)
        {
            UpdateWarehouseUI();
        }
        else
        {
            SafeSetActive(buildListPanel, false);
        }
    }

    private void AdvanceConstructionTurns()
    {
        if (buildingProgress.Count == 0) return;

        List<Town> townsInProgress = new List<Town>(buildingProgress.Keys);

        foreach (Town town in townsInProgress)
        {
            buildingProgress[town]--;

            if (buildingProgress[town] <= 0)
            {
                buildingProgress.Remove(town);
                completedTowns.Add(town);
            }
        }
    }

    private void UpdateWarehouseUI()
    {
        Town curTown = GameManager.instance.CurTown;
        if (curTown == null) return;

        if (openBuildPanelBtn != null && !openBuildPanelBtn.interactable)
            openBuildPanelBtn.interactable = true;

        if (warehouseImage != null)
            SafeSetActive(warehouseImage.gameObject, true);

        if (completedTowns.Contains(curTown))
        {
            // 3. สร้างเสร็จแล้ว
            if (warehouseImage != null)
            {
                if (warehouseImage.sprite != finishSprite) warehouseImage.sprite = finishSprite;
                SetImageAlpha(warehouseImage, 1f);
            }
            if (centerTurnText != null) SafeSetActive(centerTurnText.gameObject, false);
            ToggleCraftingSlots(true);
        }
        else if (buildingProgress.ContainsKey(curTown))
        {
            // 2. กำลังสร้าง
            if (warehouseImage != null)
            {
                if (warehouseImage.sprite != finishSprite) warehouseImage.sprite = finishSprite;
                SetImageAlpha(warehouseImage, 0.5f);
            }
            if (centerTurnText != null)
            {
                int turnsLeft = buildingProgress[curTown];
                string newText = (turnsLeft > 1) ? $"{turnsLeft} Turns" : $"{turnsLeft} Turn";
                
                SafeSetActive(centerTurnText.gameObject, true);
                if (centerTurnText.text != newText) centerTurnText.text = newText;
            }
            ToggleCraftingSlots(false);
        }
        else
        {
            // 1. ยังไม่สร้าง
            if (warehouseImage != null)
            {
                if (warehouseImage.sprite != emptySprite) warehouseImage.sprite = emptySprite;
                SetImageAlpha(warehouseImage, 1f);
            }
            if (centerTurnText != null) SafeSetActive(centerTurnText.gameObject, false);
            ToggleCraftingSlots(false);
        }
    }

    // ฟังก์ชันสั่ง SetActive เฉพาะตอนที่สถานะเปลี่ยนจริงๆ (แก้ปัญหากระพริบ 100%)
    private void SafeSetActive(GameObject obj, bool state)
    {
        if (obj != null && obj.activeSelf != state)
        {
            obj.SetActive(state);
        }
    }

    private void ToggleCraftingSlots(bool isActive)
    {
        SafeSetActive(inputSlotObj, isActive);
       
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        if (Mathf.Approximately(img.color.a, alpha)) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void OnClickOpenBuildListPanel()
    {
        Town curTown = GameManager.instance.CurTown;
        if (curTown == null) return;

        if (buildListPanel != null)
        {
            SafeSetActive(buildListPanel, true);
            buildListPanel.transform.SetAsLastSibling();

            int targetTurns = (turnsToBuild <= 0) ? 2 : turnsToBuild;

            if (completedTowns.Contains(curTown))
            {
                if (warehousePriceText != null) warehousePriceText.text = "Built (Max 1)";
                if (selectWarehouseBtn != null) selectWarehouseBtn.interactable = false;
            }
            else if (buildingProgress.ContainsKey(curTown))
            {
                int turnsLeft = buildingProgress[curTown];
                if (warehousePriceText != null) warehousePriceText.text = $"Building ({turnsLeft} Turns)";
                if (selectWarehouseBtn != null) selectWarehouseBtn.interactable = false;
            }
            else
            {
                if (warehousePriceText != null) warehousePriceText.text = $"{buildCost} Gold ({targetTurns} Turns)";
                if (selectWarehouseBtn != null) selectWarehouseBtn.interactable = (GameManager.instance.PlayerFaction.Money >= buildCost);
            }
        }
    }

    public void OnClickSelectWarehouseToBuild()
    {
        Town curTown = GameManager.instance.CurTown;
        if (curTown == null) return;

        if (completedTowns.Contains(curTown) || buildingProgress.ContainsKey(curTown))
            return;

        if (GameManager.instance.PlayerFaction.Money >= buildCost)
        {
            GameManager.instance.PlayerFaction.Money -= buildCost;

            int targetTurns = (turnsToBuild <= 0) ? 2 : turnsToBuild;
            buildingProgress.Add(curTown, targetTurns);

            UIManager.instance.UpdateMoneyText();
            SafeSetActive(buildListPanel, false);
            UpdateWarehouseUI();
        }
    }

    public void OnClickCloseBuildListPanel()
    {
        SafeSetActive(buildListPanel, false);
    }

    public bool HasWarehouse(Town town)
    {
        return completedTowns.Contains(town);
    }
}

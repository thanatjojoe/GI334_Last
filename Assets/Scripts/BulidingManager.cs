using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;

    [Header("Town Panel UI References")]
    [SerializeField] private Button openBuildPanelBtn;     // ปุ่ม Build บนหน้าต่างเมือง
    [SerializeField] private Image warehouseImage;         // รูปภาพโกดัง
    [SerializeField] private TMP_Text centerTurnText;      // ลาก Text ตัวเลขจากใน Scene มาใส่ช่องนี้

    [Header("Build Selection Panel")]
    [SerializeField] private GameObject buildListPanel;    // Panel รายการสิ่งปลูกสร้าง
    [SerializeField] private Button selectWarehouseBtn;    // ปุ่มแถบ WAREHOUSE ใน Panel
    [SerializeField] private TMP_Text warehousePriceText;  // ข้อความราคาในแถบ WAREHOUSE

    [Header("Sprites (Empty -> Finish)")]
    [SerializeField] private Sprite emptySprite;           // 1. ภาพพื้นที่ว่างเปล่า
    [SerializeField] private Sprite finishSprite;          // 2. ภาพโกดัง (ใช้ทั้งตอนกำลังสร้าง 50% และสร้างเสร็จ 100%)

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

        if (buildListPanel != null)
        {
            buildListPanel.SetActive(false);
        }

        // ซ่อน Text ตัวเลขไว้ตั้งแต่เริ่มเกม
        if (centerTurnText != null)
        {
            centerTurnText.gameObject.SetActive(false);
        }
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
        else if (buildListPanel != null && buildListPanel.activeInHierarchy)
        {
            buildListPanel.SetActive(false);
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

        if (openBuildPanelBtn != null)
            openBuildPanelBtn.interactable = true;

        if (warehouseImage != null)
            warehouseImage.gameObject.SetActive(true);

        if (completedTowns.Contains(curTown))
        {
            // 3. สร้างเสร็จแล้ว -> ซ่อน Text ตัวเลข (SetActive = false)
            if (warehouseImage != null)
            {
                warehouseImage.sprite = finishSprite;
                SetImageAlpha(warehouseImage, 1f);
            }
            if (centerTurnText != null)
                centerTurnText.gameObject.SetActive(false);
        }
        else if (buildingProgress.ContainsKey(curTown))
        {
            // 2. กำลังสร้าง -> เปิดโชว์ Text (2 Turns / 1 Turn)
            if (warehouseImage != null)
            {
                warehouseImage.sprite = finishSprite;
                SetImageAlpha(warehouseImage, 0.5f);
            }
            if (centerTurnText != null)
            {
                int turnsLeft = buildingProgress[curTown];
                centerTurnText.gameObject.SetActive(true);
                centerTurnText.text = (turnsLeft > 1) ? $"{turnsLeft} Turns" : $"{turnsLeft} Turn";
            }
        }
        else
        {
            // 1. ยังไม่สร้าง -> ซ่อน Text ตัวเลข (SetActive = false)
            if (warehouseImage != null)
            {
                warehouseImage.sprite = emptySprite;
                SetImageAlpha(warehouseImage, 1f);
            }
            if (centerTurnText != null)
                centerTurnText.gameObject.SetActive(false);
        }
    }

    private void SetImageAlpha(Image img, float alpha)
    {
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
            buildListPanel.SetActive(true);
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

            if (buildListPanel != null)
                buildListPanel.SetActive(false);

            // เปิดโชว์ตัวเลขเทิร์นทันทีที่กดปุ่มสร้าง
            if (centerTurnText != null)
            {
                centerTurnText.gameObject.SetActive(true);
                centerTurnText.text = targetTurns.ToString();
            }

            UpdateWarehouseUI();
        }
    }

    public void OnClickCloseBuildListPanel()
    {
        if (buildListPanel != null)
            buildListPanel.SetActive(false);
    }

    public bool HasWarehouse(Town town)
    {
        return completedTowns.Contains(town);
    }
}

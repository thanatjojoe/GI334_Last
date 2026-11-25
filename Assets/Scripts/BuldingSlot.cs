using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildingSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private int furProductId = 4;
    [SerializeField] private int coatProductId = 12;
    [SerializeField] private int processTurns = 2;

    [SerializeField] private Image inputIconImage;
    [SerializeField] private TMP_Text inputStatusText;

    private Sprite defaultInputBg;

    private class TownCraftData
    {
        public int processingAmount = 0;
        public int turnsLeft = 0;
    }

    private Dictionary<Town, TownCraftData> townDataDict = new Dictionary<Town, TownCraftData>();
    private int lastCheckedYear;

    void Awake()
    {
        if (inputIconImage != null) defaultInputBg = inputIconImage.sprite;
    }

    void Start()
    {
        if (GameManager.instance != null)
        {
            lastCheckedYear = GameManager.instance.Year;
        }
    }

    void Update()
    {
        if (GameManager.instance == null) return;

        if (GameManager.instance.Year > lastCheckedYear)
        {
            lastCheckedYear = GameManager.instance.Year;
            AdvanceCraftingTurns();
        }

        if (UIManager.instance != null && UIManager.instance.TownPanel.activeInHierarchy)
        {
            UpdateSlotUI();
        }
    }

    private TownCraftData GetOrCreateTownData(Town town)
    {
        if (!townDataDict.ContainsKey(town))
        {
            townDataDict[town] = new TownCraftData();
        }
        return townDataDict[town];
    }

    public void OnDrop(PointerEventData eventData)
    {
        Town curTown = GameManager.instance.CurTown;
        if (curTown == null) return;

        TownCraftData data = GetOrCreateTownData(curTown);

        if (data.processingAmount > 0)
            return;

        GameObject obj = eventData.pointerDrag;
        if (obj == null) return;

        StockDrag stockDrag = obj.GetComponent<StockDrag>();
        if (stockDrag == null)
            return;

        if (stockDrag.Cargo.ProductID != furProductId)
            return;

        int dragQuantity = stockDrag.Cargo.Quantity;
        if (dragQuantity <= 0)
            return;

        int amountToTake = Mathf.Min(100, dragQuantity);
        int cargoLeft = dragQuantity - amountToTake;

        data.processingAmount = amountToTake;
        data.turnsLeft = processTurns;

        stockDrag.Cargo.Quantity = cargoLeft;

        UpdateSlotUI();
    }

    private void AdvanceCraftingTurns()
    {
        foreach (var kvp in townDataDict)
        {
            Town town = kvp.Key;
            TownCraftData data = kvp.Value;

            if (data.processingAmount > 0)
            {
                data.turnsLeft--;

                if (data.turnsLeft <= 0)
                {
                    AddStockToSlot(coatProductId, data.processingAmount, town);

                    data.processingAmount = 0;
                    data.turnsLeft = 0;
                }
            }
        }
    }

    private void AddStockToSlot(int targetProductId, int amountChange, Town targetTown)
    {
        if (GameManager.instance.CurTown == targetTown && UIManager.instance.TownPanel.activeInHierarchy)
        {
            StockSlot[] slots = Object.FindObjectsByType<StockSlot>(FindObjectsSortMode.None);
            foreach (StockSlot slot in slots)
            {
                if (slot.ProductId == targetProductId)
                {
                    slot.UpdateQuantityStock(amountChange);

                    StockDrag dragItem = slot.GetComponentInChildren<StockDrag>();
                    if (dragItem != null && dragItem.Cargo != null)
                    {
                        dragItem.Cargo.Quantity = slot.Quantity;
                    }
                    return;
                }
            }
        }

        targetTown.Warehouse[targetProductId] += amountChange;
    }

    private void UpdateSlotUI()
    {
        Town curTown = GameManager.instance.CurTown;
        if (curTown == null) return;

        TownCraftData data = GetOrCreateTownData(curTown);

        if (data.processingAmount > 0)
        {
            if (inputIconImage != null)
            {
                Sprite[] furIcons = GameManager.instance.ProductData[furProductId].icons;
                if (furIcons != null && furIcons.Length > 0)
                    inputIconImage.sprite = furIcons.FirstOrDefault();
            }
            if (inputStatusText != null)
            {
                inputStatusText.text = $"{data.processingAmount}\n({data.turnsLeft}T)";
            }
        }
        else
        {
            if (inputIconImage != null) inputIconImage.sprite = defaultInputBg;
            if (inputStatusText != null) inputStatusText.text = "";
        }
    }
}

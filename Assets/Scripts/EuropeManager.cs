using UnityEngine;
using System.Collections.Generic;

public class EuropeManager : MonoBehaviour
{
    private const int EUROPE_DISTANCE = 3;

    [SerializeField] //Ships To Europe
    private List<ShipInTransit> shipsToEurope = new List<ShipInTransit>();
    public List<ShipInTransit> ShipsToEurope { get { return shipsToEurope; } set { shipsToEurope = value; } }

    [SerializeField] //Ships From Europe
    private List<ShipInTransit> shipsFromEurope = new List<ShipInTransit>();
    public List<ShipInTransit> ShipsFromEurope { get { return shipsFromEurope; } set { shipsFromEurope = value; } }

    [SerializeField]
    private List<NavalUnit> shipsInEurope = new List<NavalUnit>();
    public List<NavalUnit> ShipsInEurope { get { return shipsInEurope; } set { shipsInEurope = value; } }

    [SerializeField]
    private EUStock[] europeStocks = new EUStock[16];
    public EUStock[] EuropeStocks { get { return europeStocks; } set { europeStocks = value; } }

    [SerializeField]
    private NavalUnit curShip;
    public NavalUnit CurShip { get { return curShip; } set { curShip = value; } }

    [SerializeField]
    private List<LandUnit> landUnitsInEurope; //people waiting at the port
    public List<LandUnit> LandUnitsInEurope { get { return landUnitsInEurope; } set { landUnitsInEurope = value; } }

    private int normalStock = 500;

    public static EuropeManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitStartPrice();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitStartPrice()
    {
        GameManager gameMgr = GameManager.instance;

        for (int i = 0; i < europeStocks.Length; i++)
        {
            europeStocks[i] =
                new EUStock(i,
                gameMgr.ProductData[i].startBid, gameMgr.ProductData[i].startAsk,
                normalStock);
            
        }
    }

    public void QuestionGoToEurope(NavalUnit ship)
    {
        curShip = ship;
        DialogManager.instance.GoToEuropeQuestion();
    }

    public void AllowToGoToEurope()
    {
        ShipInTransit shipInTransit = new ShipInTransit(curShip, EUROPE_DISTANCE);
        shipsToEurope.Add(shipInTransit);

        GameManager.instance.LeaveSeenFogAroundUnit(curShip);
        curShip.CurHex.UnitsInHex.Remove(curShip);
        curShip.CurHex = null;
        curShip.gameObject.SetActive(false);
    }

    public void UpdateShipInTransitTurn()
    {
        //To Europe
        List<ShipInTransit> shipsReachEurope = new List<ShipInTransit>();

        foreach (ShipInTransit shipInTransit in shipsToEurope)
        {
            if (shipInTransit.TurnLeft == 1)
            {
                shipsInEurope.Add(shipInTransit.Ship);
                shipsReachEurope.Add(shipInTransit);

                //Passenger arrives Europe
                foreach (LandUnit passenger in shipInTransit.Ship.Passengers)
                {
                    passenger.UnitStatus = UnitStatus.None;
                    landUnitsInEurope.Add(passenger);
                }
                shipInTransit.Ship.Passengers.Clear();
            }
            else
            {
                shipInTransit.TurnLeft--;
                if (shipInTransit.TurnLeft < 1)
                    shipInTransit.TurnLeft = 1;
            }
        }
        //Remove from Transit
        foreach (ShipInTransit shipReachEU in shipsReachEurope)
        {
            if (shipsToEurope.Contains(shipReachEU))
                shipsToEurope.Remove(shipReachEU);
        }
        shipsReachEurope.Clear();

        //--------------

        //From Europe
        List<ShipInTransit> shipsReachNewWorld = new List<ShipInTransit>();

        foreach (ShipInTransit shipInTransit in shipsFromEurope)
        {
            if (shipInTransit.TurnLeft == 1)
            {
                NavalUnit ship = shipInTransit.Ship;

                //Show up in Map01
                GameManager.instance.CheckToGenerateShipFromEurope(ship);
                shipsReachNewWorld.Add(shipInTransit);
            }
            else
            {
                shipInTransit.TurnLeft--;
                if (shipInTransit.TurnLeft < 1)
                    shipInTransit.TurnLeft = 1;
            }
        }
        //Remove from Transit
        foreach (ShipInTransit shipReachNW in shipsReachNewWorld)
        {
            if (shipsFromEurope.Contains(shipReachNW))
                shipsFromEurope.Remove(shipReachNW);
        }
        shipsReachEurope.Clear();
    }

    public void UpdateEuropePrice(int i)
    {
        int diffStock = europeStocks[i].Quantity - normalStock;
        //Debug.Log($"Diff Stock-{i} = {diffStock}");

        int diffPrice = diffStock / -100;
        //Debug.Log($"Diff Price-{i} = {diffPrice}");

        europeStocks[i].UpdatePrice(diffPrice);

        if (diffPrice != 0)
            DialogManager.instance.PriceChangeNotification(i, europeStocks[i].BidPrice);
    }

    public void AllowToGoToNewWorld(NavalUnit ship)
    {
        ShipInTransit shipInTransit = new ShipInTransit(ship, EUROPE_DISTANCE);

        if (!shipsFromEurope.Contains(shipInTransit))
            shipsFromEurope.Add(shipInTransit);

        shipsInEurope.Remove(ship);

        UIManager.instance.UpdateIconsFromEuropeToNewWorld();
    }

    public void CheckIfPassengerInEuropeReadyToBoardShip(NavalUnit ship)
    {
        List<LandUnit> unitsToRemove = new List<LandUnit>();

        foreach (LandUnit landUnit in landUnitsInEurope)
        {
            if (landUnit.UnitType == UnitType.Land && landUnit.UnitStatus == UnitStatus.ToBoard)
            {
                if ((ship.Passengers.Count + ship.CargoList.Count) < ship.CargoHoldNum)
                {
                    landUnit.BoardingShip(ship);
                    landUnit.gameObject.SetActive(false);
                    unitsToRemove.Add(landUnit);
                }
            }
        }

        foreach (LandUnit unit in unitsToRemove)
            landUnitsInEurope.Remove(unit);
    }

    public void ConsumeEUStock()
    {
        for (int i = 0; i < europeStocks.Length; i++)
        {
            europeStocks[i].UpdateQuantity(Random.Range(-1, -25));
            UpdateEuropePrice(i);
        }
    }

    public void RandomImmigrantsToNewWorld()
    {
        int n = Random.Range(0, 100);

        if (n < 10)
        {
            int max = GameManager.instance.LandUnitData.Length;
            int id = Random.Range(0, max);
            if (id == 3 || id == 4)//Indian
                return;
            UIManager.instance.CreateRandomImmigrantsUnitDragInEuropePort(id);
            DialogManager.instance.ImmigrantsNotification(id);
        }
    }
}

using System.Collections;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AutoMoveToHex(Unit unit)
    {
        unit.ShowHideSprite(true);

        if (unit.DestinationHex != null)
            unit.CheckMoveToDestination();
        else if (unit.Faction.AtPeaceWithPlayer == false)
        {
            Unit playerUnit = TryToFindPlayerUnit(unit);

            if (playerUnit != null)
                unit.AttackUnit(playerUnit.CurHex, playerUnit);
            else
                RandomMoveOneHex(unit);
        }
        else
            RandomMoveOneHex(unit);
    }

    private IEnumerator EnemyUnitMoves()
    {
        foreach (Faction faction in GameManager.instance.Factions)
        {
            GameManager.instance.ResetAllUnits(faction);

            if (faction == GameManager.instance.PlayerFaction)
                continue;

            for (int i = 0; i < faction.Units.Count; i++)
            {
                GameManager.instance.SelectAiUnit(faction.Units[i]);

                if (faction.Units[i].Visible)
                    CameraController.instance.MoveCamera(faction.Units[i].CurPos);

                if (faction.Units[i].UnitStatus == UnitStatus.OnBoard)
                    continue;
                //Debug.Log("Move");
                AutoMoveToHex(faction.Units[i]);

                yield return new WaitForSeconds(0.1f);
            }
        }
        GameManager.instance.GameTurn++;
        //New Turn Dialog and Focus on Player's unit
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //Debug.Log("Release Mouse");
        //Debug.Log("New Turn");

        GameManager.instance.StartNewTurn();
    }

    public void StartAITurn()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(EnemyUnitMoves());
    }

    public void RandomMoveOneHex(Unit unit)
    {
        Hex anyHex = null;

        int n = Random.Range(0, 6);

        //Move
        anyHex = HexCalculator.FindHexByDir(unit.CurHex, (HexDirection)n, GameManager.instance.AllHexes);

        if (anyHex == null)
            return;

        Unit otherUnit = HexCalculator.CheckIfThisHexHasOtherFactionUnit(anyHex, unit.Faction);

        if (otherUnit == null) //No other faction's unit in that hex
        {
            AIUnitPrepareToMove(unit, anyHex);
        }
        else if (otherUnit.Faction == GameManager.instance.PlayerFaction)//that other unit is player
        {
            if (unit.Faction.AtPeaceWithPlayer)
                AIUnitStayOnHex(unit);
            else
                unit.AttackUnit(anyHex, otherUnit);
        }
        else //other AI faction's unit in that hex
        {
            unit.AttackUnit(anyHex, otherUnit);//attack other AI
        }
    }

    private void AIUnitPrepareToMove(Unit unit, Hex targetHex)
    {
        switch (unit.UnitType)
        {
            case UnitType.Land:
                LandUnit landUnit = (LandUnit)unit;
                landUnit.PrepareMoveToHex(targetHex);
                break;
            case UnitType.Naval:
                NavalUnit navalUnit = (NavalUnit)unit;
                navalUnit.PrepareMoveToHex(targetHex);
                break;
        }
    }

    private void AIUnitStayOnHex(Unit unit)
    {
        switch (unit.UnitType)
        {
            case UnitType.Land:
                LandUnit landUnit = (LandUnit)unit;
                landUnit.StayOnHex(landUnit.CurHex);
                break;
            case UnitType.Naval:
                NavalUnit navalUnit = (NavalUnit)unit;
                navalUnit.StayOnHex(navalUnit.CurHex);
                break;
        }
    }

    private Unit TryToFindPlayerUnit(Unit unit)
    {
        Unit otherUnit = HexCalculator.CheckIfThisHexHasOtherFactionUnit(unit.CurHex, unit.Faction);

        if (otherUnit == null)
            return null;

        if (otherUnit.Faction == GameManager.instance.PlayerFaction)
            return otherUnit;
        else
            return null;
    }
}

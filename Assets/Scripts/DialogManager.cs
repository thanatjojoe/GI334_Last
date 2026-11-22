using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    private Dialog[] dialogs; //0 - Naval, 1 - AI move, 2 - Army, 3 - Foreign, 4 - Scout, 5 - King, 6 - Colony, 7 - Trade, 8 - Religious, 9 - Combat

    [SerializeField]
    private DialogData[] dialogData;

    [SerializeField]
    private GameObject[] eventPanels;

    public static DialogManager instance;

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

    #region Naval Dialog
    public void GoToEuropeQuestion()
    {
        dialogs[0].gameObject.SetActive(true);
        dialogs[0].DialogGoToEuropeInit(dialogData[0]);
    }

    public void YesGoToEurope()
    {
        dialogs[0].gameObject.SetActive(false);
        EuropeManager.instance.AllowToGoToEurope();
    }

    public void NoGoToEurope()
    {
        dialogs[0].gameObject.SetActive(false);
    }

    public void ToggleNavalDialog(bool flag)
    {
        dialogs[0].gameObject.SetActive(flag);
    }

    public void ShowNavalCombatResult(Unit winner, Unit loser)
    {
        dialogs[0].gameObject.SetActive(true);
        dialogs[0].DialogCombatResultInit(winner, loser);
    }
    #endregion

    #region AI move Dialog
    public void ToggleAiMoveDialog(bool flag)
    {
        if (flag)
            dialogs[1].NotiText.text = ($"Year: {GameManager.instance.Year}\nAI is moving...");

        dialogs[1].gameObject.SetActive(flag);
    }
    #endregion

    #region Army Dialog
    public void ToggleArmyDialog(bool flag)
    {
        dialogs[2].gameObject.SetActive(flag);
    }

    public void WhetherToDeclareWarOnNation()
    {
        GameManager.instance.CurForeignFaction.MetByPlayer = true;
        dialogs[2].ToDeclareWarQuestion(GameManager.instance.CurForeignFaction);
    }

    public void LetsDeclareWarOnThem()//Map with Yes (Declare war on them) button
    {
        GameManager.instance.CurForeignFaction.AtPeaceWithPlayer = false;
        dialogs[2].YouHaveDeclaredWarByAttack(GameManager.instance.CurForeignFaction);
    }

    public void AttackAfterDeclareWar()
    {
        ToggleArmyDialog(false);
        if (GameManager.instance.CurTarget != null)
        {
            Unit defender = GameManager.instance.CurTarget;
            GameManager.instance.CurUnit.AttackUnit(defender.CurHex, defender);
        }
    }

    public void ShowArmyCombatResult(Unit winner, Unit loser)
    {
        dialogs[2].gameObject.SetActive(true);
        dialogs[2].DialogCombatResultInit(winner, loser);
    }
    #endregion

    #region Foreign Dialog
    public void ToggleForeignDialog(bool flag)
    {
        dialogs[3].gameObject.SetActive(flag);

        if (flag)
            dialogs[3].DialogFactionIntroInit(GameManager.instance.CurForeignFaction);
        else
            GameManager.instance.CurForeignFaction = null;
    }

    public void MakePeaceWithNation(bool flag)
    {
        GameManager.instance.CurForeignFaction.MetByPlayer = true;
        GameManager.instance.CurForeignFaction.AtPeaceWithPlayer = flag;

        if (flag)
            ToggleForeignDialog(false);
        else
            dialogs[3].ThisMeansWar(GameManager.instance.CurForeignFaction);
    }
    #endregion

    #region Scout Dialog
    public void ToggleScoutDialog(bool flag)
    {
        dialogs[4].gameObject.SetActive(flag);
    }

    public void ShowScoutCombatResult(Unit winner, Unit loser)
    {
        dialogs[4].gameObject.SetActive(true);
        dialogs[4].DialogCombatResultInit(winner, loser);
    }

    public void ShowScoutToMakeLandFall()
    {
        dialogs[4].gameObject.SetActive(true);
        dialogs[4].NotifyAboutMakingLandFall();
    }
    #endregion

    #region King Dialog
    public void ToggleKingDialog(bool flag)
    {
        dialogs[5].gameObject.SetActive(flag);
    }

    public void ShowFirstKingQuestDialogText()
    {
        ToggleKingDialog(true);
        string text = "Establish few colonies and sell these raw materials to our market: <color=yellow>Sugar</color>," +
            " <color=yellow>Tobacco</color> and <color=yellow>Cotton</color>.";
        dialogs[5].ShowNotiText(text);
    }
    #endregion

    #region Colony Dialog
    public void ToggleColonyDialog(bool flag)
    {
        dialogs[6].gameObject.SetActive(flag);
    }

    public void ShowColonyClearLandText()
    {
        ToggleColonyDialog(true);
        string text = "You must build a settlement on cleared land or hill, not forest or mountain. Press <color=yellow>P</color> to clear forest.";
        dialogs[6].ShowNotiText(text);
    }

    public void ShowColonyNotEnoughToolsText(int n)
    {
        ToggleColonyDialog(true);
        string text = $"You don't have enough tools. This unit currently has only <color=yellow>{n}</color> tools." +
            $"You must have at least 20 tools. Buy more tools from Europe or get it from Blacksmith's house";
        dialogs[6].ShowNotiText(text);
    }

    public void ShowColonyHasOtherTownAround()
    {
        ToggleColonyDialog(true);
        string text = $"You can't build a settlement next to another settlement or village.";
        dialogs[6].ShowNotiText(text);
    }

    public void ShowColonyNotifyBuildSettlement()
    {
        ToggleColonyDialog(true);
        dialogs[6].NotifyAboutBuildingSettlement();
    }
    #endregion

    #region Trade Dialog
    public void ToggleTradeDialog(bool flag)
    {
        dialogs[7].gameObject.SetActive(flag);
    }

    public void PriceChangeNotification(int id, int newPrice)
    {
        dialogs[7].gameObject.SetActive(true);

        string productName = GameManager.instance.ProductData[id].productName;
        string newPriceString = newPrice.ToString();

        dialogs[7].PriceInEuropeChange(productName, newPriceString);
    }
    #endregion

    #region Religious Dialog
    public void ToggleReligiousDialog(bool flag)
    {
        dialogs[8].gameObject.SetActive(flag);
    }

    public void ImmigrantsNotification(int id)
    {
        ToggleReligiousDialog(true);

        string unitName = GameManager.instance.LandUnitData[id].unitName;
        dialogs[8].NotifyAboutImmigrantsWaiting(unitName);
    }
    #endregion

    #region Combat Dialog
    public void CombatAnalysisInit(Unit ourUnit, Unit enemyUnit)
    {
        dialogs[9].DialogCombatAnalysisInit(ourUnit, enemyUnit);
    }

    public void ToggleCombatPanel(bool flag)
    {
        dialogs[9].gameObject.SetActive(flag);
    }
    #endregion

    #region Events
    public void ToggleNewLandEvent(bool flag)
    {
        eventPanels[0].SetActive(flag);
    }

    public void ToggleMakeLandfallEvent(bool flag)
    {
        eventPanels[1].SetActive(flag);
    }
    #endregion
}

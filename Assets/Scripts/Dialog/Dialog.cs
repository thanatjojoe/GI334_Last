using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    [SerializeField]
    private TMP_Text notiText;
    public TMP_Text NotiText { get { return notiText; } set { notiText = value; } }

    [SerializeField]
    private TMP_Text yesText;

    [SerializeField]
    private TMP_Text noText;

    [SerializeField]
    private Button btnYes;

    [SerializeField]
    private Button btnNo;

    [SerializeField]
    private Button btnOkay;

    [SerializeField]
    private TMP_Text okayText;

    [SerializeField]
    private Image charImage;

    [Header("Combat Dialog")]

    //Attacker
    [SerializeField]
    private Image attackerFactionImage;

    [SerializeField]
    private Image[] attackerImage;

    [SerializeField]
    private TMP_Text[] attackerNameText;

    [SerializeField]
    private TMP_Text[] attackerStrText;

    //Defender
    [SerializeField]
    private Image defenderFactionImage;

    [SerializeField]
    private Image[] defenderImage;

    [SerializeField]
    private TMP_Text[] defenderNameText;

    [SerializeField]
    private TMP_Text[] defenderStrText;

    private DialogManager dialogMgr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogMgr = DialogManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DialogGoToEuropeInit(DialogData data)
    {
        if (notiText != null)
            notiText.text = data.question;

        if (yesText != null)
            yesText.text = data.answers[0];

        if (noText != null)
            noText.text = data.answers[1];
    }

    public void DialogCombatAnalysisInit(Unit attacker, Unit defender)
    {
        //Debug.Log(attackerFactionImage);
        attackerFactionImage.sprite = attacker.Faction.ShieldIcon;
        attackerImage[0].sprite = attacker.UnitSprite.sprite;
        attackerNameText[0].text = attacker.UnitName;
        attackerStrText[0].text = attacker.Strength.ToString();

        defenderFactionImage.sprite = defender.Faction.ShieldIcon;
        defenderImage[0].sprite = defender.UnitSprite.sprite;
        defenderNameText[0].text = defender.UnitName;
        defenderStrText[0].text = defender.Strength.ToString();
    }

    public void DialogCombatResultInit(Unit winner, Unit loser)
    {
        if (notiText != null)
            notiText.text =
                ($"<color=yellow>{winner.Faction.Nation} {winner.UnitName}</color> defeats " +
                $"<color=yellow>{loser.Faction.Nation} {loser.UnitName}</color> ");

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
        {
            btnOkay.gameObject.SetActive(true);
            btnOkay.onClick.RemoveAllListeners();
            btnOkay.onClick.AddListener(() => dialogMgr.ToggleArmyDialog(false));
        }
    }

    public void DialogFactionIntroInit(Faction faction)
    {
        if (charImage != null && faction.LeaderSprite != null)
            charImage.sprite = faction.LeaderSprite;

        if (notiText != null)
        {
            if (faction.IsEuropean)
                notiText.text =
                    ($"Greetings and welcome to <color=yellow>New {faction.Nation}</color>. " +
                    $"We have justly claimed all of this land in the name of King of " +
                    $"<color=yellow>{faction.Nation}</color>. Please do not interfere with us. " +
                    $"We propose a <color=yellow>peace treaty</color>. Do you agree?");
            else
                notiText.text =
                    ($"The <color=yellow>{faction.Nation}</color> tribe welcomes you." +
                    $"To celebrate our friendship, we generously offer you " +
                    $"the land you now occupy as a gift. Will you accept our treaty " +
                    $"and live with us in peace as brothers?");
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(true);

        if (btnNo != null)
            btnNo.gameObject.SetActive(true);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(false);
    }

    public void ShowNotiText(string s)
    {
        notiText.text = s;

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(true);
    }

    public void ThisMeansWar(Faction faction)
    {
        if (notiText != null)
        {
            if (faction.IsEuropean)
                notiText.text =
                    ($"This means you are at war with <color=yellow>{faction.Nation}</color> already. " +
                    $"See you on the battlefield!");
            else
                notiText.text =
                    ($"My tribe, <color=yellow>{faction.Nation}</color> tribe will kill all of you. " +
                    $"Prepare to Die!!!");
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
        {
            btnOkay.gameObject.SetActive(true);
            btnOkay.onClick.RemoveAllListeners();
            btnOkay.onClick.AddListener(() => dialogMgr.AttackAfterDeclareWar());
        }
    }

    public void ToDeclareWarQuestion(Faction faction)
    {
        if (notiText != null)
        {
            notiText.text =
                ($"We are currently at peace with <color=yellow>{faction.Nation}</color>. " +
                $"Do you really want to attack?");
        }

        if (yesText != null)
            yesText.text = ($"Yes, let's declare WAR on them!!!");

        if (noText != null)
            noText.text = ($"No, call off the attack.");

        if (btnYes != null)
            btnYes.gameObject.SetActive(true);

        if (btnNo != null)
            btnNo.gameObject.SetActive(true);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(false);
    }

    public void YouHaveDeclaredWarByAttack(Faction faction)
    {
        if (notiText != null)
        {
            notiText.text =
                ($"You are now at war with <color=yellow>{faction.Nation}</color>.");
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(true);
    }

    public void PriceInEuropeChange(string productName, string newPrice)
    {
        if (notiText != null)
        {
            notiText.text = 
                $"<color=yellow>{productName}</color> price in Europe changes to <color=yellow>{newPrice}</color>.";
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
        {
            btnOkay.gameObject.SetActive(true);
            btnOkay.onClick.RemoveAllListeners();
            btnOkay.onClick.AddListener(() => dialogMgr.ToggleTradeDialog(false));
        }
    }

    public void NotifyAboutImmigrantsWaiting(string unitName)
    {
        if (notiText != null)
        {
            notiText.text =
                $"Due to religious unrest, <color=yellow>{unitName}</color> wants to immigrate to the new world. " +
                $"They are waiting at your Europe home port. Press <color=yellow>E</color>.";
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
        {
            btnOkay.gameObject.SetActive(true);
            btnOkay.onClick.RemoveAllListeners();
            btnOkay.onClick.AddListener(() => dialogMgr.ToggleReligiousDialog(false));
        }
    }

    public void NotifyAboutMakingLandFall()
    {
        if (notiText != null)
        {
            notiText.text =
                $"Press <color=yellow>Tab</color> to select other <color=yellow>Land Units</color>. " +
                $"Then right click on land to make landfall.";
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
        {
            btnOkay.gameObject.SetActive(true);
        }
    }

    public void NotifyAboutBuildingSettlement()
    {
        if (notiText != null)
        {
            notiText.text =
                $"Find a suitable clear land and press <color=yellow>B</color> to build a settlement. " +
                $"You can press <color=yellow>P</color> to cut trees.";
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
        {
            btnOkay.gameObject.SetActive(true);
            btnOkay.onClick.RemoveAllListeners();
            btnOkay.onClick.AddListener(() => dialogMgr.ToggleColonyDialog(false));
        }
    }
}

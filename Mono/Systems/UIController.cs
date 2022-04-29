using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// English: A class that will control most of the UI elements (including functional buttons and commands) that visualizes the current state of the game
/// 日本語：ゲームの状況を表すほとんどのUI要素（機能のあるボタンやコマンドを含め）をコントロールするクラス。
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("Select System")]
    [SerializeField]
    private SelectSystem selectSystem;

    // *********** Bottom Left of the Screen  ************
    [Header("Left_Minimap")]
    [SerializeField]
    private GameObject miniMap;
    [SerializeField]
    private Texture miniMapTexture;

    // **********  Bottom Middle of the Screen  ***********
    [Header("Middle_Panel")]
    [SerializeField]
    private GameObject displayPanel;
    [SerializeField]
    private GameObject curGroupDisplayPanel;

    [Header("Current Structure")]
    [SerializeField]
    private GameObject curStructureDisplayPanel;
    [SerializeField]
    private Image curStructurePortrait;
    [SerializeField]
    private TMPro.TextMeshProUGUI curStructureHealthText;
    [SerializeField]
    private TMPro.TextMeshProUGUI curStructureNameText;
    [SerializeField]
    private Image curStructureDefenseImage;
    [SerializeField]
    private Image curStructureAttackImage;

    [Header("Current Unit")]
    [SerializeField]
    private GameObject curUnitDisplayPanel;
    [SerializeField]
    private Image curUnitPortrait;
    [SerializeField]
    private TMPro.TextMeshProUGUI curUnitHealthText;
    [SerializeField]
    private TMPro.TextMeshProUGUI curUnitNameText;
    [SerializeField]
    private Image curUnitDefenseImage;
    [SerializeField]
    private Image curUnitAttackImage;

    [Header("Current Production")]
    [SerializeField]
    private GameObject producerDisplayPanel;
    [SerializeField]
    private Image producerPortrait;
    [SerializeField]
    private TMPro.TextMeshProUGUI producerHealthText;
    [SerializeField]
    private TMPro.TextMeshProUGUI producerNameText;
    [SerializeField]
    private ProgressBar progressBar;

    // **********  Bottom Middle of the Screen, on top of the display panel  ***********
    [Header("Select Group Bar")]
    [SerializeField]
    private GameObject[] selectGroupArray;

    // **********  Bottom Right of the Screen  ***********
    [Header("Right_Command Panel")]
    [SerializeField]
    private GameObject commandPanel;
    [SerializeField]
    private Image[] commandArray;

    // **********  Top Right of the Screen  ***********
    [Header("Resource Panel")]
    [SerializeField]
    private TMPro.TextMeshProUGUI ResourceOneText;
    [SerializeField]
    private TMPro.TextMeshProUGUI ResourceTwoText;
    [SerializeField]
    private TMPro.TextMeshProUGUI ResourceThreeText;



    public enum Panels
    {
        Minimap,
        Resource,
        StructureEntity,
        UnitEntity,
        Group,
        Production,
        Command,
        SelectGroup
    }

    private List<Panels> currentPanels;


    private void Awake()
    {
        currentPanels = new List<Panels>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }


    /// <summary>
    /// English: Update the Entity Panel with information from the entity parameter.
    /// 日本語：entityパラメータからの情報で、Entityパネルを更新する。
    /// </summary>
    /// <param name="_entity"></param>
    public void UpdateEntityPanel(IEntity _entity)
    {
        if(_entity == null) { return; }

        curUnitPortrait.sprite = _entity.GetPortrait();
        curUnitNameText.text = _entity.GetTransform().name;

        if (_entity is IUnit unit)
        {
            curUnitHealthText.text = string.Format("{0}/{1}", unit.GetCurrentHealth(), unit.GetSetHealth());
            curUnitDefenseImage.sprite = null;
            curUnitAttackImage.sprite = null;
        }
        else if(_entity is IStructure structure)
        {
            curStructureHealthText.text = string.Format("{0}/{1}", structure.GetCurrentHealth(), structure.GetSetHealth());
            curStructureDefenseImage.sprite = null;
            curStructureAttackImage.sprite = null;
        }
    }

    /// <summary>
    /// English: Update the Current Group Panel with information from the a list of entities parameter.
    /// 日本語：Entityのリストのパラメータからの情報で、現在使用しているグループのパネルを更新する。
    /// </summary>
    /// <param name="_curGroupList"></param>
    public void UpdateCurrentGroupPanel(List<IEntity> _curGroupList)
    {
        if(_curGroupList == null) { return; }
        if(_curGroupList.Count == 0) { return; }

        int subGroupCount = curGroupDisplayPanel.transform.childCount;
        int eachGroupEntityCount = curGroupDisplayPanel.transform.GetChild(0).transform.childCount;

        float count = _curGroupList.Count / eachGroupEntityCount;
        int buttonIndex = 0;
        for (int groupIndex = 0; groupIndex < count; groupIndex++)
        {
            if (buttonIndex > _curGroupList.Count) { return; }
            for (int index = buttonIndex; index < eachGroupEntityCount; index++)
            {
                if(buttonIndex > _curGroupList.Count) { return; }
                Button button = curGroupDisplayPanel.transform.GetChild(groupIndex).transform.GetChild(index).GetComponent<Button>();
                EntityButton entityButton = curGroupDisplayPanel.transform.GetChild(groupIndex).transform.GetChild(index).GetComponent<EntityButton>();
                entityButton.entity = _curGroupList[index];
                button.image.sprite = _curGroupList[index].GetPortrait();
                button.onClick.AddListener(() => { entityButton.OnClick(); });
                buttonIndex++;
            }

        }
    }

    /// <summary>
    /// English: Update the current entity's Production Panel with information from the entity parameter.
    /// 日本語：entityのパラメータからの情報で、現在選んでいるEntityのプロダクション（進捗など）のパネルを更新する。
    /// </summary>
    /// <param name="_entity"></param>
    public void UpdateProductionPanel(IEntity _entity)
    {
        if(_entity == null) { return; }
        producerPortrait.sprite = _entity.GetPortrait();

        if(_entity is IUnit unit)
        {
            producerHealthText.text = string.Format("{0}/{1}", unit.GetCurrentHealth(), unit.GetSetHealth());
        }
        else if(_entity is IStructure structure)
        {
            producerHealthText.text = string.Format("{0}/{1}", structure.GetCurrentHealth(), structure.GetSetHealth());
        }

        if (_entity.GetSelf() is IProduce produce)
        {
            progressBar.produce = produce;
        }
        else
        {
            UpdateEntityPanel(_entity);
        }

        
    }

    public void UpdateResourcePanel()
    {

    }





}

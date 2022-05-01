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
    private SelectManager selectSystem;

    // *********** Bottom Left of the Screen  ************
    [Header("Left_Minimap")]
    [SerializeField]
    private GameObject miniMap;
    [SerializeField]
    private Texture miniMapTexture;

    // **********  Bottom Middle of the Screen  ***********
    [Header("Middle_Panel")]
    [SerializeField]
    private GameObject middlePanel;

    [Header("Current Entity")]
    [SerializeField]
    private GameObject entityDisplayPanel;
    [SerializeField]
    private EntityButton curEntityButton;
    [SerializeField]
    private TMPro.TextMeshProUGUI curEntityHealthText;
    [SerializeField]
    private TMPro.TextMeshProUGUI curEntityNameText;

    [SerializeField]
    private GameObject curEntityStats;
    [SerializeField]
    private HoverInfo curUnitDefenseImage;
    [SerializeField]
    private HoverInfo curUnitAttackImage;

    [SerializeField]
    private GameObject curEntityProduction;
    [SerializeField]
    private ProgressBar progressBar;

    [Header("Current Select Group")]
    [SerializeField]
    private GameObject groupDisplayPanel;

    // **********  On top of Middle Panel  ***********
    [Header("Select Group Bar")]
    [SerializeField]
    private GameObject[] selectGroupList;

    // **********  Bottom Right of the Screen  ***********
    [Header("Right_Command Panel")]
    [SerializeField]
    private GameObject commandPanel;
    [SerializeField]
    private IButton[] commandArray;

    // **********  Top Right of the Screen  ***********
    [Header("Resource Panel")]
    [SerializeField]
    private TMPro.TextMeshProUGUI ResourceOneText;
    [SerializeField]
    private TMPro.TextMeshProUGUI ResourceTwoText;
    [SerializeField]
    private TMPro.TextMeshProUGUI ResourceThreeText;


    private void OnEnable()
    {
        EventManager.OnSelectGroupAdded += UpdateSelectGroupPanel;
        EventManager.OnCurrentGroupSelected += UpdateMiddlePanel;
        EventManager.OnCurrentGroupCleared += ClearMiddlePanel;
        EventManager.OnCurrentGroupSelected += UpdateCommandPanel;
        EventManager.OnCurrentGroupCleared += ClearCommandPanel;
    }

    private void OnDisable()
    {
        EventManager.OnSelectGroupAdded -= UpdateSelectGroupPanel;
        EventManager.OnCurrentGroupSelected -= UpdateMiddlePanel;
        EventManager.OnCurrentGroupCleared -= ClearMiddlePanel;
        EventManager.OnCurrentGroupSelected -= UpdateCommandPanel;
        EventManager.OnCurrentGroupCleared -= ClearCommandPanel;
    }

    private void Awake()
    {

    }

    private void Start()
    {
        miniMap.SetActive(true);
        middlePanel.SetActive(true);

        entityDisplayPanel.SetActive(false);
        curEntityStats.SetActive(false );
        curEntityProduction.SetActive(false );

        groupDisplayPanel.SetActive(false);

        for(int index = 0; index < selectGroupList.Length; index++)
        {
            selectGroupList[index].SetActive(false);
        }

        commandPanel.SetActive(true);

    }

    private void Update()
    {
        UpdateResourcePanel();
        UpdateCurrentEntityHealth();
        
    }

    public void UpdateMiddlePanel(List<IEntity> _curGroupList)
    {
        if(_curGroupList == null) { return; }
        if(_curGroupList.Count == 0) { return; }

        entityDisplayPanel.SetActive(false);
        groupDisplayPanel.SetActive(false);

        if (_curGroupList.Count == 1)
        {
            UpdateEntityPanel(_curGroupList[0]);
            entityDisplayPanel.SetActive(true);
        }
        else
        {
            UpdateCurrentGroupPanel(_curGroupList);
            groupDisplayPanel.SetActive(true);
        }

    }

    public void ClearMiddlePanel()
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

        curEntityButton.AssignEntity(_entity);
        curEntityNameText.text = _entity.GetName();

        // Check if Entity has progress-based action or not
        if(_entity.ReturnActionType() is IOperating)
        {
            UpdateProduction(_entity);
        }
        else
        {
            UpdateStats(_entity);
        }

    }


    public void UpdateCurrentEntityHealth()
    {
        if (curEntityButton.entity != null)
        {
            if (curEntityButton.entity is IUnit unit)
            {
                curEntityHealthText.text = string.Format("{0}/{1}", unit.GetCurrentHealth(), unit.GetSetHealth());

            }
            else if (curEntityButton.entity is IUnit structure)
            {
                curEntityHealthText.text = string.Format("{0}/{1}", structure.GetCurrentHealth(), structure.GetSetHealth());

            }
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

        for (int groupIndex = 0; groupIndex < 4; groupIndex++)
        {
            for (int index = 0; index < 30; index++)
            {
                groupDisplayPanel.transform.GetChild(groupIndex).transform.GetChild(1).transform.GetChild(index).gameObject.SetActive(false);
            }
        }

        int entityIndex = 0;
        for (int groupIndex = 0; groupIndex < 4; groupIndex++)
        {
            for (int index = 0; index < 30; index++)
            {
                if(entityIndex > _curGroupList.Count - 1) { break; }
                EntityButton entityButton = groupDisplayPanel.transform.GetChild(groupIndex).transform.GetChild(1).transform.GetChild(index).GetComponent<EntityButton>();
                groupDisplayPanel.transform.GetChild(groupIndex).transform.GetChild(1).transform.GetChild(index).gameObject.SetActive(true);
                entityButton.AssignEntity(_curGroupList[entityIndex]);
                entityIndex++;
            }
        }

    }

    public void ClearCurrentGroupPanel()
    {
        for (int groupIndex = 0; groupIndex < 4; groupIndex++)
        {
            for (int index = 0; index < 30; index++)
            {
                groupDisplayPanel.transform.GetChild(groupIndex).transform.GetChild(1).transform.GetChild(index).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// English: Update the current entity's Production Panel with information from the entity parameter.
    /// 日本語：entityのパラメータからの情報で、現在選んでいるEntityのプロダクション（進捗など）のパネルを更新する。
    /// </summary>
    /// <param name="_entity"></param>
    public void UpdateProduction(IEntity _entity)
    {
        if(_entity == null) { return; }

    }

    public void UpdateStats(IEntity _entity)
    {

    }

    public void UpdateSelectGroupPanel(int index)
    {

    }

    public void UpdateCommandPanel(List<IEntity> _curGroupList)
    {
        if (_curGroupList == null) { return; }
        if (_curGroupList.Count == 0) { return; }

        if (_curGroupList.Count == 1)
        {
        }
        else
        {
        }
    }

    public void ClearCommandPanel()
    {

    }
    public void UpdateResourcePanel()
    {

    }





}

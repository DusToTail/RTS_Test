using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private SelectSystem selectSystem;

    [SerializeField]
    private GameObject miniMap;
    [SerializeField]
    private Texture miniMapTexture;

    [SerializeField]
    private GameObject displayPanel;
    [SerializeField]
    private GameObject curGroupDisplayPanel;

    [SerializeField]
    private GameObject curStructureDisplayPanel;
    [SerializeField]
    private Sprite curStructurePortrait;
    [SerializeField]
    private TextMesh curStructureHealthText;
    [SerializeField]
    private TextMesh curStructureNameText;
    [SerializeField]
    private Sprite curStructureDefenseImage;
    [SerializeField]
    private Sprite curStructureAttackImage;

    [SerializeField]
    private GameObject curUnitDisplayPanel;
    [SerializeField]
    private Sprite curUnitPortrait;
    [SerializeField]
    private TextMesh curUnitHealthText;
    [SerializeField]
    private TextMesh curUnitNameText;
    [SerializeField]
    private Sprite curUnitDefenseImage;
    [SerializeField]
    private Sprite curUnitAttackImage;

    [SerializeField]
    private GameObject producerDisplayPanel;
    [SerializeField]
    private Sprite producerPortrait;
    [SerializeField]
    private TextMesh producerHealthText;
    [SerializeField]
    private TextMesh producerNameText;
    [SerializeField]
    private Slider progressFill;
    [SerializeField]
    private Sprite[] productionEntityImageArray;

    [SerializeField]
    private GameObject[] selectGroupArray;

    [SerializeField]
    private GameObject commandPanel;
    [SerializeField]
    private Sprite[] commandArray;


    private void Awake()
    {
        

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }







}

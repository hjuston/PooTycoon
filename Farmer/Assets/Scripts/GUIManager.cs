﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class GUIManager : MonoBehaviour
{
    void Start()
    {
        EditMode_InitializeStandardBuildingsButtons();
        EditMode_InitializePrestigeBuildingsButtons();
        EditMode_InitializeInfrastructureBuildingsButtons();
    }

    #region ** EditMode
    [Header("Edit mode")]

    #region EditMode Public properties
    public GameObject EditMode_MainPanel;
    public GameObject EditMode_MainPanelToggleButton;
    public Sprite EditMode_MainPanelToggleButtonActiveSprite;
    public Sprite EditMode_MainPanelToggleButtonNotActiveSprite;

    public GameObject EditMode_BuildingsPanel;
    public GameObject EditMode_BuildingPanelPrefab;

    public GameObject EditMode_GhostPositionGroupPanel;
    public GameObject EditMode_GhostFollowingGroupPanel;

    public GameObject EditMode_StandardBuildingsContent;
    public GameObject EditMode_InfrastructureBuildingsContent;
    public GameObject EditMode_PrestigeBuildingsContent;
    #endregion

    #region EditMode Private properties
    bool _editModeMainPanelVisible = false;
    #endregion

    #region EditMode Methods
    /// <summary>
    /// Metoda ukrywa/wyświetla główny panel editMode
    /// </summary>
    public void EditMode_ToggleMainPanel()
    {
        _editModeMainPanelVisible = !_editModeMainPanelVisible;

        // Ukrywane jest menu ulepszenia budynku
        if (_editModeMainPanelVisible == true)
        {
            BuildingMode_DisplayBuildingPanel(false);
        }

        Helper.GetGameManager().SetEditMode(_editModeMainPanelVisible);

        EditMode_MainPanelToggleButton.GetComponent<Image>().sprite = _editModeMainPanelVisible ? EditMode_MainPanelToggleButtonActiveSprite : EditMode_MainPanelToggleButtonNotActiveSprite;

        if (_editModeMainPanelVisible)
        {
            EditMode_ShowMainPanel();
        }
        else
        {
            EditMode_HideMainPanel();
            Helper.GetGridManager().CancelGhost();
        }
    }

    /// <summary>
    /// Metoda ukrywa/wyświetla panel z przyciskami do pozycjonowania budynku
    /// </summary>
    /// <param name="visible"></param>
    public void EditMode_SetGhostPositionGroupVisible(bool visible)
    {
        EditMode_GhostPositionGroupPanel.SetActive(visible);
    }

    /// <summary>
    /// Wyświetla główny panel edit mode
    /// </summary>
    public void EditMode_ShowMainPanel()
    {
        Animator animator = EditMode_MainPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsVisible", true);
        }
    }

    /// <summary>
    /// Ukrywa główny panel edit mode
    /// </summary>
    public void EditMode_HideMainPanel()
    {
        Animator animator = EditMode_MainPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsVisible", false);
        }
        EditMode_HideBuildingsPanel();
    }

    /// <summary>
    /// Wyświetlenie menu budynków
    /// </summary>
    public void EditMode_ShowBuildingsPanel()
    {
        Animator animator = EditMode_BuildingsPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsVisible", true);
        }
    }

    /// <summary>
    /// Ukrywanie menu budynków
    /// </summary>
    public void EditMode_HideBuildingsPanel()
    {
        Animator animator = EditMode_BuildingsPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsVisible", false);
        }
    }

    /// <summary>
    /// Metoda inicjalizuje panel Standardowych budowli - wypełnia go przyciskami
    /// </summary>
    public void EditMode_InitializeStandardBuildingsButtons()
    {
        GameObject[] standardBuildings = BuildingsDatabase.GetBuildingsByType(BuildingType.Standard);
        Dictionary<GameObject, Building> temp = new Dictionary<GameObject, Building>();

        // Pobieranie listy budynków
        foreach (GameObject building in standardBuildings)
        {
            Building buildingScript = building.GetComponent<Building>();

            if (buildingScript != null)
            {
                temp.Add(building, buildingScript);
            }
        }

        // Inicjalizowanie budynków
        foreach (KeyValuePair<GameObject, Building> building in temp.OrderBy(x => x.Value.iBaseCost))
        {
            // Tworznie obiektu przycisku i wrzucanie go do kontenera
            GameObject buildingInfoPanel = GameObject.Instantiate(EditMode_BuildingPanelPrefab);
            buildingInfoPanel.transform.SetParent(EditMode_StandardBuildingsContent.transform, false);

            EditMode_BuildingInfoPanelScript infoPanelScript = buildingInfoPanel.GetComponent<EditMode_BuildingInfoPanelScript>();
            if (infoPanelScript != null)
            {
                infoPanelScript.Initialize(building.Value, building.Key);
            }
        }
    }

    /// <summary>
    /// Metoda inicjalizuje panel Standardowych budowli - wypełnia go przyciskami
    /// </summary>
    public void EditMode_InitializeInfrastructureBuildingsButtons()
    {
        GameObject[] infrastructureBuildings = BuildingsDatabase.GetBuildingsByType(BuildingType.Infrastructure);
        Dictionary<GameObject, Building> temp = new Dictionary<GameObject, Building>();

        // Pobieranie listy budynków
        foreach (GameObject building in infrastructureBuildings)
        {
            Building buildingScript = building.GetComponent<Building>();

            if (buildingScript != null)
            {
                temp.Add(building, buildingScript);
            }
        }

        // Inicjalizowanie budynków
        foreach (KeyValuePair<GameObject, Building> building in temp.OrderBy(x => x.Value.iBaseCost))
        {
            // Tworznie obiektu przycisku i wrzucanie go do kontenera
            GameObject buildingInfoPanel = GameObject.Instantiate(EditMode_BuildingPanelPrefab);
            buildingInfoPanel.transform.SetParent(EditMode_InfrastructureBuildingsContent.transform, false);

            EditMode_BuildingInfoPanelScript infoPanelScript = buildingInfoPanel.GetComponent<EditMode_BuildingInfoPanelScript>();
            if (infoPanelScript != null)
            {
                infoPanelScript.Initialize(building.Value, building.Key);
            }
        }
    }

    /// <summary>
    /// Metoda inicjalizuje panel Standardowych budowli - wypełnia go przyciskami
    /// </summary>
    public void EditMode_InitializePrestigeBuildingsButtons()
    {
        GameObject[] prestigeBuildings = BuildingsDatabase.GetBuildingsByType(BuildingType.Prestige);
        Dictionary<GameObject, Building> temp = new Dictionary<GameObject, Building>();

        // Pobieranie listy budynków
        foreach (GameObject building in prestigeBuildings)
        {
            Building buildingScript = building.GetComponent<Building>();

            if (buildingScript != null)
            {
                temp.Add(building, buildingScript);
            }
        }

        // Inicjalizowanie budynków
        foreach (KeyValuePair<GameObject, Building> building in temp.OrderBy(x => x.Value.iBaseCost))
        {
            // Tworznie obiektu przycisku i wrzucanie go do kontenera
            GameObject buildingInfoPanel = GameObject.Instantiate(EditMode_BuildingPanelPrefab);
            buildingInfoPanel.transform.SetParent(EditMode_PrestigeBuildingsContent.transform, false);

            EditMode_BuildingInfoPanelScript infoPanelScript = buildingInfoPanel.GetComponent<EditMode_BuildingInfoPanelScript>();
            if (infoPanelScript != null)
            {
                infoPanelScript.Initialize(building.Value, building.Key);
            }
        }
    }
    #endregion
    #endregion

    #region ** GameStats
    [Header("Game stats")]

    #region GameStats Public properties
    public GameObject GameStats_ExperienceBar;

    public Text GameStats_LevelText;
    public Text GameStats_ExperienceText;

    public Text GameStats_MoneyText;
    public Text GameStats_IncomeText;

    public Text GameStats_PeopleCountText;
    #endregion

    #region GameStats Methods
    /// <summary>
    /// Metoda zmienia rozmiar paska doświadczenia
    /// </summary>
    /// <param name="percentage"></param>
    public void GameStats_SetExperienceBarValue(float percentage)
    {
        GameStats_ExperienceBar.transform.localScale = new Vector3(percentage, GameStats_ExperienceBar.transform.localScale.y, GameStats_ExperienceBar.transform.localScale.z);
    }

    /// <summary>
    /// Metoda zmiania wyświetlany poziom w labelu
    /// </summary>
    /// <param name="level"></param>
    public void GameStats_SetLevelText(int level)
    {
        GameStats_LevelText.text = level.ToString();
    }

    /// <summary>
    /// Metoda wyświetla informacje o doświadczeniu
    /// </summary>
    /// <param name="current"></param>
    /// <param name="required"></param>
    public void GameStats_SetExperienceValue(int current, int required)
    {
        GameStats_ExperienceText.text = string.Format("{0}/{1}", Helper.GetDisplayableValue(current), Helper.GetDisplayableValue(required));
    }

    /// <summary>
    /// Metoda wyświetla informacje o gotówce generowanej w ciągu jednej sekundy.
    /// </summary>
    /// <param name="money"></param>
    public void GameStats_SetIncomeInfo(BigInteger money)
    {
        GameStats_IncomeText.text = Helper.GetDisplayableValue(money);
    }

    /// <summary>
    /// Metoda ustawia informacje o aktualnej gotówce.
    /// </summary>
    /// <param name="building"></param>
    public void GameStats_SetCurrentMoneyInfo(BigInteger money)
    {
        GameStats_MoneyText.text = Helper.GetDisplayableValue(money);
    }
    #endregion
    #endregion

    #region ** BuildingMode
    [Header("BuildingMode")]

    #region BuildingMode Public properties
    public GameObject BuildingMode_BuildingPanel;
    public Button BuildingMode_UpgradeButtonPrefab;
    public GameObject BuildingMode_UpgradesPanel;

    public Text BuildingMode_BuildingNameText;
    public Text BuildingMode_BuildingIncomeText;
    public Sprite BuildingMode_UpgradeBuildingActiveSprite;
    public Sprite BuildingMode_UpgradeBuildingNotActiveSprite;
    public Text BuildingMode_UpgradeCostText;
    public Text BuildingMode_UpgradeNumberText;

    public Button BuildingMode_BuyUpgradeButton;
    public GameObject BuildingMode_CurrentBuildingMarker;

    public Button BuildingMode_x1MultiplierButton;
    public Button BuildingMode_x10MultiplierButton;
    public Button BuildingMode_x100MultiplierButton;

    public Sprite BuildingMode_MultiplierButtonActiveSprite;
    public Sprite BuildingMode_MutiplierButtonNotActiveSprite;

    public GameObject BuildingMode_BuildingProgressBar;

    public Text BuildingMode_TimeToCollectShitText;
    #endregion

    #region BuildingMode Methods
    /// <summary>
    /// Metoda ustawia wartość tekstu informującego o czasie pozostałym do 'zebrania' shitu przez oczyszczalnię
    /// </summary>
    /// <param name="timeLeft"></param>
    internal void BuildingMode_TimeToCollectShitUpdate(float timeLeft)
    {
        BuildingMode_TimeToCollectShitText.text = string.Format("{0:0.##}", timeLeft);
    }

    /// <summary>
    /// Metoda wywoływana po kliknięciu na budynek w trybie Building
    /// </summary>
    /// <param name="gameObject"></param>
    internal void BuildingMode_SelectBuilding(GameObject gameObject)
    {
        Building buildingScript = gameObject.GetComponent<Building>();
        if (buildingScript != null)
        {
            // Wyświetlanie panelu informacji o budynku
            BuildingMode_SetBuildingInfo(buildingScript);

            // Wyświetlanie markera
            BuildingMode_CurrentBuildingMarkerShow(gameObject);

            // Wyświetlanie paska postępu
            BuildingProgressBar progressBarScript = BuildingMode_BuildingProgressBar.GetComponent<BuildingProgressBar>();
            if(progressBarScript != null)
            {
                progressBarScript.SetFollowedBuilding(gameObject);
            }
            BuildingMode_BuildingProgressBar.SetActive(true);
        }
    }

    /// <summary>
    /// Metoda ustawia informacje o budynku w panelu informacji.
    /// </summary>
    /// <param name="building"></param>
    public void BuildingMode_SetBuildingInfo(Building building)
    {
        if (building == null)
        {
            BuildingMode_DisplayBuildingPanel(false);
        }
        else
        {
            // Wyświetlanie nazwy i poziomu budynku
            BuildingMode_BuildingNameText.text = building.Name;
            BuildingMode_UpgradeNumberText.text = building.BuildingLevel.ToString();

            // Wyświetlanie informacji o koszcie i przychodzie budynku
            BuildingMode_BuildingIncomeText.text = Helper.GetDisplayableValue(building.GetIncome());
            BuildingMode_UpgradeCostText.text = Helper.GetDisplayableValue(building.GetCost());

            BuildingMode_InitializeBuildingUpgradeButtons(building);

            // Wyświetlanie informacji o koszcie na podstawie wybranej opcji multipliera (x1, x10,  x100)
            BuildingMode_UpdateBuildingLevelCostInfo(building, Helper.GetGameManager().GetUpgradeByValue());

            Helper.GetGameManager().SetCurrentlySelectedBuilding(building);
            BuildingMode_DisplayBuildingPanel(true);
        }
    }

    /// <summary>
    /// Metoda wyświetla przyciski z ulepszeniami budynku
    /// </summary>
    /// <param name="building"></param>
    void BuildingMode_InitializeBuildingUpgradeButtons(Building building)
    {
        // Czyszczenie panelu
        foreach (Transform child in BuildingMode_UpgradesPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Wczytywanie ulepszeń
        if (building != null && building.Upgrades.Any())
        {
            foreach (Upgrade upgrade in building.Upgrades)
            {
                Button btn = Button.Instantiate(BuildingMode_UpgradeButtonPrefab);

                btn.GetComponentInChildren<Image>().sprite = upgrade.Image;

                BuildingUpgradeButton upgradeButtonScript = btn.GetComponent<BuildingUpgradeButton>();
                if (upgradeButtonScript != null)
                {
                    upgradeButtonScript.InitializeButton(upgrade);
                }

                btn.transform.SetParent(BuildingMode_UpgradesPanel.transform, false);
            }
        }
    }

    /// <summary>
    /// Metoda ukrywa/pokazuje menu z upgradeami budynku
    /// </summary>
    public void BuildingMode_DisplayBuildingPanel(bool visible)
    {
        Animator animator = BuildingMode_BuildingPanel.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsOpen", visible);
            if (!visible)
            {
                // Ukrywanie markera wskazującego budynek
                BuildingMode_CurrentBuildingMarkerHide();

                // Ukrywanie paska postepu budynku
                BuildingMode_BuildingProgressBar.SetActive(false);
            }
            else
            {
                BuildingMode_SetMultiplierButtonActive();
            }
        }
    }

    public void BuildingMode_SetMultiplierButtonActive()
    {
        int upgradeby = Helper.GetGameManager().GetUpgradeByValue();

        BuildingMode_x1MultiplierButton.GetComponent<Image>().sprite = BuildingMode_MutiplierButtonNotActiveSprite;
        BuildingMode_x10MultiplierButton.GetComponent<Image>().sprite = BuildingMode_MutiplierButtonNotActiveSprite;
        BuildingMode_x100MultiplierButton.GetComponent<Image>().sprite = BuildingMode_MutiplierButtonNotActiveSprite;

        switch (upgradeby)
        {
            case 1:
                BuildingMode_x1MultiplierButton.GetComponent<Image>().sprite = BuildingMode_MultiplierButtonActiveSprite;
                break;
            case 10:
                BuildingMode_x10MultiplierButton.GetComponent<Image>().sprite = BuildingMode_MultiplierButtonActiveSprite;
                break;
            case 100:
                BuildingMode_x100MultiplierButton.GetComponent<Image>().sprite = BuildingMode_MultiplierButtonActiveSprite;
                break;
        }
    }

    /// <summary>
    /// Metoda wyświetla informacje o aktualnym koszcie ulepszenia budynku na podstawie ilości leveli
    /// </summary>
    /// <param name="building"></param>
    /// <param name="levels"></param>
    public void BuildingMode_UpdateBuildingLevelCostInfo(Building building, int levels)
    {
        if (building == null)
        {
            BuildingMode_DisplayBuildingPanel(false);
        }
        else
        {
            BigInteger value = building.CalculateCostForNextXLevels(levels);
            BigInteger currentMoney = GameStats.Instance.GetCurrentMoney();

            BuildingMode_UpgradeCostText.text = Helper.GetDisplayableValue(value);

            if (currentMoney >= value)
            {
                SetButtonEnabled(true, BuildingMode_BuyUpgradeButton, BuildingMode_UpgradeBuildingActiveSprite, Color.white, "upgrade");
            }
            else
            {
                SetButtonEnabled(true, BuildingMode_BuyUpgradeButton, BuildingMode_UpgradeBuildingNotActiveSprite, Color.black, "not enough cash");
            }
        }
    }

    /// <summary>
    /// Metoda wyświetla marker
    /// </summary>
    public void BuildingMode_CurrentBuildingMarkerShow(GameObject followedBuilding)
    {
        BuildingMode_CurrentBuildingMarker.SetActive(true);
        SelectedBuildingMarker markerScript = BuildingMode_CurrentBuildingMarker.GetComponent<SelectedBuildingMarker>();
        if (markerScript != null)
        {
            markerScript.SetFollowedBuilding(followedBuilding);
        }
    }

    /// <summary>
    /// Metoda ukrywa marker
    /// </summary>
    public void BuildingMode_CurrentBuildingMarkerHide()
    {
        SelectedBuildingMarker markerScript = BuildingMode_CurrentBuildingMarker.GetComponent<SelectedBuildingMarker>();
        if (markerScript != null)
        {
            markerScript.UnsetFollowedBuilding();
        }
        BuildingMode_CurrentBuildingMarker.SetActive(false);
    }

    /// <summary>
    /// Metoda ustawia pozycję markera
    /// </summary>
    /// <param name="worldPosition"></param>
    public void BuildingMode_CurrentBuildingMarkerSetPosition(Vector3 worldPosition)
    {
        BuildingMode_CurrentBuildingMarker.transform.position = Camera.main.WorldToScreenPoint(worldPosition) + new Vector3(0, 25);
    }
    #endregion
    #endregion

    #region ** Options
    [Header("Options menu")]
    public GameObject OptionsMenu;

    public void ShowOptionsMenu()
    {
        OptionsMenu.SetActive(true);
    }

    public void HideOptionsMenu()
    {
        OptionsMenu.SetActive(false);
    }
    #endregion

    #region ** Other
    public void SetButtonEnabled(bool enabled, Button button, Sprite sprite, Color fontColor, string text)
    {
        button.image.sprite = sprite;
        button.enabled = enabled;

        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = text;
            buttonText.color = fontColor;
        }
    }
    #endregion
}

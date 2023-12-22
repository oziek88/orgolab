using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsPanel : MonoBehaviour {
    public delegate void OptionsPanelEventHandler(OptionsPanel optionsPanel, bool value);
    public static event OptionsPanelEventHandler onDeleteModeToggle;

    public SolutionController solutionController;
    public MouseManager mouseManager;
    public GameObject mainPanelBar;
    public GameObject addMoleculePanelBar;
    public Toggle deleteToggle;
    public Toggle heat;
    public TMP_Dropdown solvent;
    private void Start() {
        addMoleculePanelBar.SetActive(false);
        mainPanelBar.SetActive(true);
        SolutionController.Instance.GenerateCollidersAcrossScreen(this.GetComponent<RectTransform>().rect.height);

        SolutionController.onSolutionUpdated += UpdateInteractability;
    }

    private void UpdateInteractability(SolutionController sc) {
        heat.interactable = !sc.ionsInSolution;
        solvent.interactable = !sc.ionsInSolution;
        deleteToggle.interactable = !sc.ionsInSolution;
    }

    public void AddMoleculeClick() {
        mainPanelBar.SetActive(false);
        addMoleculePanelBar.SetActive(true);
    }

    public void CloseMoleculeSelectionBar() {
        addMoleculePanelBar.SetActive(false);
        mainPanelBar.SetActive(true);
    }

    public void DeleteMoleculeToggle() {
        onDeleteModeToggle?.Invoke(this, deleteToggle.isOn);
    }

    public void HintClick() {
        // call solution controller to freeze everything in solution
        ModalMenuController.Instance.CreateDialogPanel(SolutionController.Instance.GetHint());
    }

    public void SolventChange() {
        SolutionController.Instance.UpdateSolvent(solvent.options[solvent.value].text);
    }

    public void ToggleHeat() {
        SolutionController.Instance.UpdateHeat(heat.isOn);
    }

    public void AddAlkene() {
        Hydrocarbon alkene = solutionController.moleculePool.ChooseRandomAlkene();
        solutionController.Spawn(alkene);
    }

    public void AddAlkyne() {
        Hydrocarbon alkyne = solutionController.moleculePool.ChooseRandomAlkyne();
        solutionController.Spawn(alkyne);
    }

    public void AddHydrohalide() {
        Hydrohalide hX = solutionController.moleculePool.ChooseRandomHydrohalide();
        solutionController.Spawn(hX);
    }

    public void AddHydrogenBromide() {
        Hydrohalide hbr = solutionController.moleculePool.ChooseHydrogenBromide();
        solutionController.Spawn(hbr);
    }

    

    private void OnDestroy() {
        SolutionController.onSolutionUpdated -= UpdateInteractability;
    }
}

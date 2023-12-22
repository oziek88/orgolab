using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalMenuController : MonoBehaviour {
    private static ModalMenuController _instance;
    public static ModalMenuController Instance { get { return _instance; } }

    public GameObject dialogPanelPrefab;

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.

        if(Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }
    public void CreateDialogPanel(string headerText = null, string dialogText = null, string button1Text = null, Action button1Callback = null, string button2Text = null, Action button2Callback = null) {
        GameObject dialogPanelGO = Instantiate(dialogPanelPrefab, transform);
        DialogPanel dialogPanel = dialogPanelGO.GetComponent<DialogPanel>();
        dialogPanel.Setup(headerText, dialogText, button1Text, button1Callback, button2Text, button2Callback);
    }

    public void ShowGuide() {

    }
}

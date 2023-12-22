using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DialogPanel : MonoBehaviour {
    public GameObject headerGO;
    public TMP_Text headerText;

    public TMP_Text dialogText;

    public Button button1;
    public TMP_Text button1Text;
    public Button button2;
    public TMP_Text button2Text;

    public void Setup(string dialogText, string headerText = null, string button1Text = null, Action button1Callback = null, string button2Text = null, Action button2Callback = null) {
        // Set header
        if(String.IsNullOrEmpty(headerText)) {
            headerGO.SetActive(false);
        } else {
            this.headerText.text = headerText;
            headerGO.SetActive(true);
        }

        // Set dialog
        this.dialogText.text = dialogText;

        // Set buttons
        if(String.IsNullOrEmpty(button1Text) && button1Callback == null) {
            this.button1Text.text = "Close";
            button1.onClick.AddListener(delegate { Close(); });
        } else {
            this.button1Text.text = button1Text;
            button1.onClick.AddListener(delegate { button1Callback?.Invoke(); });
        }

        if(String.IsNullOrEmpty(button2Text) || button2Callback == null) {
            button2.gameObject.SetActive(false);
        } else {
            this.button2Text.text = button2Text;
            button2.onClick.AddListener(delegate { button2Callback?.Invoke(); });
            button2.gameObject.SetActive(false);
        }

    }

    public void Close() {
        Destroy(this.gameObject);
    }
}

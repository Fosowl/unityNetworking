using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class doorLogic : MonoBehaviour
{
    public TMP_Text textBox;
    public bool requireCode;
    public GameObject codePanel;
    public TMP_InputField codeInputField;
    public Button codeSubmitButton;
    public string code;
    // private
    bool onDoor = false;
    bool checkingCode = false;
    bool isOpen = false;

    void Start()
    {
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag != "Player") {
            return;
        }
        textBox.text = "[ Press 'E' to open the door ]";
        onDoor = true;
    }

    void OnTriggerExit(Collider other) {
        if (other.tag != "Player") {
            return;
        }
        textBox.text = "";
        onDoor = false;
        checkingCode = false;
        codePanel.SetActive(false);
    }

    void LateUpdate() {
        if (onDoor && Input.GetKeyDown("e")) {
            if (requireCode && !isOpen) {
                codePanel.SetActive(true);
                checkingCode = true;
            } else {
                pivotDoor();
                checkingCode = false;
            }
        }
        if (checkingCode) {
            checkCode();
        }
    }

    void checkCode() {
        if (codeInputField.text == code) {
            Debug.Log("Code accepted");
            textBox.text = "";
            codePanel.SetActive(false);
            codeInputField.text = "";
            pivotDoor();
            Debug.Log("Door opened");
        } else {
            Debug.Log("Code rejected");
            return;
        }
    }

    void pivotDoor() {
        if (isOpen) {
            gameObject.transform.Rotate(0, 270, 0);
            isOpen = false;
        } else {
            gameObject.transform.Rotate(0, 90, 0);
            isOpen = true;
        }
    }
}

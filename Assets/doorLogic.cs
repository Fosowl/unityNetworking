using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class doorLogic : MonoBehaviour
{
    public TMP_Text textBox;
    public bool requireCode = true;
    public GameObject codePanel;
    public TMP_InputField codeInputField;
    public Button codeSubmitButton;
    public string code = "1234";
    // private
    bool onDoor = false;
    bool checkingCode = false;

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
        gameObject.transform.Rotate(0, 0, 0);
        checkingCode = false;
        codePanel.SetActive(false);
    }

    void LateUpdate() {
        if (onDoor && Input.GetKeyDown("e")) {
            if (requireCode) {
                codePanel.SetActive(true);
                checkingCode = true;
            } else {
                gameObject.transform.Rotate(0, 70, 0);
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
            gameObject.transform.Rotate(0, 70, 0);
            Debug.Log("Door opened");
        } else {
            Debug.Log("Code rejected");
            return;
        }
    }
}

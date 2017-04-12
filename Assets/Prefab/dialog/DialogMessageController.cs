using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMessageController : MonoBehaviour {
	public enum DIALOG_TYPE {
		DIALOG_TYPE_ONEBUTTON,
		DIALOG_TYPE_TWOBUTTON
	};

	public GameObject oneBtnPanel;
	public GameObject twoBtnPanel;
	public GameObject mainPanel;

    public Text txtMessage;
    public Text buttonOkText;

	public Text buttonOneText;
	public Text buttonTwoText;

	public System.Action OnButtonOkClickDelegate;
	public System.Action OnButtonOneClickDelegate;
	public System.Action OnButtonTwoClickDelegate;

	public DIALOG_TYPE _dialogType;
    // Use this for initialization
    void Start () {
		
	}

	public void setDialogType(DIALOG_TYPE type) {
		_dialogType = type;

		if (_dialogType == DIALOG_TYPE.DIALOG_TYPE_ONEBUTTON) {
			oneBtnPanel.SetActive(true);
			oneBtnPanel.SetActive(false);

		} else {
			oneBtnPanel.SetActive(false);
			oneBtnPanel.SetActive(true);
		}
	}

	public void Show() {
		mainPanel.SetActive(true);
	}

	public void Hide() {
		mainPanel.SetActive(false);
	}

    public void setButtonOkText(string text) {
		buttonOkText.text = text;
    }

	public void setButtonOneText(string text) {
		buttonOneText.text = text;
	}

	public void setButtonTwoText(string text) {
		buttonTwoText.text = text;
	}

    public void setMessage(string text) {
        txtMessage.text = text;
    }

    public void onButtonOkClickHandle() {
		OnButtonOkClickDelegate();
    }

	public void onButtonOneClickHandle() {
		OnButtonOneClickDelegate();
	}

	public void onButtonTwoClickHandle() {
		OnButtonTwoClickDelegate();
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuHeader : MonoBehaviour {

    public GameObject ButtonBack;
	void Start () {

        Button btnBack = ButtonBack.GetComponent<Button>();

        btnBack.onClick.AddListener(BackClick);
	}
	
	// Update is called once per frame
	void BackClick () {
        SceneManager.LoadScene("Home");
    }
}

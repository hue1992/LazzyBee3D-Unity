using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileController : MonoBehaviour, IScenesController
{

    public void OnBackButtonClick()
    {
        SceneManager.LoadScene("Home", LoadSceneMode.Additive);
    }

    public void OnLogOutButtonClick()
    {
        FirebaseHelper.getInstance().signOut();
        SceneManager.LoadScene("Login");
    }
    public void OnLinkToFacebookButtonClick()
    {
        //link to facbook
    }
}

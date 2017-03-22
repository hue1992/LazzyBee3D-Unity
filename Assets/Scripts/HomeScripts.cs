using UnityEngine;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Facebook.Unity;
using System.Collections.Generic;
using System.Collections;


public class HomeScripts : MonoBehaviour
{


    public Button btnStudy;
    
    public Button btnHome;
    public Button btnDictionary;
    public Button btnGame;
    public Button btnProfile;

    public GameObject loginPanel;

    DatabaseReference mReference;
    FirebaseDatabase mDatabase;
    Firebase.Auth.FirebaseAuth mAuth;

    private string accessToken = null;
    private string idToken;

    string APP_NAME = "LazzyBee3D";

    void Start()
    {

        initFirebase();

        initPLayGame();

        initFacebook();

        initButton();

        //get Setting user
        getUserSetting();

        testFilterDatabase();

    }

    private void initPLayGame()
    {
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.DebugLogEnabled = true;
        //PlayGamesPlatform.Activate();
    }

    private void initFacebook()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void initButton()
    {
        //Button btn = yourButton.GetComponent<Button>();
       // Button btn_Study = btnStudy.GetComponent<Button>();

        Button btn_Home = btnHome.GetComponent<Button>();
        Button btn_Dictionary = btnDictionary.GetComponent<Button>();
        Button btn_Game = btnGame.GetComponent<Button>();
        //Button btn_Profile = btnProfile.GetComponent<Button>();


        //btn_Study.onClick.AddListener(StudyClick);
    
        btn_Home.onClick.AddListener(HomeClick);

        btn_Dictionary.onClick.AddListener(DictionaryClick);

        btn_Game.onClick.AddListener(GameClick);

       // btn_Profile.onClick.AddListener(ProfileClick);
    }

    public void ProfileClick()
    {
        if (loginPanel.activeInHierarchy == false)
        {
            loginPanel.SetActive(true);
        }
    }



    private void GameClick()
    {
        singOut();
    }

    private void DictionaryClick()
    {
        // LoginWithGoogle();
        SceneManager.LoadScene("Dictionary");
    }

    private void HomeClick()
    {
       // logiWithFacebook();
    }

    private void initFirebase()
    {
        Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://lazzybee3d-99751721.firebaseio.com/");

        mReference = FirebaseDatabase.DefaultInstance.RootReference;
        mDatabase = FirebaseDatabase.DefaultInstance;

        mAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    private void testFilterDatabase()
    {
        string _ref = Shared.PUBLIC + Shared.DICTIONARY;
        Debug.Log("get user setting ref=" + _ref);

        //mDatabase.GetReference(_ref)
        //    .EqualTo("categories", "0")
        //.getvalueasync().continuewith(task =>
        //{
        //    if (task.iscompleted)
        //    {
        //        datasnapshot mydata = task.result;
        //        if (mydata.exists)
        //        {
        //            debug.log("results:" + task.result.getrawjsonvalue());
        //            //setting mysetting = (setting)mydata.getvalue(true);
        //            //debug.log("tostring:" + mysetting.tostring());

        //        }
        //        else
        //        {
        //            debug.log("empty data");
        //            // initdefaultsetting(_ref);
        //        }

        //    }
        //    else
        //    {
        //        debug.log("get my setting error");
        //    }
        //});

       // .ValueChanged += HandleValueChanged;



    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Error:" + args.DatabaseError.Message);
            return;
        }
        Debug.Log("Results=" + args.Snapshot.GetRawJsonValue());
    }

    private void getUserSetting()
    {
        string _ref = Shared.MySettingRef("1000");
        Debug.Log("get user setting ref=" + _ref);
        mDatabase.GetReference(_ref).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot myData = task.Result;
                if (myData.Exists)
                {
                    Debug.Log("results:" + task.Result.GetRawJsonValue());
                    Setting mySetting = (Setting)myData.GetValue(true);
                    Debug.Log("toString:" + mySetting.ToString());

                }
                else
                {
                    Debug.Log("Empty data");
                    initDefaultSetting(_ref);
                }

            }
            else
            {
                Debug.Log("get my setting error");
            }
        });
    }

    private void initDefaultSetting(string _ref)
    {
        //Init default setting                    
        Setting defaultSettiing = Setting.DefaultSetting();
        string jsondefaultSettiing = JsonUtility.ToJson(defaultSettiing);

        mReference.Child(_ref).SetRawJsonValueAsync(jsondefaultSettiing);
    }

   public void StudyClick()
    {
        SceneManager.LoadScene("Study");
        //Check status study per day 
      //  checkStatusLearn();

    }


    private void checkStatusLearn()
    {
        string refMyLearn = Shared.PRIVATE + Shared.USER + "1000/" + Shared.REVIEW;
        Debug.Log("check status learn:" + refMyLearn);
        mDatabase.GetReference(refMyLearn).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {

                if (task.Result.Exists)
                {
                    //get Review word
                }
                else
                {
                    Debug.Log("Empty review word");
                    //Init new word

                }

                //Load study scene
                SceneManager.LoadScene("Study");
            }
        });
    }


  

    private void FacebookAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            // Print current access token's User ID
            //DebugOnScreen.Log(aToken.UserId);
            // Print current access token's granted permissions
            //foreach (string perm in aToken.Permissions)
            //{
            //    DebugOnScreen.Log(perm);
            //}

            // Print current access token'
            DebugOnScreen.Log(Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);

            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(Facebook.Unity.AccessToken.CurrentAccessToken.TokenString);
            mAuth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    DebugOnScreen.Log("SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    DebugOnScreen.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                DebugOnScreen.Log("User signed in successfully: " + newUser.DisplayName + " (," + newUser.UserId + ")");
            });

        }
        else
        {
            DebugOnScreen.Log("User cancelled login");
        }
    }


    private void logiWithFacebook()
    {
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, FacebookAuthCallback);
    }

    private void LoginWithGoogle()
    {
        Social.localUser.Authenticate(GoogleLoginCallback);
    }

    void GoogleLoginCallback(bool success)
    {
        if (success)
        {
            Debug.LogFormat(APP_NAME + " :{0}", "Login success");
            if (idToken == null)
                //PlayGamesPlatform.Instance.GetIdToken(GetIdTokenCallback);
            if (accessToken == null)
                //watting 5 second
                StartCoroutine(wattingForGetAccessToken());
        }
        else
        {
            Debug.LogError("Google login failed. If you are not running in an actual Android/iOS device, this is expected.");
        }
    }

    private void GetIdTokenCallback(string token)
    {
        if (token != null)
        {
            idToken = token;
            //Debug.LogFormat(APP_NAME + " : getIdToken :{0}", token);
            Debug.LogFormat(APP_NAME + " : IdToken :{0}", idToken);
        }



    }

    IEnumerator wattingForGetAccessToken()
    {
        Debug.LogFormat(APP_NAME + " : Watting 5 second return accessToken ");
        yield return new WaitForSeconds(5);
        //getAccesstoken
        //Social.localUser.Authenticate(GetAccessToken);
        //accessToken = PlayGamesPlatform.Instance.GetToken();
        if (accessToken != null)
        {
            Debug.LogFormat(APP_NAME + " : accessToken :{0}", accessToken);
            linkGoogleToFireBase(idToken, accessToken);
        }
        else
        {
            Debug.LogFormat(APP_NAME + " : Get AccessToken is Null.Reclick button login!");
            LoginWithGoogle();
        }

    }

    private void linkGoogleToFireBase(string iDtoken, string accessToken)
    {

        if (idToken == null)
        {
            Debug.LogErrorFormat(APP_NAME + " : idToken is Null");
            return;
        }
        else if (accessToken == null)
        {
            Debug.LogErrorFormat(APP_NAME + " : accessToken is Null");
            return;
        }
        else
        {
            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(iDtoken, accessToken);
            mAuth.SignInWithCredentialAsync(credential).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogFormat(APP_NAME + " :{0}", "SignInWithCredentialAsync encountered an error: " + task.Exception);
                    Debug.LogFormat(APP_NAME + " :{0}", "SignInWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogFormat(APP_NAME + " :{0}", "SignInWithCredentialAsync encountered an error: " + task.Exception);
                    DebugOnScreen.Log("SignInWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                DebugOnScreen.Log("Login with google:"+newUser.DisplayName);
                Debug.LogFormat(APP_NAME + " :{0}", "User signed in successfully: " + newUser.DisplayName);
                DebugOnScreen.Log("User signed in successfully: " + newUser.DisplayName);
            });
        }
    }

    private void singOut()
    {
        FB.LogOut();
        //PlayGamesPlatform.Instance.SignOut();
        mAuth.SignOut();
    }
}

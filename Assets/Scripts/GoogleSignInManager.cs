using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Unity;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.UI;

public class GoogleSignInManager : MonoBehaviour
{
    public Text infoText;
    private bool IsLink = false;
    public string webClientId = "231884747474-8t5s47m1iotadabo5ac8c7p6re803gih.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        if (PlayerPrefs.GetString("current_access_token") == "google_access_token")
            StartCoroutine(WaitForInitialization());;
    }

    public void SignInWithGoogle() 
    {
        IsLink = false;
        OnSignIn(); 
    }
    public void SignOutFromGoogle() { OnSignOut(); }
    public void LinkWithGoogle()
    {
        IsLink = true;
        OnSignIn();
    }

    private void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
        }
        else
        {
            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            AddToInformation("Email = " + task.Result.Email);
            AddToInformation("Google ID Token = " + task.Result.IdToken);
            AddToInformation("Email = " + task.Result.Email);
            if (!IsLink) {
                SignInWithGoogleOnFirebase(task.Result.IdToken);
            }
            else
            {
                LinkWithGoogleOnFirebase(task.Result.IdToken);
            }

        }
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        FirebaseAuthenticator.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                AddToInformation("Sign In Successful.");
                PlayerPrefs.SetString("current_access_token", "google_access_token");
            }
        });
    }
    private void LinkWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        FirebaseAuthenticator.instance.auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                AddToInformation("Sign In Successful.");
            }
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddToInformation("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public void ShowUserInfo()
    {
        infoText.text = FirebaseAuthenticator.instance.auth.CurrentUser.DisplayName;
    }

    private void AddToInformation(string str) { infoText.text += "\n" + str; }

    private IEnumerator WaitForInitialization()
    {
        while ((FirebaseAuthenticator.instance.auth == null))
        {
            yield return null;
        }
        SignInWithGoogle();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Firebase.Auth;
using System;
using UnityEngine.UI;
using Firebase;

public class FacebookAuthenticationHandler : MonoBehaviour
{

    [SerializeField] private Text displayName;

    private void Start()
    {

        if (!FB.IsInitialized)
        {
            FB.Init(OnFacebookInitialized);
        }
        else
        {
            FB.ActivateApp();
        }


        if (PlayerPrefs.GetString("current_access_token") == "fb_access_token")
            StartCoroutine(WaitForInitialization()); 
    }

    private void OnFacebookInitialized()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.LogError("Failed to initialize the Facebook SDK");
        }
    }

    public void Login()
    {
        //if (FB.IsLoggedIn)
        //{

        //    Debug.Log("User is already logged in to Facebook");
        //    AuthenticateWithFirebase(FB.AccessToken);
        //    return;
        //}

        //Probably should save the ID on player prefs or something

        var permissions = new List<string> { "public_profile", "email" };
        FB.LogInWithReadPermissions(permissions, OnFacebookLoggedIn);
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.LogError("Failed to login to Facebook: " + result.Error);
            return;
        }

        Debug.Log("Successfully logged in to Facebook");
        AuthenticateWithFirebase(result.AccessToken);
    }

    private void AuthenticateWithFirebase(AccessToken accessToken)
    {
        var credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);
        FirebaseAuthenticator.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed to authenticate with Firebase: " + task.Exception.Message);
                return;  
            }

            Debug.Log("Successfully authenticated with Firebase");
            PlayerPrefs.SetString("current_access_token", "fb_access_token");
            var user = task.Result;
            // TODO: Handle the authenticated user
        });
    }

    private void LinkWithFirebase(AccessToken accessToken)
    {

        //string currentUserId = auth.CurrentUser.UserId;
        //string currentEmail = auth.CurrentUser.Email;
        //string currentDisplayName = auth.CurrentUser.DisplayName;
        //System.Uri currentPhotoUrl = auth.CurrentUser.PhotoUrl;

        //// Sign in with the new credentials.
        //auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
        //    if (task.IsCanceled)
        //    {
        //        Debug.LogError("SignInWithCredentialAsync was canceled.");
        //        return;
        //    }
        //    if (task.IsFaulted)
        //    {
        //        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
        //        return;
        //    }

        //    Firebase.Auth.FirebaseUser newUser = task.Result;
        //    Debug.LogFormat("User signed in successfully: {0} ({1})",
        //        newUser.DisplayName, newUser.UserId);

        //    // TODO: Merge app specific details using the newUser and values from the
        //    // previous user, saved above.
        //});

        var credential = FacebookAuthProvider.GetCredential(accessToken.TokenString);
        FirebaseAuthenticator.instance.auth.CurrentUser.LinkWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError(task.Exception);
                return;
            }

            Debug.Log("Successfully linked account with Firebase");
            var user = task.Result;
        });
    }

    private IEnumerator WaitForInitialization()
    {
        while ((!FB.IsInitialized) || (FirebaseAuthenticator.instance.auth == null))
        {
            yield return null;
        }

        Debug.Log("FB isInitialized: " + FB.IsInitialized);
        Debug.Log("Firebase isInitialized: " + FirebaseAuthenticator.instance.auth == null);
        Login();
    }

}

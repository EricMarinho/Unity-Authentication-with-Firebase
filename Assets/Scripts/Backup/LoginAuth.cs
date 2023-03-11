using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.UI;
using Google;
using System.Net.Http;
using TMPro;

public class LoginAuth : MonoBehaviour
{

    public InputField emailInputField;
    public InputField passwordInputField;
    [SerializeField] private Text nameText;
    [SerializeField] private Text emailText;

    public void LoginButton(){
        StartCoroutine(StartLogin(emailInputField.text, passwordInputField.text));
    }

    private IEnumerator StartLogin(string email, string password){
        var LoginTask = FirebaseAuthenticator.instance.auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if(LoginTask.Exception != null){
            HandleLoginErrors(LoginTask.Exception);
        }
        else{
            LoginUser(LoginTask);
        }
    }

    private void HandleLoginErrors(System.AggregateException loginException){
        Debug.LogWarning(message: $"Failed to register task with {loginException}");
        FirebaseException firebaseEx = loginException.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        //errorMessage.text = DefineLoginErrorMessage(errorCode);
        Debug.Log(DefineLoginErrorMessage(errorCode));
    }

    private string DefineLoginErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Missing Email";

            case AuthError.MissingPassword:
                return "Missinhg Password";

            case AuthError.InvalidEmail:
                return "Invalid Email";

            case AuthError.UserNotFound:
                return "User Not Found";

            default:
                return "Login failed";
        }
    }

    private void LoginUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> loginTask)
    {
        FirebaseAuthenticator.instance.user = loginTask.Result;
        Debug.LogFormat("User Signed in sucessfully {0} {1}", FirebaseAuthenticator.instance.user.DisplayName, FirebaseAuthenticator.instance.user.Email);
        //errorMessage.text = "";
        Debug.Log("Logged In");
    }

}

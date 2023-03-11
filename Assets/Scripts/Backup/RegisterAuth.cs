using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

public class RegisterAuth : MonoBehaviour
{

    public InputField usernameRegisterField;
    public InputField emailRegisterField;
    public InputField passwordRegisterField;
    private LoginAuth loginAuth;

    private void Start()
    {
        FindObjectOfType<LoginAuth>();
        loginAuth = GetComponent<LoginAuth>();
    }

    public void RegisterButton()
    {
        StartCoroutine(StartRegister(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator StartRegister(string email, string password, string userName)
    {
        if(!CheckRegistrationFieldAndReturnForErrors())
        {
            var RegisterTask = FirebaseAuthenticator.instance.auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);
            
            if(RegisterTask.Exception != null)
            {
                HandleRegisterErrors(RegisterTask.Exception);
            }
            else
            {
                StartCoroutine(RegisterUser(RegisterTask, userName, email, password));
            }
        }

    }

    private IEnumerator RegisterUser(System.Threading.Tasks.Task<Firebase.Auth.FirebaseUser> registerTask, string userName, string email, string password)
    {
        FirebaseAuthenticator.instance.user = registerTask.Result;
        
        if(FirebaseAuthenticator.instance != null)
        {
            UserProfile profile = new UserProfile { DisplayName = userName };
            var ProfileTask = FirebaseAuthenticator.instance.user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: ()=>  ProfileTask.IsCompleted);

            if(ProfileTask.Exception != null)
            {
                HandleProfileCreationErrors(ProfileTask.Exception);
            }
            else
            {
                loginAuth.emailInputField.text = email;
                loginAuth.passwordInputField.text = password;
                loginAuth.LoginButton();

            }
        }
    }

    private void HandleProfileCreationErrors(AggregateException exception)
    {
        Debug.LogWarning(message: $"Failer to register task with {exception}");
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        //warningRegisterText.text = "Username set Failed";
        Debug.Log("Username set Failed");
    }

    private void HandleRegisterErrors(AggregateException exception)
    {
        Debug.LogWarning(message: $"Failed to register task with {exception}");
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        //warningRegisterText.text = DefineRegisterErrorMessage(errorCode);
        Debug.Log(errorCode);

    }

    private string DefineRegisterErrorMessage(AuthError errorCode)
    {
        switch(errorCode)
        {
            case AuthError.MissingEmail:
                return "Missing Email";
            case AuthError.InvalidEmail:
                return "Invalid Email";
            case AuthError.MissingPassword:
                return "Missing Password";
            case AuthError.WeakPassword:
                return "Weak Passwork";
            case AuthError.EmailAlreadyInUse:
                return "Email already in Use";
            default:
                return "Unable to Register the User";

        }
    }

    private bool CheckRegistrationFieldAndReturnForErrors()
    {
        if(usernameRegisterField.text == "")
        {
            //warningRegisterText.text = "Nome de usuario vazio";
            Debug.Log("Nome de usuario vazio");
            return true;
        }
        //else if(passwordRegisterField.text != verifyPasswordRegisterField.text)
        //{
        //    warningRegisterText.text = "Senha e verificar senha não coincidem";
        //    return true;
        //}
        else { return false; }
    }
}

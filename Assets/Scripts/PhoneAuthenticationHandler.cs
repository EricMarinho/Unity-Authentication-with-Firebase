using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class PhoneAuthenticationHandler : MonoBehaviour
{
    public InputField phoneNumberInput;
    public InputField verificationCodeInput;
    private string verificationId;

    private void Awake()
    {
        if (PlayerPrefs.GetString("current_access_token") == "otp_access_token")
        {
            StartCoroutine(WaitForInitialization());
        }
    }

    public void SendVerificationCode()
    {
        PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(FirebaseAuthenticator.instance.auth);
        provider.VerifyPhoneNumber(phoneNumberInput.text,
            60, // timeout duration in seconds
            null,
            verificationCompleted: (credential) =>
            {
                // This callback is triggered automatically on Android devices
                // when the verification code is detected without the need for the user
                // to manually enter the code.
                Debug.Log("Verification completed");
                FirebaseAuthenticator.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError(task.Exception);
                    }
                    else
                    {
                        Debug.Log("Authentication successful");
                    }
                });
            },
            verificationFailed: (error) =>
            {
                Debug.LogError(error);
            },
            codeSent: (id, token) =>
            {
                Debug.Log("Verification code sent");
                verificationId = id;
            },
            codeAutoRetrievalTimeOut: (id) =>
            {
                Debug.Log("Code auto retrieval timed out");
            });
    }

    public void VerifyCode()
    {
        PhoneAuthProvider provider = PhoneAuthProvider.GetInstance(FirebaseAuthenticator.instance.auth);
        Credential credential = provider.GetCredential(verificationId, verificationCodeInput.text);
        FirebaseAuthenticator.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError(task.Exception);
            }
            else
            {
                Debug.Log("Authentication successful");
            }
        });
    }

    public void VerifyCode(Credential credential)
    {
        FirebaseAuthenticator.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError(task.Exception);
            }
            else
            {
                Debug.Log("Authentication successful");
            }
        });
    }

    private IEnumerator WaitForInitialization()
    {
        while ((FirebaseAuthenticator.instance.auth == null))
        {
            yield return null;
        }
        string credentialString = PlayerPrefs.GetString("firebase_auth_credential");
        byte[] credentialBytes = Convert.FromBase64String(credentialString);
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(credentialBytes);
        Credential credential = (Credential)bf.Deserialize(ms);
        VerifyCode(credential);
    }

}

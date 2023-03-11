using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserHandler : MonoBehaviour
{
    [SerializeField] private Text infoText;
    [SerializeField] private InputField newUserInputField;

    public void ShowUserInfo()
    {
        infoText.text = FirebaseAuthenticator.instance.auth.CurrentUser.UserId;
    }

    public void ChangeUsername()
    {
        Firebase.Auth.FirebaseUser user = FirebaseAuthenticator.instance.auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = newUserInputField.text
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });
        }
    }

}

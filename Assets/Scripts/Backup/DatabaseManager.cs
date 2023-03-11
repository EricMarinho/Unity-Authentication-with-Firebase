//using Firebase.Database;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class DatabaseManager : MonoBehaviour
//{
//    public InputField emailInputField;
//    public InputField passwordInputField;

//    private string userID;
//    private DatabaseReference mDatabaseRef;

//    private void Start()
//    {
//        userID = SystemInfo.deviceUniqueIdentifier;
//        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
//    }

//    public void CreateUser()
//    {
//        User newUser = new User(emailInputField.text, passwordInputField.text);
//        string json = JsonUtility.ToJson(newUser);

//        mDatabaseRef.Child("users").Child(userID).SetRawJsonValueAsync(json);
//    }

//    public IEnumerator GetPassword(Action<string> onCallback)
//    {
//        var userNameData = mDatabaseRef.Child("users").Child(userID).Child("username").GetValueAsync();

//        yield return new WaitUntil(predicate: () => userNameData.IsCompleted);

//        if (userNameData != null)
//        {
//            DataSnapshot snapshot = userNameData.Result;
//            onCallback.Invoke(snapshot.Value.ToString());
//        }

//    }

//    public IEnumerator GetEmail(Action<string> onCallback)
//    {
//        var userNameData = mDatabaseRef.Child("users").Child(userID).Child("email").GetValueAsync();

//        yield return new WaitUntil(predicate: () => userNameData.IsCompleted);

//        if (userNameData != null)
//        {
//            DataSnapshot snapshot = userNameData.Result;
//            onCallback.Invoke(snapshot.Value.ToString());
//        }

//    }

//    public void GetUserInfo()
//    {
//        StartCoroutine(GetEmail((string email) =>
//        {
//            Debug.Log(email);
//        }));

//        StartCoroutine(GetPassword((string password) =>
//        {
//            Debug.Log(password);
//        }));
//    }

//    public void UpdatePlayerInfo() // Basically you can update different values sepparately, like just the username or just the email
//    {
//        mDatabaseRef.Child("users").Child(userID).Child("password").SetValueAsync(passwordInputField.text);
//        mDatabaseRef.Child("users").Child(userID).Child("email").SetValueAsync(emailInputField.text);
//    }
//}

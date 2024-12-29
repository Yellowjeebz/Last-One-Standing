using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class oldRegister : MonoBehaviour
{
    public TMP_InputField nameField;
    public TMP_InputField passwordField;
    public TMP_InputField pConfirmationField;
    public Button registerButton;
    public TMP_Text passwordsDontMatchText;
    public string userDetailsScreen;
    public bool pwMatch;
    public bool pwMatchMessageActive;
    
    void Start()
    {
        passwordsDontMatchText.gameObject.SetActive(false);
    }
    void Update()
    {
        if(passwordField.text.Length > 0 && pConfirmationField.text.Length > 0)
        {
            confirmPasswords();            
            if(!pwMatch)
            {
                passwordsDontMatchText.gameObject.SetActive(true);
            } else
            {
                passwordsDontMatchText.gameObject.SetActive(false);
            }   
        }
        else if(passwordField.text.Length == 0 || pConfirmationField.text.Length == 0)
        {
            passwordsDontMatchText.gameObject.SetActive(false);
        }
    }
    public void CallRegisterMethod()
    {
        //if(pwMatch){
        StartCoroutine(RegisterMethod());
        
    }
    public void confirmPasswords()
    {
        if(passwordField.text == pConfirmationField.text)
        {
            pwMatch = true;
        }
        else pwMatch = false;
    }
    IEnumerator RegisterMethod()//make a call to the WAMP URL (localhost)
    {
        WWWForm form = new WWWForm();
        form.AddField("Name", nameField.text);
        form.AddField("Password", passwordField.text);
        
        using(UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/register.php", form))
        {
            yield return www.SendWebRequest();
            if(www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("error" + www.error);
                //UnityEngine.SceneManagement.SceneManager.LoadScene(userDetailsScreen);
            }
            else
            {
                Debug.Log("success");   
            }  
        }
    }
    public void VerifyInputs()
    {
        //tells the forms to only accept if the conditions are met (validation)
        registerButton.interactable = (nameField.text.Length >= 8 && passwordField.text.Length >=8);
    }



}

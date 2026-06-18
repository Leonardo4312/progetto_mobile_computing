using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabAuth : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    // Funzione associata al bottone LOGIN
    public void OnClickLogin()
    {
        
        if (usernameInput.text.Length < 3 || passwordInput.text.Length < 6)
        {
            statusText.text = GameManager.instance.isItalian 
                ? "L'username deve avere 3 caratteri e la password almeno 6!" 
                : "Username must be at least 3 characters and password at least 6!";
            return;
        }

        
        statusText.text = GameManager.instance.isItalian ? "Connessione a PlayFab..." : "Connecting to PlayFab...";

        var request = new LoginWithPlayFabRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }

    // Funzione associata al bottone REGISTRATI
    public void OnClickRegister()
    {
        
        if (usernameInput.text.Length < 3 || passwordInput.text.Length < 6)
        {
            statusText.text = GameManager.instance.isItalian 
                ? "Username (min 3) o Password (min 6) non validi!" 
                : "Invalid Username (min 3) or Password (min 6)!";
            return;
        }

       
        statusText.text = GameManager.instance.isItalian ? "Creazione account..." : "Creating account...";

        var request = new RegisterPlayFabUserRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        
        statusText.text = GameManager.instance.isItalian ? "Accesso consentito!" : "Access granted!";
        
       
        GameManager.instance.OnLoginVerified();
    }

    private void OnLoginFailure(PlayFabError error)
    {
        
        string errorePrefisso = GameManager.instance.isItalian ? "Errore Login: " : "Login Error: ";
        statusText.text = errorePrefisso + error.ErrorMessage;
        
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        
        statusText.text = GameManager.instance.isItalian 
            ? "Account Creato! Premi ACCEDI per entrare." 
            : "Account Created! Press LOGIN to enter.";
    }

    private void OnRegisterFailure(PlayFabError error)
    {
       
        string errorePrefisso = GameManager.instance.isItalian ? "Errore Reg: " : "Registration Error: ";
        statusText.text = errorePrefisso + error.ErrorMessage;
        
        Debug.LogError(error.GenerateErrorReport());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NHDigital.Scripts;
using UnityEngine.UI;
using System;

public class NHDStartup : MonoBehaviour
{
    NHDUSerLogin userlogin;
    NHDLogger log;
    public Text debugtext;
    public Button cloudloginbutton;
    public Button justplaybutton;

    void DeactivateButtons()
    {
        cloudloginbutton.gameObject.SetActive(false);
        justplaybutton.gameObject.SetActive(false);
    }

    public void ButtonCloudLogin()
    {
        debugtext.text = "Cloud login - please wait";

        DeactivateButtons();

        UserLogin();

        return;
    }

    public void ButtonJustPlay()
    {
        debugtext.text = "Playing offline";

        DeactivateButtons();

        return;
    }

    void UpdateStatus()
    {
        string strId = "PlayFab id: " + userlogin.strPFUserId;
        if (userlogin.strXboxGamertag != "")
            strId += " | Gamertag: " + userlogin.strXboxGamertag;

        debugtext.text = strId;

        log.Write(strId);
    }


    IEnumerator WaitforXboxLoginCompletion()
    {
        log.Write("WaitforXboxLoginCompletion pre-sleeper " + userlogin.bXboxLoginComplete);

        while (!userlogin.bXboxLoginComplete)
        {
            yield return new WaitForSeconds(1);
        }

        log.Write("WaitforXboxLoginCompletion post-sleeper " + userlogin.bXboxLoginComplete);

        userlogin.PlayFabLogin();
    }

    IEnumerator WaitforPlayFabLoginCompletion()
    {
        log.Write("WaitforPlayFabLoginCompletion pre-sleeper " + userlogin.bPlayFabLoginComplete);

        while (!userlogin.bPlayFabLoginComplete)
        {
            yield return new WaitForSeconds(1);
        }

        log.Write("WaitforPlayFabLoginCompletion post-sleeper " + userlogin.bPlayFabLoginComplete);

        UpdateStatus();
    }

    void UserLogin()
    {
        userlogin = new NHDUSerLogin(log);

        switch (Application.platform)
        {
            case RuntimePlatform.XboxOne:
            case RuntimePlatform.WSAPlayerX64:
                userlogin.XboxLogin();
                StartCoroutine(WaitforXboxLoginCompletion());
                break;

            case RuntimePlatform.WindowsEditor:
                userlogin.PlayFabLogin();
                break;

            default:
                userlogin.PlayFabLogin();
                break;

        }

        StartCoroutine(WaitforPlayFabLoginCompletion());

    }

    void Start()
    {
        log = new NHDLogger();

        debugtext.text = "Welcome";

    }

#if DEBUG
    private void OnGUI()
    {
        log.OnGUI();
    }
#endif 

}

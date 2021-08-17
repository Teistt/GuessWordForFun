using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TitleGameUI : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject optionPanel;
    public GameObject playSettingPanel;
    public InputField teamNumber;
    public InputField cardNumber;
    public Text infoTextTeams;
    public Text infoTextCards;

    private int minTeamNumber = 2;
    private int maxTeamNumber = 8;
    private int minCardsNumber = 10;
    private int maxCardsNumber = 100;

    private void Start()
    {
        mainPanel.SetActive(true);
        playSettingPanel.SetActive(false);
        optionPanel.SetActive(false);
        teamNumber.text = GameSettings._teamsNumber.ToString();
        cardNumber.text = GameSettings._cardsNumber.ToString();

        infoTextTeams.text = "Minimal Teams Number "+ minTeamNumber + "\n Maximal Teams Number " + maxTeamNumber;
        infoTextCards.text = "Minimal Cards Number " + minCardsNumber + "\n Maximal Cards Number " + maxCardsNumber;
    }

    public void MainMenuButton()
    {
        mainPanel.SetActive(true);
        playSettingPanel.SetActive(false);
        optionPanel.SetActive(false);
        Debug.Log("main");
    }

    public void ChangeLanguage(string langue)
    {
        GameSettings._langue = langue;
    }

    public void PlayButton()
    {
        mainPanel.SetActive(false);
        playSettingPanel.SetActive(true);
        optionPanel.SetActive(false);
        Debug.Log("yes");
    }
    public void CreditsButton()
    {
        Debug.Log("Credits");
    }

    public void OptionButton()
    {
        mainPanel.SetActive(false);
        playSettingPanel.SetActive(false);
        optionPanel.SetActive(true);
        Debug.Log("options");
    }
    public void QuitButton()
    {

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
}

#region TEAMNUMBER
public void SetTeamNumber()
    {
        
        int nb = int.Parse(teamNumber.text);

        if (nb < minTeamNumber)
        {
            nb = minTeamNumber;
        }

        if (nb > maxTeamNumber)
        {
            nb = maxTeamNumber;
        }

        teamNumber.text = nb.ToString();
        GameSettings._teamsNumber = nb;
        Debug.Log(GameSettings._teamsNumber);
    }

    public void ButtonTeamNumber(int input)
    {
        int nb = GameSettings._teamsNumber;
        nb=nb+input;

        if (nb < minTeamNumber)
        {
            nb = minTeamNumber;
        }

        if (nb > maxTeamNumber)
        {
            nb = maxTeamNumber;
        }
                
        teamNumber.text = nb.ToString();
        GameSettings._teamsNumber = nb;
        Debug.Log(GameSettings._teamsNumber);
    }
    #endregion
    
    #region CARDNUMBER
    public void SetCardNumber()
    {
        int nb = int.Parse(cardNumber.text);

        if (nb < minCardsNumber)
        {
            nb = minCardsNumber;
        }

        if (nb > maxCardsNumber)
        {
            nb = maxCardsNumber;
        }

        cardNumber.text = nb.ToString();
        GameSettings._cardsNumber = nb;
        Debug.Log(GameSettings._cardsNumber);
    }

    public void ButtonCardNumber(int input)
    {
        int nb = GameSettings._cardsNumber;
        nb = nb + input;

        if (nb < minCardsNumber)
        {
            nb = minCardsNumber;
        }

        if (nb > maxCardsNumber)
        {
            nb = maxCardsNumber;
        }

        cardNumber.text = nb.ToString();
        GameSettings._cardsNumber = nb;
        Debug.Log(GameSettings._cardsNumber);
    }
    #endregion

    public void LaunchGame()
    {
        SceneManager.LoadScene(1);
    }
}

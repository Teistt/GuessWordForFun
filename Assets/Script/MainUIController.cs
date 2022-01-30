using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUIController : MonoBehaviour
{
    #region Singleton
    //On créé un BM pour y lier ce BM
    public static MainUIController instance;

    //Singleton: permet d'accèder au build manager depuis n'importe où sans avoir à le définir à chaque fois
    private void Awake()
    {
        //On vérifie si un build manager a déjà été instantié
        if (instance != null)
        {
            //Si oui, erreur
            Debug.LogError("double mainUIController");
            return;
        }
        //Sinon on instantie ce BM
        instance = this;
    }
    #endregion


    private GameController gameController;

    public GameObject[] casesCanvas;
    public Text[] teamNumberCases;
    public Text[] mancheNumberCases;
    public Text rulesText;

    public Text recapPointsTeam;
    public Text recapPointsManche;
    public Text recapPointsVictory;
    public Text winnerTeam;

    public Text timerManche;

    public Text cardManche;

    public Text recapCardChangeTeam;



    //Pour chaque équipes on va compter le nbre d'éléments dans deck avec M1Win correspondant à l'id de la team
    private int[] pointsPerTeams = new int[GameSettings._teamsNumber];

    private void Start()
    {

        gameController = GameController.instance;

        foreach (GameObject panel in casesCanvas)
        {
            panel.SetActive(false);
        }
        casesCanvas[0].SetActive(true);

        for (int i = 0; i < pointsPerTeams.Length; i++)
        {
            pointsPerTeams[i] = 0;
        }
    }


    public void Launch()
    {
        gameController._stateNumber = 1;

        foreach (GameObject i in casesCanvas)
        {
            i.SetActive(false);
        }
        casesCanvas[1].SetActive(true);
    }

    public void C1Display(int mancheNumber, int teamIndex) {
        //mancheNumberCases[0].text = "Manche " + mancheNumber;
        //teamNumberCases[0].text = "Team " + (teamIndex + 1) + " GO!!";

        switch (mancheNumber)
        {
            case 1:
                rulesText.text= "Tu dois faire deviner le mot à ton équipe. Tu ne peux pas prononcer de mots similaires ni les traduire ou les épeler.";
                break;
                
            case 2:
                rulesText.text= "N'utilise qu'un seul mot par carte. Tes équipiers ne peuvent faire qu'une seule proposition, si c'est loupé, passe à la suivante.";
                break;
                
            case 3:
                rulesText.text= "Mime le mot. Tu peux fredonner ou faire des bruitages, mais tu ne dois pas parler.";
                break;
                
            default:
                rulesText.text= "ERROR";
                break;
        }
    }
    
    public void C2Display(int mancheNumber, int teamIndex){
        mancheNumberCases[1].text = "Manche " + mancheNumber;
        teamNumberCases[1].text = "Team " + (teamIndex+1) + " GO!!";
    }


    public void C3Display(int mancheNumber, int teamIndex)
    {
        mancheNumberCases[2].text = "Round " + mancheNumber+ " Time's UP!";
        teamNumberCases[2].text = "Team " + (teamIndex + 1);
    }

    public void GoToState(int sstate)
    {
        Debug.Log("Next state, sstate ="+sstate);
        foreach (GameObject i in casesCanvas)
        {
            i.SetActive(false);
        }

        casesCanvas[sstate].SetActive(true);
        
        gameController.GoToState(sstate);
    }

    public void NextManche()
    {
        foreach (GameObject i in casesCanvas)
        {
            i.SetActive(false);
        }
        casesCanvas[1].SetActive(true);

        gameController.NextManche();
        //On relance le c1_Rules
    }

    public void Relaunch()
    {
        //Just reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Menu()
    {
        //Go to Main Menu, aka scene 0
        SceneManager.LoadScene(0);
    }

    public void UpdateTimerManche(float time)
    {
        timerManche.text = Mathf.Round(time) + "s";
    }

    public void CardMancheTxt(string txtCard)
    {
        cardManche.text = txtCard;
    }


    public void RecapPoints(int mancheNumber, int teams, List<CardsBP> deck)
    {
        mancheNumberCases[3].text = "Round " + mancheNumber + " COMPLETE!";
        switch (mancheNumber)
        {
            case 1:
                for (int i = 0; i < pointsPerTeams.Length; i++)
                {
                    foreach (CardsBP cards in deck)
                    {
                        if (cards.M1_win == i)
                        {
                            pointsPerTeams[i]++;
                        }
                    }
                }

                recapPointsManche.text = "Score:";

                for (int i = 0; i < pointsPerTeams.Length; i++)
                {
                    recapPointsManche.text += $"\nTeam {i + 1}: {pointsPerTeams[i]} pts ";
                }
                break;
                

            case 2:
                for (int i = 0; i < pointsPerTeams.Length; i++)
                {
                    foreach (CardsBP cards in deck)
                    {
                        if (cards.M2_win == i)
                        {
                            pointsPerTeams[i]++;
                        }
                    }
                }

                recapPointsManche.text = "Well Played!\nRecap:\n";

                for (int i = 0; i < pointsPerTeams.Length; i++)
                {
                    recapPointsManche.text += $"\nTeam {i + 1}: {pointsPerTeams[i]} points ";
                }
                break;
                

            case 3:
                for (int i = 0; i < pointsPerTeams.Length; i++)
                {
                    foreach (CardsBP cards in deck)
                    {
                        if (cards.M3_win == i)
                        {
                            pointsPerTeams[i]++;
                        }
                    }
                }

                recapPointsVictory.text = "Well Played!\nRecap:\n";

                for (int i = 0; i < pointsPerTeams.Length; i++)
                {
                    recapPointsVictory.text += $"\nTeam {i + 1}: {pointsPerTeams[i]} points ";
                }

                int pointsWinner = pointsPerTeams.Max();
                int idWinner = pointsPerTeams.ToList().IndexOf(pointsWinner);

                winnerTeam.text = "TEAM "+(idWinner+1)+" WIN!!!";
                break;

        }
    }
    

    public void RecapMancheTxt(List<CardsBP> deck)
    {
        Debug.Log("Recap Manche");
        recapCardChangeTeam.text = "Well Played!\nRecap:\n";
        foreach (CardsBP element in deck)
        {
            recapCardChangeTeam.text += $"\n{element.word} ";
        }
    }

    public void CheckCard()
    {

        //Quand on appuie sur le bouton check de la carte
        //on lance cette fonction qui vient lancer gameController.CheckCard()
        //Si après avoir validé la carte le deckRound est vide le num de la manche
        //Actuelle est retourné afin de passer à la suivante
        //Sinon on recoit 0
        int mmanche = gameController.CheckCard();
        if (mmanche !=0)
        {
            if (mmanche == 3)
            {
                GoToState(5);
            }
            else
            {
                GoToState(4);
            }
        }
    }

    public void SkipCard()
    {
        gameController.SkipCard();
    }
}

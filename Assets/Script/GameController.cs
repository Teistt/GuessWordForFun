using System;
using System.Text;
using System.Security.Cryptography;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{

    #region Singleton
    //On créé un BM pour y lier ce BM
    public static GameController instance;

    //Singleton: permet d'accèder au build manager depuis n'importe où sans avoir à le définir à chaque fois
    private void Awake()
    {
        //On vérifie si un build manager a déjà été instantié
        if (instance != null)
        {
            //Si oui, erreur
            Debug.LogError("double GameController");
            return;
        }
        //Sinon on instantie ce BM
        instance = this;
    }
    #endregion

    private int mancheNumber;
    public int _mancheNumber { get { return mancheNumber; } set { mancheNumber = value; } }

    private int stateNumber;
    public int _stateNumber { get { return stateNumber; } set { stateNumber = value; } }


    private MainUIController mainUIController;
    public List<CardsBP> deck = new List<CardsBP>();
    private List<CardsBP> deckRound = new List<CardsBP>();
    private List<CardsBP> deckRoundWin = new List<CardsBP>();

    private int cardsInGame;
    private int teamsInGame;
    private float baseChrono;

    private int cardIndex=0;
    private int teamIndex=0;

    float chrono;

    private bool c1Displayed = false;
    private bool c2Displayed = false;
    private bool launchedTimer = false;
    private bool changedTeam = false;
    private bool pointsCalculated = false;
    
    

    private void Start()
    {
        //BetterStreamingAssets inialization. SSe DictionnaryPath()
        BetterStreamingAssets.Initialize();
        NewDictionnaryRead();
        mainUIController = MainUIController.instance;

        mancheNumber = 0;
        stateNumber = 0;
        cardsInGame = GameSettings._cardsNumber;
        teamsInGame = GameSettings._teamsNumber;
        teamIndex = 0;

        baseChrono = GameSettings._baseChrono;
        chrono = baseChrono;
        //Debug.Log("manche number: " + mancheNumber + " , state number: " + stateNumber);

        //On récupère tous les mots existant dans le dico du jeu
        string[] cards = ParseFile();
        deck.Capacity = cards.Length;

        //On remplit la liste à partir du tableau cards
        for (int i = 0; i < cards.Length; i++)
        {
            deck.Add(new CardsBP() { word = cards[i], id = i});
        }

        //On mélange toutes les cartes tirées
        ShuffleDeck(deck);


        //On supprime les dernières cartes de la List Deck pour avoir le nombre de cartes défini dans GameSettings
        deck.RemoveRange(cardsInGame, deck.Count - cardsInGame);

        deck.ForEach(val => { deckRound.Add(val); });
        mancheNumber=1;
    }

    void Update()
    {
        //Pour chaque manche, on switchera d'UI suivant l'état
        //State 0: uniquement avant manche 1, écran prêt à lancer le jeu
        //State 1: écran des règles
        //State 2: écran de jeu avec cartes et timer
        //State 3: tjrs dans la même manche, écran avant de changer de team
        //State 4: uniquement après fin manche, récap points totaux
        //State 5: uniquement après manche 4, écran de fin du jeu et de victoire

        switch (stateNumber)
        {
            //Case 0: uniquement avant manche 1, écran prêt à lancer le jeu
            case 0:
                break;

            //Case 1: écran des règles
            case 1:
                if (c1Displayed) { 
                    break;
                }
                c1Displayed = true;
                pointsCalculated = false;
                mainUIController.C1Display(mancheNumber, teamIndex);
                
                break;

            //Case 2: écran de jeu avec cartes et timer
            case 2:
                Debug.Log("state 2");
                c1Displayed = false;
                pointsCalculated = false;

                if (launchedTimer)
                {
                    Debug.Log("launched timer");
                    //La manche a commencé
                    chrono -= Time.deltaTime;

                    if (chrono <= 0)
                    {
                        //On check si il reste des cartes à valider dans le deckM1
                        //(spoiler si on arrive ici il en reste, mais on check quand même paske merde)
                        if (deckRound.Count == 0)
                        {
                            //Si on est pas à la 3e manche
                            if (mancheNumber < 3)
                            {
                                //On passe au case 4 corresponant aux règles de la manche suivante; autrement dit on saute le case3
                                mainUIController.GoToState(4);
                            }
                            else
                            {
                                //Sinon on passe au case 5, aka la fin du game
                                mainUIController.GoToState(5);
                            }
                            
                        }

                        //Sinon on charge le case3 pour la prochaine team
                        //Le case3 renverra au case2
                        else
                        {
                            //Debug.Log("deckM1.Count!=0");
                            mainUIController.RecapMancheTxt(deckRoundWin);
                            ResetDeckRoundWin();
                            mainUIController.GoToState(3);
                        }
                        break;
                    }

                    mainUIController.CardMancheTxt(deckRound[cardIndex].word);

                }
                else
                {

                    Debug.Log("!launched timer");
                    //On active un éventuel changement d'équipe à la fin du timer
                    changedTeam = false;
                    //Quand on débute la première manche, on lance le timer

                    launchedTimer = !launchedTimer;
                }

                mainUIController.UpdateTimerManche(chrono);

                Debug.Log("coucou");
                if (c2Displayed)
                {
                    break;
                }
                c2Displayed = true;
                mainUIController.C2Display(mancheNumber, teamIndex);

                break;

            //Case 3: tjrs dans la même manche, écran avant de changer de team
            case 3:

                Debug.Log("wesh state 3");
                //on ne change d'équipe que lors du passage de la première Update()
                if (changedTeam)
                {
                    break;
                }

                c2Displayed = false;
                ChangeTeamRound();
                changedTeam = true;
                mainUIController.C3Display(mancheNumber, teamIndex);

                break;

            //Case 4: uniquement après fin manche 1 et 2, récap points totaux
            case 4:
                if (pointsCalculated)
                {
                    break;
                }

                c2Displayed = false;
                pointsCalculated = true;
                mainUIController.RecapPoints(mancheNumber, teamsInGame,deck);

                ResetDeckRoundWin();
                ShuffleDeck(deck);
                RefillDeckRound();
                ChangeTeamRound();
                chrono = baseChrono;
                
                break;

            //Case 5: uniquement après manche 3, écran de fin du jeu et de victoire
            case 5:
                if (pointsCalculated) break;
                pointsCalculated = true;
                mainUIController.RecapPoints(mancheNumber, teamsInGame, deck);

                break;

            default:
                Debug.LogError("OUT OF MAIN SWITCH");

                break;
        }
    }

    public void NextState()
    {
        stateNumber++;
    }

    public void GoToState(int sstate)
    {
        stateNumber = sstate;
    }


    public void NextManche()
    {
        
        if (_mancheNumber >= 3)
        {
            GoToState(5);
        }
        else
        {
            mancheNumber++;
            stateNumber = 1;
        }
        
        //Reremplir deckRound et vider deckRoundWin
    }

    private void ChangeTeamRound()
    {
        //On est sorti d'une manche, on reset le chrono et la booleenne associée
        //On récap les scores de la team précédente de la manche 1
        //Avant de retourner au case 2 pour la prochaine team
        chrono = baseChrono;
        launchedTimer = !launchedTimer;
        cardIndex = 0;

        //Si il y a x équipes, leurs ID iront de 0 à X-1
        if (teamIndex >= (teamsInGame - 1))
        {
            teamIndex = 0;
        }
        else
        {
            teamIndex++;
        }
    }


    public void SkipCard()
    {
        if (cardIndex >= (deckRound.Count - 1))
        {
            cardIndex = 0;
        }
        else
        {
            cardIndex++;
        }
    }


    public int CheckCard()
    {
        //Quand on appuie sur le bouton check de la carte
        //on lance MainUIController.CheckCard() qui vient lancer cette fonction
        //Si après avoir validé la carte le deckRound est vide on retourne la manche
        //Actuelle afin de passer à la suivante
        //Sinon on retourne 0
        switch (mancheNumber)
        {
            case 1:
                deckRound[cardIndex].M1_win = teamIndex;
                deckRoundWin.Add(deckRound[cardIndex]);
                deckRound.RemoveAt(cardIndex);

                if (cardIndex >= (deckRound.Count) - 1)
                {
                    cardIndex = 0;
                }

                if (deckRound.Count == 0)
                {
                    return mancheNumber;
                }

                break;
                
            case 2:
                deckRound[cardIndex].M2_win = teamIndex;
                deckRoundWin.Add(deckRound[cardIndex]);
                deckRound.RemoveAt(cardIndex);

                if (cardIndex >= (deckRound.Count) - 1)
                {
                    cardIndex = 0;
                }

                if (deckRound.Count == 0)
                {
                    return mancheNumber;
                }

                break;
                
            case 3:
                deckRound[cardIndex].M3_win = teamIndex;
                deckRoundWin.Add(deckRound[cardIndex]);
                deckRound.RemoveAt(cardIndex);


                if (cardIndex >= (deckRound.Count) - 1)
                {
                    cardIndex = 0;
                }

                if (deckRound.Count == 0)
                {
                    return mancheNumber;
                }

                break;

            default:
                Debug.LogError("out of switch CheckCard");

                return 0;
        }

        return 0;
    }

    private void RefillDeckRound()
    {
        deckRound.Clear();
        deck.ForEach(val => { deckRound.Add(val); });
        deckRound.ForEach(val => Debug.Log(val.word));
    }
    
    private void ResetDeckRoundWin()
    {
        deckRoundWin.Clear();
    }


    public void Relaunch()
    {
        //Reset score
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    #region DECK_BUILDING
    
    
    string NewDictionnaryRead()
    {
        //Using BetterStreamingAssets plugin.
        //Precaution: keep all file names in StreamingAssets lowercase

        //BetterStreamingAssets.Initialize() done in Start().
        //All paths are relative to StreamingAssets directory. So we want "dictionnaries/en.xml"

        string path = "dictionnaries/"+GameSettings._langue+".xml";
        byte[] data = BetterStreamingAssets.ReadAllBytes(path);
        //Debug.Log(BetterStreamingAssets.ReadAllText(path));
        string text = Encoding.UTF8.GetString(data,0,data.Length);
        //Debug.Log(text);
        return text;
    }


    string DictionnaryPath()
    {
        string dictionnaryRelativePath = Application.streamingAssetsPath;
        dictionnaryRelativePath = dictionnaryRelativePath + "\\dictionnaries\\" + GameSettings._langue + ".xml";
        //Debug.Log(dictionnaryRelativePath);
        return dictionnaryRelativePath;
    }


    string[] ParseFile()
    {
        //string text = File.ReadAllText(DictionnaryPath());
        string text = NewDictionnaryRead();

        char[] separators = { '\n' };
        string[] strValues = text.Split(separators);
        
        return strValues;
    }


    void ShuffleDeck(List<CardsBP> bp)
    {
        //Obligé de déclarer using System et de spécifier de prendre System.Random() sinon le compiler ne sait pas si
        //Il doit utiliser le random de Unity (static) ou celui du System
        var rand = new System.Random();
        //OrderBy appartien à System.Linq
        var randomized = deck.OrderBy(item => rand.Next());

        List<CardsBP> listTemp = new List<CardsBP>();
        CardsBP temp;

        for (int i = 0; i < randomized.Count(); i++)
        {
            //Debug.Log(randomized.ElementAt(i));
            do
            {
                temp = randomized.ElementAt(i);
            } while (listTemp.Contains(temp));
            listTemp.Add(temp);
            //Debug.Log(bp[i].word);
        }
        bp.Clear();
        listTemp.ForEach(val => { bp.Add(val); });

        listTemp.Clear();
    }

    #endregion DECK_BUILDING
}

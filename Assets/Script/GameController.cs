using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class GameController : MonoBehaviour
{
    //Singleton
    #region Singleton
    //GameController construction to link this one
    public static GameController instance;

    //Singleton: allow to have only one of GameController created
    private void Awake()
    {
        //Check if a GameController already exist
        if (instance != null)
        {
            Debug.LogError("double GameController");
            return;
        }
        //If there is not GameController already created, create this one
        instance = this;
    }
    #endregion

    //Round number
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
    public int _teamIndex { get { return teamIndex; } }

    float chrono;

    private bool c1Displayed = false;
    private bool c2Displayed = false;
    private bool launchedTimer = false;
    private bool changedTeam = false;
    private bool pointsCalculated = false;

    private string dictionnary=null;
    
    IEnumerator  Start()
    {
        //BetterStreamingAssets inialization. See DictionnaryPath()

        mainUIController = MainUIController.instance;

        mancheNumber = 0;
        stateNumber = 0;
        cardsInGame = GameSettings._cardsNumber;
        teamsInGame = GameSettings._teamsNumber;
        teamIndex = 0;

        baseChrono = GameSettings._baseChrono;
        chrono = baseChrono;

        BetterStreamingAssets.Initialize();

        //We read all words in xml file
        yield return NewDictionnaryRead();
        
        //All words listed in the GameSettings._langue dictionnary are separated in a string array
        string[] cards = ParseFile();
        deck.Capacity = cards.Length;

        //We fill the deck List based on the cards array
        for (int i = 0; i < cards.Length; i++)
        {
            deck.Add(new CardsBP() { word = cards[i], id = i});
        }

        //We shuffle the deck list, aka mix games cards
        ShuffleDeck(deck);


        //And we remove the end of the deck list according to only have the number of cards as stated in GameSettings._cardsNumber
        deck.RemoveRange(cardsInGame, deck.Count - cardsInGame);

        //We duplicate the game's deck in order to manipulate a "round" deck, since all rounds will be based on the same deck
        deck.ForEach(val => { deckRound.Add(val); });

        //We can go to Round 1
        mancheNumber=1;
    }

    void Update()
    {
        //For each round, we will switch unity's panel according to the state of the round
        //State 0: only for round 1, ready to launch game screen
        //State 1: Rules of the round
        //State 2: screen game with cards, skip and check buttons
        //State 3: screen recap cards and to change team (if deckRound not empty and timer <=0)
        //State 4: screen recap points and to next round (if deckRound empty)
        //State 5: only after round 3, screen victory screen

        switch (stateNumber)
        {
            //Case 0: only for round 1, ready to launch game screen
            case 0:
                break;

            //Case 1: Rules of the round
            case 1:
                //Update case 1 panel only one time
                if (c1Displayed) { 
                    break;
                }
                c1Displayed = true;

                //Reset pointsCalculated in anticipation of previous case 4
                pointsCalculated = false;

                //Update Rule Text according to round number and teamButton text according to team order to play
                mainUIController.C1Display(mancheNumber, teamIndex);
                
                break;

            //Case 2: screen game with cards, skip and check buttons
            case 2:
                //Reset c1Displayed in anticipation of future case 1
                c1Displayed = false;
                pointsCalculated = false;

                //Launch timer only once
                if (launchedTimer)
                {
                    //the round has started

                    Debug.Log("timer launched");
                    //update timer since last frame
                    chrono -= Time.deltaTime;

                    if (chrono <= 0)
                    {
                        //We check if deckRound is empty
                        //(it shouldn't happen at all)
                        if (deckRound.Count == 0)
                        {
                            //If we are not at the last round go to state 4
                            if (mancheNumber < 3)
                            {
                                //Go to state 4 to recap present round and access next round
                                mainUIController.GoToState(4);
                            }
                            //else go to state 5
                            else
                            {
                                //Go to victory screen
                                mainUIController.GoToState(5);
                            }
                            
                        }

                        //If deckRound is not empty, go to state 3 for next team turn
                        else
                        {
                            //Debug.Log("deckM1.Count!=0");
                            //We update stae 3 recap text before emptying deckroundwin
                            mainUIController.RecapMancheTxt(deckRoundWin);
                            ResetDeckRoundWin();
                            mainUIController.GoToState(3);
                        }
                        break;
                    }

                    //While there is time for the team's round, display current word
                    mainUIController.CardMancheTxt(deckRound[cardIndex].word);

                }
                else
                {
                    //If first frame of the state 2, we launch timer with the GameSettings._baseChrono value

                    //Reset changedTeam for a possible future state 3 or 4
                    //On active un éventuel changement d'équipe à la fin du timer
                    changedTeam = false;
                    //Quand on débute la première manche, on lance le timer

                    launchedTimer = !launchedTimer;
                }

                //Refresh timer display
                mainUIController.UpdateTimerManche(chrono);
                
                //If first frame of state 2, update round number and team playing display
                if (c2Displayed)
                {
                    break;
                }
                c2Displayed = true;
                mainUIController.C2Display(mancheNumber, teamIndex);

                break;

            //Case 3: screen recap cards and to change team (if deckRound not empty and timer <=0)
            case 3:

                Debug.Log("state 3");
                //We change team only for this first state 3 call
                if (changedTeam)
                {
                    break;
                }

                c2Displayed = false;
                ChangeTeamRound();
                changedTeam = true;
                mainUIController.C3Display(mancheNumber, teamIndex);

                break;

            //Case 4: screen recap points and to next round (if deckRound empty)
            case 4:
                if (pointsCalculated)
                {
                    break;
                }

                c2Displayed = false;
                pointsCalculated = true;
                mainUIController.RecapPoints(mancheNumber, teamsInGame,deck);

                //Prepare for next round, reset chrono
                ResetDeckRoundWin();
                ShuffleDeck(deck);
                RefillDeckRound();
                ChangeTeamRound();
                chrono = baseChrono;
                
                break;

            //Case 5: only after round 3, screen victory screen
            case 5:
                if (pointsCalculated)
                {
                    break;
                }

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

    //Increase Round number, if we are at the round3 just go to victory screen; else go back to rules screen
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
    }

    private void ChangeTeamRound()
    {
        //State 2's Timer is 0 and deckRound is not empty, so it's at an other team to play
        chrono = baseChrono;
        launchedTimer = !launchedTimer;
        cardIndex = 0;

        //With X équipes, ID's will be from 0 to X-1
        //So Team 1 index's is actually 0
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
        //State2 button's skip call MainUIController.SkipCard() which call this ftc
        //display the next card in the deckRound List
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
        //State2 button's check card call MainUIController.CheckCard() which call this ftc
        //When a card is checked, we save the current teamIndex on the appropriate cardBP's field
        //For example if team 2 (teamIndex=1) check a card in round 2
        //We save 1 on the M2_win field's card

        //Then we add this card to deckRoundWin and remove it from deckRound
        //We check cardIndex is not out of bound

        //If deckRound is empty we return the actual round to MainUIController.CheckCard() in order to go to next round
        //Else we return 0
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

    //Clear deckRound then clone it from deck
    private void RefillDeckRound()
    {
        deckRound.Clear();
        deck.ForEach(val => { deckRound.Add(val); });
        deckRound.ForEach(val => Debug.Log(val.word));
    }
    
    //Clear deckRound
    private void ResetDeckRoundWin()
    {
        deckRoundWin.Clear();
    }


    //Reload the activeScene; called from state 5's relaunch button
    public void Relaunch()
    {
        //Reset score
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    #region DECK_BUILDING

    IEnumerator UserDetailsXmlPath(string filePath)
    {

        //filePath = "StreamingAssets/myFile.txt"; //This works because index.html is in the same folder as StreamingAssets ?

        Debug.Log("biz");

        UnityWebRequest uwr = UnityWebRequest.Get(filePath);

        //UnityWebRequestAsyncOperation async = uwr.SendWebRequest();

        uwr.SendWebRequest();

        Debug.Log("uwr request sent");

        while (!uwr.isDone) {
            Debug.Log("not done yet");
            yield return null;
        }

        Debug.Log("UWR1 : ");


        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log("Error: " + uwr.error);
            Debug.Log("Response Code: " + uwr.responseCode);
        }
        else
        {
            Debug.Log("load done");
            dictionnary =uwr.downloadHandler.text;
            

            //yield return uwr.isDone;
        }
         //yield return uwr.isDone;
    }

    IEnumerator WaitingXmlFile(string filePath)
    {
        Debug.Log("waiting...");

        yield return StartCoroutine(UserDetailsXmlPath(filePath));

        Debug.Log("waiting done");
    }

    IEnumerator NewDictionnaryRead()
    { 
        //string text;
        //Using BetterStreamingAssets plugin.
        //Precaution: keep all file names in StreamingAssets lowercase

        //BetterStreamingAssets.Initialize() done in Start().
        //All paths are relative to StreamingAssets directory. So we want "dictionnaries/en.xml"

        
        string path = "dictionnaries/"+GameSettings._langue+".xml";
        string webPath = Application.streamingAssetsPath + "/" + path;

        if (webPath.Contains("://")|| webPath.Contains(":///"))
        {
            
            Debug.Log("COUCOU");
            dictionnary = null;
            yield return StartCoroutine(WaitingXmlFile(webPath));

            Debug.Log("text: " + dictionnary);

        }
        else
        {
            byte[] data = BetterStreamingAssets.ReadAllBytes(path);
            dictionnary = Encoding.UTF8.GetString(data, 0, data.Length);
        }
    }

    string[] ParseFile()
    {
        char[] separators = { '\n' };
        string[] strValues = dictionnary.Split(separators);
        //We empty dictionnary in order to free memory space
        dictionnary = "";
        return strValues;
    }


    void ShuffleDeck(List<CardsBP> bp)
    {
        //Need to specify System.Random or compiler doesn't know if using System's or Unity's random
        var rand = new System.Random();

        var randomized = deck.OrderBy(item => rand.Next());

        List<CardsBP> listTemp = new List<CardsBP>();
        CardsBP temp;

        for (int i = 0; i < randomized.Count(); i++)
        {
            do
            {
                temp = randomized.ElementAt(i);
            } while (listTemp.Contains(temp));
            listTemp.Add(temp);
        }
        bp.Clear();
        listTemp.ForEach(val => { bp.Add(val); });

        listTemp.Clear();
    }

    #endregion DECK_BUILDING
}

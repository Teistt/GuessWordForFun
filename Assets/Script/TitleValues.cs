using UnityEngine;

public class TitleValues : MonoBehaviour
{
    private TitleGameUI titleGameUI;


    public int minTeamNumber ;
    public int maxTeamNumber ;
    public int minCardsNumber;
    public int maxCardsNumber;

    void Start()
    {
        titleGameUI = TitleGameUI.instance;

        minTeamNumber= titleGameUI._minTeamNumber;
        maxTeamNumber=titleGameUI._maxTeamNumber;
        minCardsNumber=titleGameUI._minCardsNumber;
        maxCardsNumber=titleGameUI._maxCardsNumber;
    }
}

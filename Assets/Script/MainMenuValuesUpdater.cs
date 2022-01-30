using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.GlobalVariables;

public class MainMenuValuesUpdater : MonoBehaviour
{
    private GameController gameController;

    //Used for global.teamNb variable
    private int lastTeamNb=0;
    private int currentTeamNb;

    //Used for global.round variable
    private int lastRound=0;
    private int currentRound;


    private void Start()
    {
        //reference gameController
        gameController = GameController.instance;

        //Set global.teamNb's variable
        currentTeamNb = gameController._teamIndex + 1;
        lastTeamNb = currentTeamNb;

        //Set global.round's variable
        currentRound = gameController._mancheNumber;
        lastRound = currentRound;

        // Get our GlobalVariablesSource
        var source = LocalizationSettings
            .StringDatabase
            .SmartFormatter
            .GetSourceExtension<GlobalVariablesSource>();

        // Get teamNb global variable
        var teamNb =
            source["global"]["teamNb"] as IntGlobalVariable;
        // Update teamNb global variable 
        teamNb.Value = currentTeamNb;

        // Get round global variable
        var round =
            source["global"]["round"] as IntGlobalVariable;
        // Update round global variable 
        round.Value = currentRound;
    }


    void Update()
    {
        currentTeamNb = gameController._teamIndex + 1;
        currentRound = gameController._mancheNumber;


        if (lastTeamNb != currentTeamNb)
        {
            lastTeamNb = currentTeamNb;


            // Get our GlobalVariablesSource
            var source = LocalizationSettings
                .StringDatabase
                .SmartFormatter
                .GetSourceExtension<GlobalVariablesSource>();
            // Get teamNb global variable
            var teamNb =
                source["global"]["teamNb"] as IntGlobalVariable;
            // Update teamNb global variable 
            teamNb.Value = currentTeamNb;
        }

        if (lastRound != currentRound)
        {
            lastRound = currentRound;


            // Get our GlobalVariablesSource
            var source = LocalizationSettings
                .StringDatabase
                .SmartFormatter
                .GetSourceExtension<GlobalVariablesSource>();
            // Get round global variable
            var round =
                source["global"]["round"] as IntGlobalVariable;
            // Update round global variable 
            round.Value = currentRound;
        }
        
    }
}

//This file will contain the necessary functions for the hard AI mode for HexControl
//Current section to work on: preventOpponentVictory method
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hardAI : MonoBehaviour
{
    TurnController tcontrol;
    LevelController levelcont;
    ActionController actions;
    Player AIPlayer;

    private void Awake()
    {
        tcontrol = GameObject.Find("GameController").GetComponent<TurnController>();
        levelcont = GameObject.Find("LevelController").GetComponent<LevelController>();
        actions = GameObject.Find("GameController").GetComponent<ActionController>();
        AIPlayer = this.GetComponent<Player>();
    }

    public void hardAIAction()
    {
        bool ActionSelected = false; //Keep a running track of whether or not we have arrived at a decision
        ActionSelected = winningMove(); //If a winning move is possible, then it will be carried out and ActionSelected will be set to true
        if(ActionSelected == true)
        {
            Debug.Log("AI played a winning move");
        }
        else
        {
            //If the winning move did not succeed then we need to carry on down the tree
            Debug.Log("Winning move not available at this time");
            //Do the next thing
            ActionSelected = PreventOpponentVictory(); //If the AI can stop another player from winning, then it will do so and ActionSelected will be set to true
        }
        if(ActionSelected == true)
        {
            Debug.Log("AI attempted to prevent opponent victory");
        }
        else
        {
            //Carry on down the decision tree
            ActionSelected = ApproachingEndGame(); //If the AI thinks that it can win in less than 5 turns it will pursue a more aggressive End game strategy where it is purely focussed on the victory condition
        }
        if (ActionSelected == true)
        {
            Debug.Log("AI tried end game mode action");
        }
        else
        {
            ActionSelected = NormalGameLogicSequence();
        }
        if (ActionSelected == true)
        {
            Debug.Log("AI takes normal action");
        }
        else
        {
            Debug.Log("No action taken by AI");
        }
    }

    private bool tryLobbyAction()
    {
        bool actionTaken = false;
        int repealIndex = CheckForPersistentNegativeEffects();

        if(repealIndex == -1) //If nothing worth repealing, continue onwards
        {
            int actionToTake = chooseLobbyAction();
            if (actionToTake == 0)
            {
                actionTaken = false;
            }
            else
            {
               actions.PerformLobbyAction(actionToTake, AIPlayer, levelcont.players);
                actionTaken = true;
            }
        }
        else
        {
            int randNum = Random.Range(1, 7); //Roll a d6
            if (randNum <= 3)
            {
                actions.RepealEdict(AIPlayer, repealIndex); //If a suitable edict has been found to repeal then repeal it
                actionTaken = true;
            }
            else
            {
                int actionToTake = chooseLobbyAction();
                if (actionToTake == 0)
                {
                    actionTaken = false;
                }
                else
                {
                    actions.PerformLobbyAction(actionToTake, AIPlayer, levelcont.players);
                    actionTaken = true;
                }
            }
        }
        return actionTaken;
    }

    private int chooseLobbyAction() //Here we select a lobby action at random, check if it is worth doing
    {
        int lobbyAction = 0; //If lobbyAction == 0, no action is performed. We start with this value to ensure that nothing weird is likely to happen
        int[] lobbyingOptionsAvailable = levelcont.lobbyingOptions;
        bool[] attempted = new bool[4] {false,false,false,false};
        int triedCount = 0;
        bool finished = false;
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("player");
        while (finished == false)
        {
            int randomIndex = Random.Range(1, 5); //Four actions available that are not repeal and repeal is at 0
            if (attempted[randomIndex-1] == false)
            {
                bool success = false;
                attempted[randomIndex-1] = true;
                triedCount++;
                if (lobbyingOptionsAvailable[randomIndex] == 1) 
                {
                    Debug.Log("testing if Green Energy Initiative might be useful");
                    //green energy initiative
                    int NpowOpponent = 0;
                    foreach(GameObject playerObj in allPlayers)
                    {
                        if(playerObj.GetComponent<Player>().tileCounts[3] > NpowOpponent)
                        {
                            NpowOpponent = playerObj.GetComponent<Player>().tileCounts[3];
                        }
                    }
                    if(AIPlayer.tileCounts[3] < 2 && NpowOpponent >= 2 && AIPlayer.playerInfluence > 100)
                    {
                        success = true;
                        lobbyAction = 1;
                    }
                    else
                    {
                        success = false;
                    }
                }
                else if (lobbyingOptionsAvailable[randomIndex] == 2)
                {
                    Debug.Log("testing if Power to the People might be useful");
                    //powertothepeople
                    int NpowResOpponent = 0; //A measure of the value of residential districts with associated power plants
                    int NpowIndOpponent = 0; //A measure of the value of industrial districts with associated power plants
                    int NpowResPlayer = AIPlayer.tileCounts[1] * AIPlayer.tileCounts[3];
                    int NpowIndPlayer = AIPlayer.tileCounts[2] * 3 * AIPlayer.tileCounts[3];
                    foreach(GameObject playerObj in allPlayers)
                    {
                        Player player = playerObj.GetComponent<Player>();
                        int tempValNum = player.tileCounts[1] * player.tileCounts[3];
                        if(tempValNum > NpowResOpponent)
                        {
                            NpowResOpponent = tempValNum;
                            tempValNum = 0;
                        }
                        tempValNum = player.tileCounts[2] * 3 * player.tileCounts[3]; //Assert that industrial zones are three times as valuable as residential
                        if(tempValNum > NpowIndOpponent)
                        {
                            NpowIndOpponent = tempValNum;
                            tempValNum = 0;
                        }
                        if(tcontrol.powerToThepeople != true && ((NpowIndOpponent > NpowIndPlayer && NpowResOpponent <= NpowResPlayer) || (NpowResPlayer > NpowResOpponent)))
                        {
                            success = true;
                            lobbyAction = 2;
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }
                else if (lobbyingOptionsAvailable[randomIndex] == 3)
                {
                    //rule of law
                    Debug.Log("testing if Rule of Law mgiht be useful");
                    int maxNCivOpponent = 0;
                    foreach(GameObject playerObj in allPlayers)
                    {
                        Player player = playerObj.GetComponent<Player>();
                        if(player.tileCounts[4] > maxNCivOpponent && player.playerNumber != AIPlayer.playerNumber) //If player has more Civics and is not the current AI player
                        {
                            maxNCivOpponent = player.tileCounts[4];
                        }
                    }
                    if(maxNCivOpponent+1 < AIPlayer.tileCounts[4] && tcontrol.ruleOfLaw != true) //If the AIplayer has at least 2 more Civic buildings than anyone else then it is worth using Rule Of Law
                    {
                        success = true;
                        lobbyAction = 3;
                    }
                    else
                    {
                        success = false;
                    }
                }
                else if (lobbyingOptionsAvailable[randomIndex] == 4)
                {
                    Debug.Log("testing if bribes as industry is worthwhile");
                    //bribes as industry. This can use the same logic as Rule of Law
                    int maxNCivOpponent = 0;
                    foreach (GameObject playerObj in allPlayers)
                    {
                        Player player = playerObj.GetComponent<Player>();
                        if (player.tileCounts[4] > maxNCivOpponent && player.playerNumber != AIPlayer.playerNumber) //If player has more Civics and is not the current AI player
                        {
                            maxNCivOpponent = player.tileCounts[4];
                        }
                    }
                    if (maxNCivOpponent < AIPlayer.tileCounts[4] && tcontrol.ruleOfLaw != true) //If the AIplayer has at least 2 more Civic buildings than anyone else then it is worth using Rule Of Law
                    {
                        success = true;
                        lobbyAction = 4;
                    }
                    else
                    {
                        success = false;
                    }
                }
                else if (lobbyingOptionsAvailable[randomIndex] == 5)
                {
                    //MoveGoalposts
                    //This will need to be the same logic as PreventOpponentVictory
                    int winCondition = levelcont.VictoryCondition;
                    bool opponentWinPossible = false;
                    if (winCondition == 0)
                    {
                        int distanceToVictory = 1000; //
                                                      //Victory condition is to reach target influence
                        foreach (GameObject playerObject in allPlayers)
                        {
                            Player playerComponent = playerObject.GetComponent<Player>();
                            if (playerComponent.playerNumber != AIPlayer.playerNumber && (levelcont.TargetInfluence - playerComponent.playerInfluence) < distanceToVictory && (playerComponent.playerInfluence + tcontrol.CalculateInfIncrease(playerComponent.playerNumber)) >= levelcont.TargetInfluence)
                            {
                                //If the playerComponent does not belong to the target player AND the distance to victory is lower than the current stored value AND the player in question is on track to win
                                opponentWinPossible = true;
                                distanceToVictory = levelcont.TargetInfluence - playerComponent.playerInfluence; //Set the new distance to Victory (this will ensure that the player closest to winning will be targeted)
                                Debug.Log("AI has decided that player will win soon");
                            }
                            else
                            {
                                Debug.Log("Player does not appear to be about to win");
                            }
                        }
                    }
                    else if (winCondition == 1) //If win condition is controlling the board
                    {
                        int distanceToVictory = 1000; //
                        foreach (GameObject playerObject in allPlayers)
                        {
                            Player playerComponent = playerObject.GetComponent<Player>();
                            int NplayerTerritories = playerComponent.tileCounts[0] + playerComponent.tileCounts[1] + playerComponent.tileCounts[2] + playerComponent.tileCounts[3] + playerComponent.tileCounts[4] + playerComponent.tileCounts[5];
                            if (NplayerTerritories - 3 >= levelcont.TargetCells && playerComponent.playerNumber != AIPlayer.playerNumber && levelcont.TargetCells - NplayerTerritories <= distanceToVictory)
                            {
                                //If the playerComponent does not belong to the target player AND the distance to victory is lower than the current stored value AND the player in question only needs 3 or fewer more districts to win the game
                                opponentWinPossible = true;
                                distanceToVictory = levelcont.TargetCells - NplayerTerritories; //Set the new distance to Victory (this will ensure that the player closest to winning will be targeted)
                            }
                            else
                            {
                                Debug.Log("Player does not appear to be about to win");
                            }
                        }
                    }
                    else if(winCondition == 2)
                    {
                        int distanceToVictory = 10000; //
                                                       //Victory condition is to reach target influence
                        foreach (GameObject playerObject in allPlayers)
                        {
                            Player playerComponent = playerObject.GetComponent<Player>();
                            if (playerComponent.playerNumber != AIPlayer.playerNumber && (levelcont.TargetCash - playerComponent.playerCash) < distanceToVictory && (playerComponent.playerCash + tcontrol.CalculateCashIncrease(playerComponent.playerNumber)) >= levelcont.TargetCash)
                            {
                                //If the playerComponent does not belong to the target player AND the distance to victory is lower than the current stored value AND the player in question is on track to win
                                opponentWinPossible = true;
                                distanceToVictory = levelcont.TargetCash - playerComponent.playerCash; //Set the new distance to Victory (this will ensure that the player closest to winning will be targeted)
                            }
                            else
                            {
                                Debug.Log("Player does not appear to be about to win");
                            }
                        }
                    }
                    else if(winCondition == 3)
                    {
                        foreach (GameObject playerObject in allPlayers)
                        {
                            Player playerComponent = playerObject.GetComponent<Player>();
                            if (playerComponent.playerNumber != AIPlayer.playerNumber && playerComponent.tileCounts[5] >= levelcont.TargetLandmarks - 1)
                            {
                                opponentWinPossible = true;
                            }
                            else
                            {
                                Debug.Log("Player does not appear to be about to win");
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Error in hardAI.cs::ChooseLobbyAction -- choice 5: Victory condition is not recognised!");
                        success = false;
                        opponentWinPossible = false;
                    }
                    if (opponentWinPossible == true)
                    {
                        //If the AI thinks an opponent might win then it is worthwhile changing the victory condition
                        success = true;
                        lobbyAction = 5;
                    }
                    else
                    {
                        success = false;
                    }

                }
                else if (lobbyingOptionsAvailable[randomIndex] == 6)
                {
                    //Rent hike
                    Debug.Log("testing whether Rent Hike is worthwhile");
                    int winCondition = levelcont.VictoryCondition;
                    bool goodIdea = false;
                    int maxResOpponent = 0;
                    foreach (GameObject playerObj in allPlayers)
                    {
                        Player player = playerObj.GetComponent<Player>();
                        if (player.tileCounts[1] > maxResOpponent)
                        {
                            maxResOpponent = player.tileCounts[1];
                        }
                    }
                    if(winCondition == 0 && AIPlayer.tileCounts[1] < maxResOpponent - 3) //If influence victory and the current player has much fewer residences than another player
                    {
                        goodIdea = true; //Then Rent hike might be a good idea
                    }
                    else if (winCondition == 0)
                    {
                        goodIdea = false; //otherwise if the winCondition==0, Rent hike is probably a bad idea
                    }
                    else if(AIPlayer.tileCounts[1] > maxResOpponent + 1) //If player has at least 2 more Residential districts than any other player
                    {
                        goodIdea = true;
                    }
                    else
                    {
                        goodIdea = false;
                    }
                    if(goodIdea == true && tcontrol.rentHike != true)
                    {
                        success = true;
                        lobbyAction = 6;
                    }
                    else
                    {
                        success = false;
                    }
                }
                else if (lobbyingOptionsAvailable[randomIndex] == 7)
                {
                    //decentralise government
                    Debug.Log("test decentralise government");
                    int winCondition = levelcont.VictoryCondition;
                    int maxNCivOpponent = 0;
                    foreach (GameObject playerObj in allPlayers)
                    {
                        Player player = playerObj.GetComponent<Player>();
                        if (player.tileCounts[4] > maxNCivOpponent && player.playerNumber != AIPlayer.playerNumber) //If player has more Civics and is not the current AI player
                        {
                            maxNCivOpponent = player.tileCounts[4];
                        }
                    }
                    if (winCondition != 0 && maxNCivOpponent < AIPlayer.tileCounts[4] && tcontrol.ruleOfLaw != true) //If the AIplayer has at least 2 more Civic buildings than anyone else then it is worth using Rule Of Law
                    {
                        success = true;
                        lobbyAction = 7;
                    }
                    else if (winCondition == 0 && maxNCivOpponent > AIPlayer.tileCounts[4] +1  && tcontrol.ruleOfLaw != true)
                    {
                        success = true;
                        lobbyAction = 7;
                    }
                    else
                    {
                        success = false;
                    }
                }
                else if (lobbyingOptionsAvailable[randomIndex] == 8)
                {
                    //office refurbishment
                    Debug.Log("Checking if office refurbishment is worthwhile");
                    int maxNCivOpponent = 0;
                    foreach (GameObject playerObj in allPlayers)
                    {
                        Player player = playerObj.GetComponent<Player>();
                        if (player.tileCounts[4] > maxNCivOpponent && player.playerNumber != AIPlayer.playerNumber) //If player has more Civics and is not the current AI player
                        {
                            maxNCivOpponent = player.tileCounts[4];
                        }
                    }
                    if ((AIPlayer.tileCounts[4] == 0 && maxNCivOpponent > 0) || AIPlayer.tileCounts[4] < maxNCivOpponent - 2)
                    {
                        success = true;
                        lobbyAction = 8;
                    }
                    else
                    {
                        success = false;
                    }
                }
                else if (lobbyingOptionsAvailable[randomIndex] == 9)
                {
                    //free housing initiative
                    Debug.Log("testing free housing initiative.");
                    int winCondition = levelcont.VictoryCondition;
                    int maxNResOpponent = 0;
                    foreach (GameObject playerObj in allPlayers)
                    {
                        Player player = playerObj.GetComponent<Player>();
                        if (player.tileCounts[1] > maxNResOpponent && player.playerNumber != AIPlayer.playerNumber) //If player has more Civics and is not the current AI player
                        {
                            maxNResOpponent = player.tileCounts[1];
                        }
                    }
                    if((winCondition == 0 && AIPlayer.tileCounts[1] > maxNResOpponent + 1) || (winCondition !=0 && AIPlayer.tileCounts[1] < maxNResOpponent-2))
                    {
                        success = true;
                        lobbyAction = 9;
                    }
                    else
                    {
                        success = false;
                    }
                }
                else
                {
                    Debug.Log("Error in hardAI::chooseLobbyAction : lobby action index not recognised.");
                }
                if (triedCount == 4 || success == true)
                {
                    finished = true;
                }
            }
        }
        if (lobbyAction == 0)
        {
            Debug.Log("No suitible lobby action found");
        }
        return lobbyAction;
    }

    private int CheckForPersistentNegativeEffects()
    {
        //This will check for effects of active edicts that negatively impact the AI player and if so, returns the index of the action to repeal
        int index = -1;
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("player");
        if (tcontrol.powerToThepeople == true)
        {
            int maxResidential = 0;
            foreach(GameObject player in allPlayers)
            {
                Player playerComponent = player.GetComponent<Player>();
                if(playerComponent.playerNumber != AIPlayer.playerNumber && playerComponent.tileCounts[1] > maxResidential && playerComponent.tileCounts[3] > 0)
                {
                    maxResidential = playerComponent.tileCounts[1];
                }
            }
            if(maxResidential > AIPlayer.tileCounts[1])
            {
                index = 2;
            }
        }
        else if(tcontrol.ruleOfLaw == true)
        {
            foreach(GameObject player in allPlayers)
            {
                Player playerComponent = player.GetComponent<Player>();
                if(playerComponent.tileCounts[4] > AIPlayer.tileCounts[4])
                {
                    index = 3;
                }
            }
        }
        else if(tcontrol.bribesAsIndustry == true)
        {
            foreach (GameObject player in allPlayers)
            {
                Player playerComponent = player.GetComponent<Player>();
                if (playerComponent.tileCounts[4] > AIPlayer.tileCounts[4])
                {
                    index = 4;
                }
            }
        }
        else if (tcontrol.rentHike == true)
        {
            int winCondition = levelcont.VictoryCondition;
            if(winCondition == 0)
            {
                int nresmax = 0;
                foreach (GameObject player in allPlayers)
                {
                    Player playerComponent = player.GetComponent<Player>();
                    if(playerComponent.tileCounts[1] > nresmax && playerComponent.playerNumber != AIPlayer.playerNumber)
                    {
                        nresmax = playerComponent.tileCounts[1];
                    }
                }
                if(nresmax < AIPlayer.tileCounts[1])
                {
                    index = 6;
                }
                else
                {
                    index = -1;
                }
            }
            else 
            {
                int nresmax = 0;
                foreach (GameObject player in allPlayers)
                {
                    Player playerComponent = player.GetComponent<Player>();
                    if(playerComponent.tileCounts[1] > nresmax && playerComponent.playerNumber != AIPlayer.playerNumber)
                    {
                        nresmax = playerComponent.tileCounts[1];
                    }
                }
                if(nresmax > AIPlayer.tileCounts[1])
                {
                    index = 6;
                }
                else
                {
                    index = -1;
                }
            }
        }
        else if(tcontrol.freeHousingInitiative == true)
        {
            int randnum = Random.Range(1, 7);
            if (randnum <= 3)
            {
                index = 7;
            }
            else
            {
                index = -1;
            }
        }
        else if (tcontrol.decentraliseGovernment == true)
        {
            int randnum = Random.Range(1, 7);
            if (randnum <= 3)
            {
                index = 7;
            }
            else
            {
                index = -1;
            }
        }
        else
        {
            index = -1;
        }
        return index;
    }

    private bool NormalGameLogicSequence()
    {
        bool ActionTaken = false;
        bool contbool = false;
        int winCondition = levelcont.VictoryCondition;
        if (winCondition == 0)
        {
            //If win condition is influence
            if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,5)) && tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) + levelcont.CashPerLan >= 200 && CountAvailableCellsOfType(AIPlayer,5) > 0)
            {          
                Debug.Log("Attempting to buy Landmark (NormalGameLogicSequence:winCondition==0:Step1)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 5);
            }
            else if (AIPlayer.playerInfluence > 100)
            {
                int randnum = Random.Range(1, 7); //Roll a D6
                if (randnum < 2)
                {
                    Debug.Log("Attempting to take lobby action (NormalGameLogicSequence:winCondition==0:Step2)");
                    ActionTaken = tryLobbyAction();
                    if(ActionTaken == false)
                    {
                        Debug.Log("Not taking lobby action (NormalGameLogicSequence:winCondition==0:Step2)");
                        contbool = true; //If we try to take a lobby action but none are suitable, we continue along the decision tree.
                    }
                }
                else
                {
                    Debug.Log("Not taking lobby action (NormalGameLogicSequence:winCondition==0:Step2)");
                    contbool = true;
                }
            }
            else
            {
                Debug.Log("Not taking lobby action (NormalGameLogicSequence:winCondition==0:Step2)");
                contbool = true;
            }
            if (contbool == true)
            {
                contbool = false;
                //Now we have gone out of the randomised section and we only have to type out the alternative route once.
                Debug.Log("AI player cash: ");
                Debug.Log(AIPlayer.playerCash);
                if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 4)) && CountAvailableCellsOfType(AIPlayer, 4) > 0)
                {
                    Debug.Log("Attempting to buy Civic district (NormalGameLogicSequence:winCondition==0:Step3)");
                    ActionTaken = BuyBestcellOfType(AIPlayer, 4);
                }
                else if(AIPlayer.tileCounts[2] >= 2 && AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,3)) && CountAvailableCellsOfType(AIPlayer, 3) > 0)
                {
                    Debug.Log("Attempting to buy power plant district (NormalGameLogicSequence:winCondition==0:Step4)");
                    ActionTaken = BuyBestcellOfType(AIPlayer, 3);
                }
                else if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 2)) && CountAvailableCellsOfType(AIPlayer, 2) > 0 && (AIPlayer.tileCounts[2] <2 || tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) > Mathf.Abs(levelcont.InfPerInd) + 10))
                {
                    Debug.Log("Attempting to buy Industrial district (NormalGameLogicSequence:winCondition==0:Step5)");
                    ActionTaken = BuyBestcellOfType(AIPlayer, 2);
                }
                else if (AIPlayer.playerCash >=minimumCellCost(FindAvailableCellsOfType(AIPlayer, 1)) && CountAvailableCellsOfType(AIPlayer, 1) > 0)
                {
                    Debug.Log("Attempting to buy Residential district (NormalGameLogicSequence:winCondition==0:Step6)");
                    ActionTaken = BuyBestcellOfType(AIPlayer, 1);
                }
                else if (AIPlayer.playerInfluence >= 100)
                {
                    Debug.Log("Attempting to take Lobby action (NormalGameLogicSequence:winCondition==0:Step7)");
                    ActionTaken = tryLobbyAction();
                }
                else
                {
                    //Do nothing as all possible actions have been attempted.
                    Debug.Log("Doing nothing (NormalGameLogicSequence:winCondition==0:step7)");
                    Debug.Log("No viable actions to take");
                    ActionTaken = true;
                }
            }
        }
        else if(winCondition == 1)
        {
            //If win condition is control of board
            if (AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) <=  500) //If player is making less than 500 cash per turn, focus on making more cash
            {
                if (AIPlayer.tileCounts[2] >= 2 && AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 3)) && CountAvailableCellsOfType(AIPlayer, 3) > 0) //If buying a power plant makes sense, then try to do that
                {
                    Debug.Log("Attempting to buy power plant district (NormalGameLogicSequence:winCondition==1:Step1,1)");
                    ActionTaken = BuyBestcellOfType(AIPlayer, 3);
                }
                else if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 2)) && CountAvailableCellsOfType(AIPlayer,2) > 0) //If player can afford to buy an industrial district
                {
                    Debug.Log("Attempting to buy industrial district (NormalGameLogicSequence:winCondition==1:Step1,2)");
                    ActionTaken = BuyBestcellOfType(AIPlayer, 2);
                }
                else if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,1)) && CountAvailableCellsOfType(AIPlayer,1) > 0) //If player can buy residential
                {
                    Debug.Log("Attempting to buy residential district (NormalGameLogicSequence:winCondition==1,step1,3)");
                    ActionTaken = BuyBestcellOfType(AIPlayer, 1);
                }
                else if(AIPlayer.playerInfluence >= 100)
                {
                    Debug.Log("Attempting lobby action (NormalGameLogicSequence:winCondition==1,step1,4)");
                    ActionTaken = tryLobbyAction();
                }
                else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCells(AIPlayer)) && FindAvailableCells(AIPlayer).Count > 0)
                {
                    Debug.Log("Try to buy any cell (NormalGameLogicSequence:winCondition==1,step1,5)");
                    ActionTaken = BuyBestCellOfAnyType(AIPlayer);
                }
                else
                {
                    Debug.Log("No action taken: no viable action found. (NormalGameLogicSequence:winCondition==1,step1,6)");
                }
            }
            else
            {
                //Else try to buy up cells prioritising the cheapest ones
                if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCells(AIPlayer)) && FindAvailableCells(AIPlayer).Count > 0)
                {
                    Debug.Log("Try to buy any cell (NormalGameLogicSequence:winCondition==1,step2,1)");
                    ActionTaken = BuyBestCellOfAnyType(AIPlayer);
                }
                else if (AIPlayer.playerInfluence >= 100)
                {
                    Debug.Log("Attempting lobby action (NormalGameLogicSequence:winCondition==1,step2,2)");
                    ActionTaken = tryLobbyAction();
                }
                else
                {
                    Debug.Log("No action taken: no viable action found. (NormalGameLogicSequence:winCondition==1,step2,3)");
                }
            }
        }
        else if (winCondition == 2)
        {
            //If win condition is target cash
            if(tcontrol.powerToThepeople == true && AIPlayer.tileCounts[1] >= 5 && AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,3)) && CountAvailableCellsOfType(AIPlayer,3)>0) 
            {
                Debug.Log("Attempting to buy power plant district (NormalGameLogicSequence:winCondition==2,step1)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 3);
            }
            else if(AIPlayer.tileCounts[2] >= 3 && AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,3))&& CountAvailableCellsOfType(AIPlayer, 3) > 0)
            {
                Debug.Log("Attempting to buy power plant district (NormalGameLogicSequence:winCondition==2,step2)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 3);
            }
            //If can afford industrial district
            else if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,2))&& CountAvailableCellsOfType(AIPlayer,2) > 0)
            {
                Debug.Log("Attempting to buy industrial district (NormalGameLogicSequence:winCondition==2,step3)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 2);
            }
            else if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 1)) && CountAvailableCellsOfType(AIPlayer, 1) > 0)
            {
                Debug.Log("Attempting to buy residential district (NormalGameLogicSequence:winCondition==2,step4)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 1);
            }
            else if (AIPlayer.playerInfluence >= 100)
            {
                Debug.Log("Attempting lobby action (NormalGameLogicSequence:winCondition==2,step5)");
                ActionTaken = tryLobbyAction();
            }
            else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,4)) + levelcont.CashPerCiv && CountAvailableCellsOfType(AIPlayer,4) > 0 && tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) >= levelcont.CashPerCiv + 100)
            {
                Debug.Log("Attempting to buy Civic district (NormalGameLogicSequence:winCondition==2,step6)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 4);
            }
            else
            {
                Debug.Log("No action taken: no viable action found. (NormalGameLogicSequence:winCondition==2,step7)");
            }
        }
        else if (winCondition == 3)
        {
            //If win condition is to get 3 landmarks
            //Try to buy a landmark and if that is not possible then need to prioritise making more money
            if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 5)) && tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) >= levelcont.CashPerLan + 100 && CountAvailableCellsOfType(AIPlayer, 5) > 0)
            {
                Debug.Log("Attempting to buy landmark (NormalGameLogicSequence:winCondition==3,step1)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 5);
            }
            else if (tcontrol.powerToThepeople == true && AIPlayer.tileCounts[1] >= 5 && AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 3)) && CountAvailableCellsOfType(AIPlayer,3) > 0)
            {
                Debug.Log("Attempting to buy power plant district (NormalGameLogicSequence:winCondition==3,step2)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 3);
            }
            else if (AIPlayer.tileCounts[2] >= 3 && AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 3)) && CountAvailableCellsOfType(AIPlayer, 3) > 0)
            {
                Debug.Log("Attempting to buy power plant district (NormalGameLogicSequence:winCondition==3,step3)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 3);
            }
            else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,2)) && CountAvailableCellsOfType(AIPlayer, 2) >0)
            {
                Debug.Log("Attempting to buy industrial district (NormalGameLogicSequence:winCondition==3,step4)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 2);
            }
            else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 1)) && CountAvailableCellsOfType(AIPlayer,1) > 0)
            {
                Debug.Log("Attempting to buy industrial district (NormalGameLogicSequence:winCondition==3,step5)");
                ActionTaken = BuyBestcellOfType(AIPlayer, 1);
            }
            else if (AIPlayer.playerInfluence >= 100)
            {
                Debug.Log("Attempting lobby action (NormalGameLogicSequence:winCondition==3,step6)");
                ActionTaken = tryLobbyAction();
            }
            else
            {
                Debug.Log("No Action taken. No viable actions found (NormalGameLogicSequence:winCondition==3,step7)");
            }
            //If power plant makes sense
            //If can buy residential
            //If can lobby
            //Else do nothing
        }
        else
        {
            Debug.Log("Error in hardAI.cs:NormalGameLogicSequence :: Unrecognised victory condition");
        }
        return ActionTaken;
    }

    private bool ApproachingEndGame()
    {
        //This method will check to see whether the AI thinks that it is close to winning the game and, if so, it will pursue an aggressive strategy aimed solely on getting as close to victory as possible
        int endGameCriterion = 5; //The number of turns in which the AI will win excluding actions and opposing players
        bool ActionTaken = false;
        bool nearEndGame = false;
        int winCondition = levelcont.VictoryCondition;
        if (winCondition == 0)
        {
            //If win condition is influence victory
            if (AIPlayer.playerInfluence + (tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) * endGameCriterion) >= levelcont.TargetInfluence)
            {
                //If the AIplayer will win in the number of turns in the endGameCriterion parameter on the condition that nothing happens, then we go into end game strategy mode
                Debug.Log("AI has decided that end game is near");
                nearEndGame = true;
            }
        }
        else if (winCondition == 1)
        {
            //If win condition is controlling the board
            int sumPlayerCells = AIPlayer.tileCounts[0] + AIPlayer.tileCounts[1] + AIPlayer.tileCounts[2] + AIPlayer.tileCounts[3] + AIPlayer.tileCounts[4] + AIPlayer.tileCounts[5];
            if (sumPlayerCells + (endGameCriterion*2) >= levelcont.TargetCells)
            {
                //Assuming the player is able to buy at least 2 cells per turn then we go into near end game mode
                nearEndGame = true;
            }
        }
        else if (winCondition == 2)
        {
            //If win condition is Cash victory
            if (AIPlayer.playerCash + (tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) * endGameCriterion) >= levelcont.TargetCash)
            {
                nearEndGame = true;
            }
        }
        else if (winCondition == 3)
        {
            //If win condition is landmark victory
            if (AIPlayer.tileCounts[5] >= 1 && AIPlayer.playerCash >= cheapestCell(FindAvailableCellsOfType(AIPlayer,5)).GetComponent<HexCell>().cellPrice)
            {
                nearEndGame = true;
            }
        }
        else
        {
            nearEndGame = false;
        }
        if (nearEndGame == true)
        {
            //If the AI thinks it can win soon then use the following logic
            if (winCondition == 0)
            {
                //If can, buy landmark
                if(AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,5)))
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 5);
                }
                //If can, buy Civic
                else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,4)))
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 4);
                }
                //If can, buy Residential
                else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 1)))
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 1);
                }
                else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 3)) && tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) <= 200)
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 3);
                }
                else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer,2)) && tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) <= 200)
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 2);
                }
                else
                {
                    //Do nothing
                    Debug.Log("No viable action to take.");
                    ActionTaken = true;
                }
            }
            else if (winCondition == 1)
            {
                //Buy the cheapest available cell
                List<GameObject> possibleCells = FindAvailableCells(AIPlayer);
                if (possibleCells.Count > 0)
                {
                    ActionTaken = BuyBestCellOfAnyType(AIPlayer);
                }    
            }
            else if (winCondition == 2)
            {
                //If can, buy Industrial
                if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 2)))
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 2);
                }
                else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 3)))
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 3);
                }
                else if (AIPlayer.playerCash >= minimumCellCost(FindAvailableCellsOfType(AIPlayer, 1)))
                {
                    ActionTaken = BuyBestcellOfType(AIPlayer, 1);
                }
                else if (AIPlayer.playerInfluence >= 100)
                {
                    ActionTaken = tryLobbyAction();
                    if(ActionTaken == false)
                    {
                        if (AIPlayer.tileCounts[5] > 0)
                        {
                            ActionTaken = SellBestCellOfType(AIPlayer, 5);
                        }
                        else if (AIPlayer.tileCounts[4] > 0)
                        {
                            ActionTaken = SellBestCellOfType(AIPlayer, 4);
                        }
                        else
                        {
                            ActionTaken = false;
                        }
                    }
                    else
                    {
                        Debug.Log("Lobbying action attempted");
                    }
                }
                else if (AIPlayer.tileCounts[5] > 0)
                {
                    ActionTaken = SellBestCellOfType(AIPlayer, 5);
                }
                else if (AIPlayer.tileCounts[4] > 0)
                {
                    ActionTaken = SellBestCellOfType(AIPlayer, 4);
                }
                else
                {
                    //Do nothing
                    Debug.Log("No viable action to take.");
                    ActionTaken = true;
                }
            }
            else if (winCondition == 3)
            {
                //Buy the cheapest available landmark cell
                ActionTaken = BuyBestcellOfType(AIPlayer,5);
            }
            else
            {
                Debug.Log("Error in hardAI.cs:ApproachingEndGame :: Victory condition not recognised, aborting action");
                ActionTaken = false;
            }
        }
        else
        {
            //If the AI does not think it can win soon then it does not take an action and instead exits this method to carry on down the decision tree
            ActionTaken = false;
        }
        return ActionTaken; //Return a bool stating whether or not the AI has carried out an action
    }

    
    //This method will test whether an opponent may win this turn and, if so, try to do something about it
    private bool PreventOpponentVictory()
    {
        //For each victory condition:
        //First we need to get a list of all other players
        //Then see if any of those players are about to win
        //Then decide how to prevent that from happening
        int winCondition = levelcont.VictoryCondition;
        bool takenAction = false;
        bool opponentWinPossible = false;
        Player targetPlayer = AIPlayer; //Declaring a Player component without a value may be dangerous so I will actually set the target player to be the current player for now because then it will ensure that nothing is likely to go seriously wrong
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("player");
        if (winCondition == 0)
        {
            int distanceToVictory = 1000; //
            //Victory condition is to reach target influence
            foreach(GameObject playerObject in allPlayers)
            {
                Player playerComponent = playerObject.GetComponent<Player>();
                if(playerComponent.playerNumber != AIPlayer.playerNumber && (levelcont.TargetInfluence - playerComponent.playerInfluence) < distanceToVictory && (playerComponent.playerInfluence + tcontrol.CalculateInfIncrease(playerComponent.playerNumber)) >= levelcont.TargetInfluence)
                {
                    //If the playerComponent does not belong to the target player AND the distance to victory is lower than the current stored value AND the player in question is on track to win
                    opponentWinPossible = true;
                    distanceToVictory = levelcont.TargetInfluence - playerComponent.playerInfluence; //Set the new distance to Victory (this will ensure that the player closest to winning will be targeted)
                    targetPlayer = playerComponent; //Set the target player
                    Debug.Log("AI has decided that player will win soon:");
                    Debug.Log(targetPlayer);
                }
                else
                {
                    Debug.Log("Player does not appear to be about to win");
                }
            }
            if (opponentWinPossible == true)
            {
                //If an opponent has been found that might win we will try to take some action
                //Need to get the cheapest available cells of each type for the target player
                List<GameObject> targetPlayerLandmarks = ownedCellsOfType(targetPlayer, 5);
                List<GameObject> targetPlayerCivics = ownedCellsOfType(targetPlayer, 4);
                List<GameObject> targetPlayerRes = ownedCellsOfType(targetPlayer, 1);
                if(targetPlayer.playerInfluence + tcontrol.CalculateInfIncrease(targetPlayer.playerNumber) - levelcont.InfPerLan < levelcont.TargetInfluence && targetPlayer.tileCounts[5] > 0 && cheapestCell(targetPlayerLandmarks).GetComponent<HexCell>().cellPrice <= AIPlayer.playerCash)
                {
                    //If the removing a landmark will prevent the player from winning and the target player owns a landmark and we can buy it, we do so.
                    takenAction = BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 5);
                }
                else if (targetPlayer.playerInfluence + tcontrol.CalculateInfIncrease(targetPlayer.playerNumber) - levelcont.InfPerCiv < levelcont.TargetInfluence && targetPlayer.tileCounts[4] > 0 && cheapestCell(targetPlayerCivics).GetComponent<HexCell>().cellPrice <= AIPlayer.playerCash)
                {
                    takenAction = BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 4);
                }
                else if (targetPlayer.playerInfluence + tcontrol.CalculateInfIncrease(targetPlayer.playerNumber) - levelcont.InfPerRes < levelcont.TargetInfluence && targetPlayer.tileCounts[1] > 0 && cheapestCell(targetPlayerCivics).GetComponent<HexCell>().cellPrice <= AIPlayer.playerCash)
                {
                    takenAction = BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 1);
                }
                else 
                {
                    takenAction = tryLobbyAction();
                }
            }
            else
            {
                takenAction = false;
            }
        }
        else if (winCondition == 1) //If win condition is controlling the board
        {
            int distanceToVictory = 1000; //
            //Victory condition is to reach target influence
            foreach(GameObject playerObject in allPlayers)
            {
                Player playerComponent = playerObject.GetComponent<Player>();
                int NplayerTerritories = playerComponent.tileCounts[0] + playerComponent.tileCounts[1] + playerComponent.tileCounts[2] + playerComponent.tileCounts[3] + playerComponent.tileCounts[4] + playerComponent.tileCounts[5];
                if(NplayerTerritories - 3 >= levelcont.TargetCells && playerComponent.playerNumber != AIPlayer.playerNumber && levelcont.TargetCells - NplayerTerritories <= distanceToVictory)
                {
                    //If the playerComponent does not belong to the target player AND the distance to victory is lower than the current stored value AND the player in question only needs 3 or fewer more districts to win the game
                    opponentWinPossible = true;
                    distanceToVictory = levelcont.TargetCells - NplayerTerritories; //Set the new distance to Victory (this will ensure that the player closest to winning will be targeted)
                    targetPlayer = playerComponent; //Set the target player
                }
                else
                {
                    Debug.Log("Player does not appear to be about to win");
                }
                if(opponentWinPossible == true && minimumCellCost(OwnedCellsAll(targetPlayer)) <= AIPlayer.playerCash)
                {
                    //If the opponent is likely to win soon, and the current player can afford to buy the cheapest cell of the target player then buy that cell
                    takenAction = BuyBestCellFromPlayer(AIPlayer, targetPlayer);
                }
                else
                {
                    //otherwise try something more drastic...
                    takenAction = tryLobbyAction();
                }
            }
        }
        else if (winCondition == 2) //If win condition is amassing 10000 cash
        {
            int distanceToVictory = 10000; //
            //Victory condition is to reach target influence
            foreach(GameObject playerObject in allPlayers)
            {
                Player playerComponent = playerObject.GetComponent<Player>();
                if(playerComponent.playerNumber != AIPlayer.playerNumber && (levelcont.TargetCash - playerComponent.playerCash) < distanceToVictory && (playerComponent.playerCash + tcontrol.CalculateCashIncrease(playerComponent.playerNumber)) >= levelcont.TargetCash)
                {
                    //If the playerComponent does not belong to the target player AND the distance to victory is lower than the current stored value AND the player in question is on track to win
                    opponentWinPossible = true;
                    distanceToVictory = levelcont.TargetCash - playerComponent.playerCash; //Set the new distance to Victory (this will ensure that the player closest to winning will be targeted)
                    targetPlayer = playerComponent; //Set the target player
                }
                else
                {
                    Debug.Log("Player does not appear to be about to win");
                }
            }
            if(opponentWinPossible == true)
            {
                //If the opponent might win...
                List<GameObject> targetPlayerInd = ownedCellsOfType(targetPlayer, 2);
                List<GameObject> targetPlayerPow = ownedCellsOfType(targetPlayer, 3);
                List<GameObject> targetPlayerRes = ownedCellsOfType(targetPlayer, 1);
                if(targetPlayer.playerCash + tcontrol.CalculateCashIncrease(targetPlayer.playerNumber) - levelcont.CashPerInd < levelcont.TargetCash && targetPlayer.tileCounts[2] > 0 && minimumCellCost(targetPlayerInd) <= AIPlayer.playerCash)
                {
                    //Buy opponent's industrial district if possible and it would help
                    takenAction = BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 2);
                }      
                else if (targetPlayer.playerCash + tcontrol.CalculateCashIncrease(targetPlayer.playerNumber) - levelcont.CashPerRes < levelcont.TargetCash && targetPlayer.tileCounts[1] > 0 && minimumCellCost(targetPlayerRes) <= AIPlayer.playerCash)
                {
                    //Buy oppenent's residential district if possible and if it would help
                    takenAction = BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 1);
                }
                else if (targetPlayerPow.Count > 0)
                {
                    float PMult = 1.0f;
                    for(int i=0; i < targetPlayerPow.Count-1; i++)
                    {
                        //For each powerplant tile owned by the target player
                        PMult *= levelcont.PowerPlantMultiplier;
                    }
                    if (tcontrol.powerToThepeople == false && targetPlayer.playerCash + tcontrol.CalculateCashIncrease(targetPlayer.playerNumber) - (targetPlayerInd.Count * PMult) + (targetPlayerInd.Count * PMult / levelcont.PowerPlantMultiplier) < levelcont.TargetCash && targetPlayer.tileCounts[3] > 0 && minimumCellCost(targetPlayerPow) <= AIPlayer.playerCash)
                    {
                        //Buy a powerplant
                        BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 3);
                    }
                    else if (tcontrol.powerToThepeople == true && targetPlayer.playerCash + tcontrol.CalculateCashIncrease(targetPlayer.playerNumber) - (targetPlayerRes.Count * PMult) + (targetPlayerRes.Count * PMult / levelcont.PowerPlantMultiplier) < levelcont.TargetCash && targetPlayer.tileCounts[3] > 0 && minimumCellCost(targetPlayerPow) <= AIPlayer.playerCash)
                    {
                        BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 3);
                    }
                    else
                    {
                        //If buying a power plant wont help or isn't possible, try a lobbying action
                        takenAction = tryLobbyAction();
                    }
                }
                else
                {
                    //else try some lobbying
                    takenAction = tryLobbyAction();
                }   
            }
            else
            {
                //If the opponent probably won't win this turn
                takenAction = false;
            }
        }
        else if (winCondition == 3) //If win condition is controlling 3 landmarks
        {
            //If Another player has target landmarks - 1 then do something about it!
            foreach(GameObject playerObject in allPlayers)
            {
                Player playerComponent = playerObject.GetComponent<Player>();
                if (playerComponent.playerNumber != AIPlayer.playerNumber && playerComponent.tileCounts[5] >= levelcont.TargetLandmarks-1)
                {
                    opponentWinPossible = true;
                    targetPlayer = playerComponent;
                    //No distance to victory variable used because the best move is to target the player that goes last and then hope that the last player will handle players that go before it...
                }
                else
                {
                    Debug.Log("Player does not appear to be about to win");
                }
                if (opponentWinPossible == true)
                {
                    List<GameObject> targetPlayerLandmarks = ownedCellsOfType(targetPlayer, 5);
                    if (minimumCellCost(targetPlayerLandmarks) <= AIPlayer.playerCash)
                    {
                        takenAction = BuyBestCellOfTypeFromPlayer(AIPlayer, targetPlayer, 5);
                    }
                    else
                    {
                        takenAction = tryLobbyAction();
                    }
                }
                else
                {
                    takenAction = false;
                }
            }
        }
        else
        {
            Debug.Log("Error encountered in hardAI.cs:preventOpponentVictory:: Victory condition not found");
        }
        return takenAction;
    }
    //PreventOpponentVictory end

    //This method will see whether the AI can win this turn and, if it can, take the appropriate action
    private bool winningMove()
    {
        //If a winning move can be made, we execute that move and return true.
        //If a winning move cannot be made, we return false
        bool winMove = false; // We initialise with false to make sure nothing weird happens.
        //First check what the win condition is...
        int winCondition = levelcont.VictoryCondition; 
        int NlandmarkAvailable = CountAvailableCellsOfType(AIPlayer,5);
        int NResAvailable = CountAvailableCellsOfType(AIPlayer, 1);
        int NIndAvailable = CountAvailableCellsOfType(AIPlayer, 2);
        int NPowAvailable = CountAvailableCellsOfType(AIPlayer, 3);
        int NCivAvailable = CountAvailableCellsOfType(AIPlayer, 4);
        if (winCondition==0)
        {
            //Win condition is to reach a target influence level
            if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NlandmarkAvailable > 0)
            {
                //If buying a landmark will win the game and there is a landmark available that the AIplayer can afford
                //Then buy the landmark
                winMove = BuyBestcellOfType(AIPlayer, 5);
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NCivAvailable > 0)
            {
                //If we cannot get a landmark then we will try for a civic building
                winMove = BuyBestcellOfType(AIPlayer, 4);
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NResAvailable > 0)
            {
                //Try for a residential building
                winMove = BuyBestcellOfType(AIPlayer, 1);
            }
            else if(tcontrol.powerToThepeople == true)
            {
                //Else if power to the people is switched on, then buying a powerplant might win the game
                if (AIPlayer.playerInfluence + AIPlayer.tileCounts[1] * (int)(levelcont.InfPerRes * (AIPlayer.tileCounts[3]+1) * levelcont.PowerPlantMultiplier) >= levelcont.TargetInfluence && NPowAvailable > 0)
                {
                    winMove = BuyBestcellOfType(AIPlayer, 3);
                }
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) - levelcont.InfPerInd >= levelcont.TargetInfluence && AIPlayer.tileCounts[2] > 0)
            {
                //If selling an industrial district will allow the AI player to win the game
                winMove = SellBestCellOfType(AIPlayer, 2);
            }
            else
            {
                winMove = false;
            }
        }
        else if (winCondition==1)
        {
            //Win condition is to control the map
            int sumTiles = AIPlayer.tileCounts[0] + AIPlayer.tileCounts[1] + AIPlayer.tileCounts[2] + AIPlayer.tileCounts[3] + AIPlayer.tileCounts[4] + AIPlayer.tileCounts[5];
            if(sumTiles + 1 >= levelcont.TargetCells)
            {
                //If buying one more tile will win the game
                List<GameObject> availableCells = FindAvailableCells(AIPlayer);
                if(availableCells.Count > 0) //If there are any available cells in the list
                {
                    GameObject cheapest = cheapestCell(availableCells);
                    HexCell cheapestHexCellComponent = cheapest.GetComponent<HexCell>();
                    if (AIPlayer.playerCash >= cheapestHexCellComponent.cellPrice)
                    {
                        Debug.Log("Parse to actions from winningMove");
                        actions.OnBuyCell(AIPlayer, cheapestHexCellComponent); //Buy the cheapest cell available to win the game
                        winMove = true;
                    }
                }
            }
        }
        else if (winCondition==2)
        {
            //Win condition is to gain target cash
            int sumTiles = AIPlayer.tileCounts[0] + AIPlayer.tileCounts[1] + AIPlayer.tileCounts[2] + AIPlayer.tileCounts[3] + AIPlayer.tileCounts[4] + AIPlayer.tileCounts[5];
            float CpI = levelcont.CashPerInd;
            float CpR = levelcont.CashPerRes;
            if (tcontrol.powerToThepeople == true)
            {
                for (int j = 0; j < AIPlayer.tileCounts[3]; j++)
                {
                    CpR = (CpR * levelcont.PowerPlantMultiplier);
                }
            }
            else
            {
                for (int j = 0; j < AIPlayer.tileCounts[3]; j++)
                {
                    CpI = (CpI * levelcont.PowerPlantMultiplier);
                }
            }

            float CpIplus1PP = CpI * levelcont.PowerPlantMultiplier;
            float CpRpluss1PP = CpR * levelcont.PowerPlantMultiplier;

            int minimumIndCost = minimumCellCost(2, AIPlayer);
            int minimumPowCost = minimumCellCost(3, AIPlayer);
            int minimumResCost = minimumCellCost(1, AIPlayer);
            if (AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) + CpI - minimumIndCost >= levelcont.TargetCash && NIndAvailable > 0)
            {
                //If buying a new industrial district makes sense
                winMove = BuyBestcellOfType(AIPlayer, 2);
            }
            else if(AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) - (CpI * AIPlayer.tileCounts[2]) + (CpIplus1PP*AIPlayer.tileCounts[2]) - minimumPowCost >= levelcont.TargetCash && NPowAvailable > 0 && tcontrol.powerToThepeople != true)
            {
                //If buying a powerplant makes sense when powerplants affect industrial districts
                winMove = BuyBestcellOfType(AIPlayer, 3);
                
            }
            else if(AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) - (CpR * AIPlayer.tileCounts[1]) + (CpRpluss1PP * AIPlayer.tileCounts[1]) - minimumPowCost >= levelcont.TargetCash && NPowAvailable >0 && tcontrol.powerToThepeople == true)
            {
                //If buying a powerplant makes sense when powerplants affect residential districts
                winMove = BuyBestcellOfType(AIPlayer, 3);
            }
            else if(AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) + CpR - minimumResCost >= levelcont.TargetCash && NResAvailable > 0)
            {
                winMove = BuyBestcellOfType(AIPlayer, 1);
            }
            else if(sumTiles > 5) //If we have more than 5 tiles owned
            {
                List<GameObject> allOwnedCells = OwnedCellsAll(AIPlayer);
                GameObject mostExpensiveOwnedCell = mostExpensiveCell(allOwnedCells);
                HexCell expensiveComponent = mostExpensiveOwnedCell.GetComponent<HexCell>();
                if((expensiveComponent.cellPrice * 0.5) + AIPlayer.playerCash >= levelcont.TargetCash)
                {
                    Debug.Log("Parse to actions from winningMove");
                    actions.OnSellCell(AIPlayer, expensiveComponent);
                    winMove = true;
                }
                else
                {
                    winMove = false;
                }
            }
            else
            {
                winMove = false;
            }
        }
        else if(winCondition==3)
        {
            //win condition is to gain 3 landmarks
            if(AIPlayer.tileCounts[5] >= levelcont.TargetLandmarks && NlandmarkAvailable > 0)
            {
                winMove = BuyBestcellOfType(AIPlayer, 5); //If the player can get a landmark and they only need one more landmark, then buy it
            }
            else
            {
                winMove = false; //Otherwise no winning move is available.
            }
        }
        else
        {
            Debug.Log("Error encountered in hardAI.cs:winningMove:: Victory condition not found");
        }
        return winMove;
    }
    //winning move method ends
    //Here ends the core logic methods of the AI
    //Below this point are general purpose methods for performing calculations, buying and selling districts etc.
    int CountAvailableCellsOfType(Player currentPlayer, int targetCellType)//This is very similar to FindAvailableCells and works in the same way but returns a simple integer of the number of available cells
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell"); //Get all of the objects with the tag "HexCells" This should be every cell on the board and nothing else
        int count = 0;//Start a counter with the value 0 
        foreach(GameObject cell in fullCellsList)
        {
            //Debug.Log(count);
            HexCell theCellComponent = cell.GetComponent<HexCell>(); //Get the HexCell component of the HexCell game object
            /*Debug.Log(targetCellType);
            Debug.Log(theCellComponent.cellType);
            Debug.Log(currentPlayer);
            Debug.Log(theCellComponent.cellOwner);
            Debug.Log(theCellComponent.cellPrice);*/
            if(theCellComponent.cellType == targetCellType && currentPlayer.playerCash >= theCellComponent.cellPrice && theCellComponent.cellOwner != currentPlayer.playerNumber) //Check that cell price is the correct variable name
            {
                //If the cell is of the correct type and the player can afford to buy it
                count++;
            }
          /*  else //This else statement should be commented out unless we need to test that the default state is proceding as expected. Otherwise it will just clutter up the log
            {
                Debug.Log("Everything is probably fine.");
            }*/
        }
        return count;
    }

    List<GameObject> FindAvailableCellsOfType(Player currentPlayer, int targetCellType) //This is very similar to Count available cells and works in the same way but returns a list of gameObjects
    {
        List<GameObject> possibleCells = new List<GameObject>();
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        foreach(GameObject cell in fullCellsList)
        {
            HexCell theCellComponent = cell.GetComponent<HexCell>();
            if (theCellComponent.cellType == targetCellType && currentPlayer.playerCash >= theCellComponent.cellPrice && theCellComponent.cellOwner != currentPlayer.playerNumber) //Check that cell price is the correct variable name
            {
                //If the cell is of the correct type and the player can afford to buy it
                possibleCells.Add(cell);//Add the HexCell gameObject to the list
            }
            /*  else //This else statement should be commented out unless we need to test that the default state is proceding as expected. Otherwise it will just clutter up the log
            {
                Debug.Log("Everything is probably fine.");
            }*/
        }
        return possibleCells;
    }

    GameObject cheapestCell(List<GameObject> cellList) //This method finds the cheapest
    {
        int minCellPrice = 100000; //Start at an arbitrarily large number that would not be found within the main game
        GameObject cheapestCell = new GameObject(); //Assign cheapest cell to be an empty game object on initialisation. This is simply to avoid errors.
        bool foundMinCell = false; //This boolean will just be used to write an error message to the log if a cheapest cell cannot be found
        foreach(GameObject cell in cellList)
        {
            HexCell hexCellComp = cell.GetComponent<HexCell>();
            if(hexCellComp.cellPrice < minCellPrice)
            {
                minCellPrice = hexCellComp.cellPrice;
                cheapestCell = cell;
                foundMinCell = true;
            }
        }
        if(foundMinCell != true)
        {
            Debug.Log("Error in hardAI.cs:cheapestCell :: Cannot determine cheapest cell! Input list of cells may be empty");
        }
        return cheapestCell;
    }

    GameObject mostExpensiveCell(List<GameObject> cellList)
    {
        int maxCellPrice = 0;
        GameObject mostExpensiveCell = new GameObject();
        bool foundMaxCell = false;
        foreach(GameObject cell in cellList)
        {
            HexCell theCellComp = cell.GetComponent<HexCell>();
            if(theCellComp.cellPrice > maxCellPrice)
            {
                maxCellPrice = theCellComp.cellPrice;
                mostExpensiveCell = cell;
                foundMaxCell = true;
            }
        }
        if(foundMaxCell != true)
        {
            Debug.Log("Error in hardAI:mostExpensiveCell :: Cannot determine most expensive cell! Input list of cells may be empty");
        }
        return mostExpensiveCell;
    }

    List<GameObject> ownedCellsOfType(Player AIPlayer, int cellType)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> ownedCells = new List<GameObject>();
        foreach(GameObject cell in fullCellsList)
        {
            HexCell hexcomp = cell.GetComponent<HexCell>();
            if(hexcomp.cellOwner == AIPlayer.playerNumber && hexcomp.cellType == cellType)
            {
                ownedCells.Add(cell);
            }
        }
        return ownedCells;
    }

    List<GameObject> OwnedCellsAll(Player currentPlayer)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> ownedCells = new List<GameObject>();
        foreach(GameObject cell in fullCellsList)
        {
            HexCell hexComponent = cell.GetComponent<HexCell>();
            if(hexComponent.cellOwner == currentPlayer.playerNumber)
            {
                ownedCells.Add(cell);
            }
        }
        return ownedCells;
    }

    List<GameObject> FindAvailableCells(Player currentPlayer)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> posscells = new List<GameObject>();
        foreach(GameObject cell in fullCellsList)
        {
            HexCell component = cell.GetComponent<HexCell>();
            if(component.cellOwner != AIPlayer.playerNumber && component.cellPrice <= AIPlayer.playerCash)
            {
                posscells.Add(cell);
            }
        }
        return posscells;
    }

    int minimumCellCost(int cellType, Player currentPlayer)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> possCells = new List<GameObject>();
        int minimum = 1000000;
        foreach(GameObject cell in fullCellsList)
        {
            HexCell hexComponent = cell.GetComponent<HexCell>();
            if(hexComponent.cellType == cellType && hexComponent.cellOwner != currentPlayer.playerNumber && hexComponent.cellPrice < minimum)
            {
                minimum = hexComponent.cellPrice;
            }
        }
        return minimum;
    }


    bool BuyBestcellOfType(Player currentPlayer, int cellTypeNo)
    {
        List<GameObject> availableCellsOfType = FindAvailableCellsOfType(currentPlayer, cellTypeNo);
        Debug.Log("Available cells:");
        Debug.Log(availableCellsOfType.Count);
        GameObject CellToBuy = cheapestCell(availableCellsOfType);
        Debug.Log("Cell price: ");
        Debug.Log(CellToBuy.GetComponent<HexCell>().cellPrice);
        HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
        Debug.Log("parse to actions from BuyBestcellOfType");
        actions.OnBuyCell(currentPlayer, theCell); //Buy the cell
        bool winMove = true; //Confirm that a winning move has been performed.
        return winMove;
    }
    bool SellBestCellOfType(Player currentPlayer, int cellTypeNo)
    {
        List<GameObject> ownedIndCells = ownedCellsOfType(currentPlayer, cellTypeNo);
        GameObject theCellToSell = mostExpensiveCell(ownedIndCells);
        HexCell theHexCellToSell = theCellToSell.GetComponent<HexCell>();
        Debug.Log("parse to actions from SellBestCellOfType");
        actions.OnSellCell(currentPlayer, theHexCellToSell);
        bool winMove = true;
        return winMove;
    }
    bool BuyBestCellOfTypeFromPlayer(Player currentPlayer, Player targetPlayer, int cellTypeNo)
    {
        List<GameObject> availableCellsOfType = ownedCellsOfType(targetPlayer, cellTypeNo);
        GameObject CellToBuy = cheapestCell(availableCellsOfType);
        HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
        Debug.Log("parse to actions from BuyBestCellOfTypeFromPlayer");
        actions.OnBuyCell(currentPlayer, theCell); //Buy the cell
        bool winMove = true; //Confirm that a winning move has been performed.
        return winMove;
    }
    bool BuyBestCellFromPlayer(Player currentPlayer, Player targetPlayer)
    {
        List<GameObject> targetownedCells = OwnedCellsAll(targetPlayer);
        GameObject CellToBuy = cheapestCell(targetownedCells);
        HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
        Debug.Log("parse to actions from BuyBestCellFromPlayer");
        actions.OnBuyCell(currentPlayer, theCell); //Buy the cell
        bool winMove = true; //Confirm that a winning move has been performed.
        return winMove;
    }
    bool BuyBestCellOfAnyType(Player currentPlayer)
    {
        bool winMove = false;
        List<GameObject> availableCells = FindAvailableCells(currentPlayer);
        GameObject CellToBuy = cheapestCell(availableCells);
        HexCell theCell = CellToBuy.GetComponent<HexCell>();
        Debug.Log("parse to actions from BuyBestCellOfAnyType");
        actions.OnBuyCell(currentPlayer,theCell);
        winMove = true;
        return winMove;
    }
    int minimumCellCost(List<GameObject> listOfCells)
    {
        int mincost = 1000000;
        foreach (GameObject cellObj in listOfCells)
        {
            HexCell cell = cellObj.GetComponent<HexCell>();
            if (cell.cellPrice < mincost) mincost = cell.cellPrice;
        }
        return mincost;
    }
}
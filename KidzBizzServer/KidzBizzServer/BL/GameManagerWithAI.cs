﻿using System.Diagnostics.Metrics;

namespace KidzBizzServer.BL
{
    public class GameManagerWithAI
    {
        Player player = new Player();
        AIPlayer aiPlayer = new AIPlayer();
        int currentPlayerIndex;
        int diceRoll;


        public GameManagerWithAI(Player player, AIPlayer aiPlayer, int currentPlayerIndex,int diceRoll)
        {
            Player = player;
            AiPlayer = aiPlayer;
            CurrentPlayerIndex = currentPlayerIndex;
            DiceRoll = diceRoll;

        }

        public Player Player { get => player; set => player = value; }
        public AIPlayer AiPlayer { get => aiPlayer; set => aiPlayer = value; }
        public int CurrentPlayerIndex { get => currentPlayerIndex; set => currentPlayerIndex = value; }
        public int DiceRoll { get => diceRoll; set => diceRoll = value; }





        // פעולה להפעלת משחק חדש
        public void StartNewGame()
        {
            // לוגיקה להפעלת משחק חדש
            Game game = new Game();
            game.NumberOfPlayers = 2;
            game.GameDuration = new TimeSpan(0, 0, 0);
            game.GameStatus = "Active";
            game.GameTimestamp = DateTime.Now;

            // insert the new game into the data base inside table 'game'
            SaveDetailsGameToDatabase(game);

            // initial money for each player = 1500 
            // initial propertys for each player = 0
            // initial position for each player = 0

            player.CurrentBalance = 1500;
            aiPlayer.CurrentBalance = 1500;
            player.CurrentPosition = 0;
            aiPlayer.CurrentPosition = 0;
            player.Properties = new List<Property>();
            aiPlayer.Properties = new List<Property>();

            currentPlayerIndex = new Random().Next(0, 2);  // פעולה שמגרילה מי השחקן שיתחיל ראשון

        }

        // פעולה לשמירת פרטי משחק במסד הנתונים
        public void SaveDetailsGameToDatabase(Game game)
        {

            // לוגיקת שמירת פרטי משחק במסד הנתונים
        }

        // פעולה למעבר לתור השחקן הבא
        public void MoveToNextPlayer()
        {
            // לוגיקת מעבר לשחקן הבא
        }

        // פעולה לזריקת קובייה
        public void RollDice()
        {
            Random random = new Random();
            int dice1 = random.Next(1, 7);
            int dice2 = random.Next(1, 7);
            diceRoll = dice1 + dice2;


            if (dice1 == dice2)
            {
                // Double rolled, allow another turn for the same player
                // No need to change currentPlayerIndex
            }
            else
            {
                // Change turn to the next player
                currentPlayerIndex = (currentPlayerIndex + 1) % 2;
            }

            // Move the current player
            MovePlayer(diceRoll);
            // Update player details
            //UpdatePlayerDetails();

           
    
        }





        //מימוש פונקציה שמזיזה את השחקן בהתאם להגרלה
        private void MovePlayer(int steps)
        {
            if (currentPlayerIndex == 0)
            {
                player.CurrentPosition = (player.CurrentPosition + steps) % 40; // Assuming 40 slots on the board

                // After each dice roll, update the player's position on the game board in your React state

                HandleSlotActions(player.CurrentPosition, "Go");

                //נקבל את type from react

            }
            else
            {
                aiPlayer.CurrentPosition = (aiPlayer.CurrentPosition + steps) % 40; // Assuming 40 slots on the board


                //After each dice roll, update the player's position on the game board in your React state

                HandleSlotActions(aiPlayer.CurrentPosition, "Jail");

                //נקבל את type from react
            }
        }



        //כל הפעולות שנבצע על לוח המשחק SWITCH CASE
        private void HandleSlotActions(int currentPos, string slot)
        {
            switch (currentPos)
            {
                case 0: // when enter the "GO" slot the user gets 200 NIS
                    // Add NIS 200 to the player's balance
                    if (currentPlayerIndex == 0)
                    {
                        player.CurrentBalance += 200;
                    }
                    else
                    {
                        aiPlayer.CurrentBalance += 200;
                    }
                    break;

                //case "מלונות": // when we know the index of hotel we will change it 
                //    // Check if the property is available for purchase
                //    if (currentPlayerIndex == 0)
                //    {
                //        // Determine the property the player landed on based on their current position
                //        Property property = GetPropertyAtPosition(currentPosition);

                //        // Check if the property belongs to the other player
                //        if (IsPropertyOwnedByOtherPlayer(property))
                //        {
                //            // Deduct 10% of the property value from the current player's balance
                //            int rentAmount = (int)(property.PropertyPrice * 0.1);
                //            player.CurrentBalance -= rentAmount;

                //            // Add the rent amount to the other player's balance
                //            aiPlayer.CurrentBalance += rentAmount;
                //        }
                //        else
                //        {
                //            // Prompt the player to buy or not
                //            // For example, you could implement a UI prompt or return a boolean from a function
                //            bool shouldBuy = AskPlayerToBuyProperty(property);
                //            if (shouldBuy)
                //            {
                //                // Deduct the property price from the player's balance and add the property to their list
                //                player.CurrentBalance -= property.Price;
                //                player.Properties.Add(property);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // Activate AI player's functions for handling property
                //        // This logic should also include checking if the property belongs to the player
                //        // If not, AI player should decide whether to buy or not
                //        ActivateAIPlayerFunctionForProperty(currentPosition);
                //    }
                //    break;

                //case "הידעת":
                //    // Decide whether to answer a question
                //    if (currentPlayerIndex == 0)
                //    {
                //        // Prompt the player to answer a question
                //        // For example, you could implement a UI prompt or return a boolean from a function
                //        bool shouldAnswer = AskPlayerToAnswerQuestion();
                //        if (shouldAnswer)
                //        {
                //            // Display question to the player and handle correct answer
                //            bool answeredCorrectly = DisplayAndCheckQuestion();
                //            if (answeredCorrectly)
                //            {
                //                // Win money if answered correctly
                //                player.CurrentBalance += 100; // For example, add NIS 100
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // Activate AI player's functions for handling knowledge slot
                //        ActivateAIPlayerFunctionForKnowledge();
                //    }
                //    break;
                    
                    //נניח שתא מס' 30 זה הכלא אז נפעיל את הפונקציה לעשות סקיפ ל3 טורות
                case 15:
                    if ((currentPlayerIndex == 0 && player.CurrentPosition == 30) || (currentPlayerIndex == 1 && aiPlayer.CurrentPosition == 30)) {

                        SkipTurns(3);

                    }
                    break;

                // Add other cases for different slot types as needed

                default:
                    // Handle other slot types
                    break;
            }
        }

        
        //פונקציה שמדלגת על 3 טורות
        private void SkipTurns(int turnsToSkip)
        {
            // Logic to skip turns for the current player
            // For example, you can increment currentPlayerIndex to move to the next player
            for (int i = 0; i < turnsToSkip; i++)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % 2;
            }
        }

        //האם הנכס שייך למישהו אחר ? לעבור על מערך הנכסים ולבדוק אם קיים.
        private bool IsPropertyOwnedByOtherPlayer(Property property)
        {
            // Check if the property belongs to the other player
            if (currentPlayerIndex == 0)
            {
                // Check if the property exists in the AI player's list of properties
                //foreach (var property in aiPlayer.Properties)
                //{
                //    if (property.PropertyId == property.PropertyId)
                //    {
                //        return true; // Property belongs to the AI player
                //    }
                //}
            }
            else
            {
                // Check if the property exists in the player's list of properties
                //foreach (var property in player.Properties)
                //{
                //    if (property.PropertyId == property.PropertyId)
                //    {
                //        return true; // Property belongs to the player
                //    }
                //}
            }

            return false; // Property does not belong to the other player
        }


        //בהתאם למיקום לקבל את הנכס שדרכתי עליו,להביא את הדירות מהדאטה בייס
        //private Property GetPropertyAtPosition(int position)
        //{
        //    // Placeholder logic to retrieve property based on position
        //    // You need to implement the actual logic to map positions to propertys
        //    // This could involve accessing a predefined list of propertys or querying a database

        //    // For demonstration purposes, let's assume propertys are predefined
        //    // and we have a list of propertys where each property corresponds to a specific position

        //    // Example hardcoded list of propertys (for demonstration only)
        //    //List<Property> propertys = new List<Property>
        //    //  {

        //    //         new Property { PropertyId = 1, Name = "Park Lane", Price = 350 },
        //    //           new Property { PropertyId = 2, Name = "Mayfair", Price = 400 },
        //    //        // Add more propertys as needed
        //    //     };

        //    // Retrieve the property at the specified position
        //    // Assuming position is 0-based index
        //    //int index = position % propertys.Count; // Modulo operation to handle circular board
        //    //return propertys[index];
        //}
    

        // מימוש פונקציה שתשאל את השחקן האם לקנות את הנכס
        //private bool AskPlayerToBuyProperty(Property property)
        //{
        //    // Implement logic to prompt the player to buy or not (e.g., show a UI dialog)
        //    // Return true if the player chooses to buy, false otherwise
        //}


        //private bool AskPlayerToAnswerQuestion()
        //{
        //    // Implement logic to prompt the player to answer a question (e.g., show a UI dialog)
        //    // Return true if the player chooses to answer, false otherwise
        //}

        //מימוש פונקציה שתראה על המסך את השאלה לשחקן
        //private bool DisplayAndCheckQuestion()
        //{
        //    // Implement logic to display a question to the player and check if they answered correctly
        //    // Return true if the player answers correctly, false otherwise
        //}

        // מימוש פונקציה הסתברותית ששחקן האיהיי יקנה את הנכס
        private void ActivateAIPlayerFunctionForProperty(Property property)
        {
            // Implement logic for AI player's actions when landing on an property slot
        }


        //מימוש פונקציה הסתברותית ששחקן האיהי יענה נכון על כרטיס הידעת
        private void ActivateAIPlayerFunctionForKnowledge()
        {
            // Implement logic for AI player's actions when landing on a knowledge slot
        }


        //בכל מקום שנבצע שינוי של כסף נקרא לפונקציה זו
        private void UpdatePlayerDetails(double price)
        {
            if (currentPlayerIndex == 0)
            {
                // Update player details
                player.CurrentBalance -= price; // For example, deduct 100 from the player's balance
                                              // Update other player details as needed
            }
            else
            {
                // Update AIPlayer details
                aiPlayer.CurrentBalance -= price; // For example, deduct 100 from the AI player's balance
                                                // Update other AIPlayer details as needed
            }
        }

        // פעולה לסיום משחק
        public void EndGame()
        {
            // לוגיקת סיום משחק
        }

        // פעולה להפסקת משחק
        public void PauseGame()
        {
            // לוגיקת הפסקת משחק
        }

        // פעולה להמשך משחק
        public void ContinueGame()
        {
            // לוגיקת המשך משחק
        }   
    }
}

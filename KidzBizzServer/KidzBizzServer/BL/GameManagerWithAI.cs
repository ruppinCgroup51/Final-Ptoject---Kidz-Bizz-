

using System.Diagnostics.Metrics;

namespace KidzBizzServer.BL
{
    public class GameManagerWithAI
    {
        Player player = new Player();
        Player aiPlayer = new Player();
        int currentPlayerIndex;
        int diceRoll;
        Game game = new Game();
        bool skipNextTurn;
        bool extraTurn;
        List<PrisonUser> prisonUsers = new List<PrisonUser>(); // הוספת רשימת אסירים


        public GameManagerWithAI(Player player, Player aiPlayer, int currentPlayerIndex, int diceRoll, Game game)
        {
            Player = player;
            AiPlayer = aiPlayer;
            CurrentPlayerIndex = currentPlayerIndex;
            DiceRoll = diceRoll;
            Game = game;

        }

        public GameManagerWithAI()
        {
        }

        public Player Player { get => player; set => player = value; }
        public Player AiPlayer { get => aiPlayer; set => aiPlayer = value; }
        public int CurrentPlayerIndex { get => currentPlayerIndex; set => currentPlayerIndex = value; }
        public int DiceRoll { get => diceRoll; set => diceRoll = value; }

        public Game Game { get => game; set => game = value; }

        // פעולה להפעלת משחק חדש
        public (int gameId, List<Player> players) StartNewGame(User user)
        {
            DBservices dbs = new DBservices();


            var (startingMoney, currentLocation) = dbs.GetGameSettings();

            // Create a new game instance
            Game game = new Game
            {
                NumberOfPlayers = 2,
                GameDuration = "00:00:00",  // no duration initially // Timespan made some problems later we will change it 
                GameStatus = "Active",
                GameTimestamp = DateTime.Now
            };

            // Insert the game into the database
           int gameId =  game.InsertGame(); // Assuming this method exists and returns game ID


            // Create players
            Player player = new Player
            {
                // how to convert decimal to double
                User = user,
                CurrentBalance = Convert.ToDouble(startingMoney),
                CurrentPosition = currentLocation,
                PlayerStatus = "Active",
                LastDiceResult = 0,
                PlayerType = 0

            };

            player.Insert();


            Player aiPlayer = new Player
            {
                User = new User
                {
                    UserId = 1016,
                    Gender = "Not specified",
                    LastName = "AI",
                    Password = "password",
                    Username = "AIPlayer",
                    FirstName = "AI",
                    AvatarPicture = "https://robohash.org/avatar1"
                }, // Assuming AI player has user ID 2
                CurrentBalance = Convert.ToDouble(startingMoney),
                CurrentPosition = currentLocation,
                PlayerStatus = "Active",
                LastDiceResult = 0,
                PlayerType = new Random().Next(1, 4)
            };

            aiPlayer.Insert();

            // Randomly decide who starts first
            currentPlayerIndex = new Random().Next(0, 2);
            return (gameId, new List<Player> { player, aiPlayer });
        }



        public Player EndGame(int gameId, Player player, Player aiPlayer)
        {
            DBservices dbs = new DBservices();
            Game game = dbs.GetGameById(gameId);

            game.GameDuration = (DateTime.Now - game.GameTimestamp).ToString("g");
            game.GameStatus = "Completed";

            game.UpdateGame();

            player.PlayerStatus = "Not Active";
            aiPlayer.PlayerStatus = "Not Active";

            player.Update();
            aiPlayer.Update();

            string winner = DetermineWinner(); // Implement this method to decide based on game logic
            Console.WriteLine($"The game has ended. The winner is {winner}.");

            if (winner == "Player" || winner == "Draw")
            {
                player.User.Score += 200; // Update score for human player
                player.Update();
                return player;
            }
            else
            {
                return aiPlayer;
            }

        }

        public string DetermineWinner()
        {

            if (player.CurrentBalance > aiPlayer.CurrentBalance)
            {
                return "Player";
            }
            else if (aiPlayer.CurrentBalance > player.CurrentBalance)
            {
                return "AIPlayer";
            }
            else
            {
                if (player.Properties.Count() > aiPlayer.Properties.Count())
                {
                    return "Player";
                }
                else if (aiPlayer.Properties.Count() > player.Properties.Count())
                {
                    return "AIPlayer";
                }
                else
                {
                    return "Draw";
                }

            }
        }

        // פעולה לזריקת קובייה
        public Player RollDice(Player player)
        {
            Random random = new Random();
            int dice1 = random.Next(1, 7);
            int dice2 = random.Next(1, 7);
            int diceRoll = dice1 + dice2;

            // Update the player's currentPosition
            player.CurrentPosition += diceRoll;
            player.CurrentPosition %= 40; // Wrap around to start if currentPosition >= 40
            player.LastDiceResult = diceRoll;
            player.Dice1 = dice1;
            player.Dice2 = dice2;

            // Save the new position in the database
            player.UpdatePosition();

            // Return the updated player
            return player;
        }


        public void PayRent(int playerId, int propertyOwnerId, int propertyId)
        {
            DBservices dbServices = new DBservices();
            decimal rentPrice = dbServices.GetRentPrice(propertyId);
            decimal payerBalance = dbServices.GetPlayerBalance(playerId);
            decimal ownerBalance = dbServices.GetPlayerBalance(propertyOwnerId);

            if (payerBalance >= rentPrice)
            {
                dbServices.UpdatePlayerBalance(playerId, payerBalance - rentPrice);  // Deduct rent from payer
                dbServices.UpdatePlayerBalance(propertyOwnerId, ownerBalance + rentPrice);  // Add rent to owner
            }
            else
            {
                throw new Exception("Player cannot afford to pay the rent.");
            }
        }

        public double AIBuyProperty(int playerType)
        {
            // הרפתקן
            if (playerType == 1)
            {
                return 0.7;

            }
            // מאוזן
            else if (playerType == 2)
            {
                return 0.5;

            }
            else if (playerType == 3) {

                return 0.3;
            }
            else
            return 0;

        }

        // מתודה קיימת לטיפול בנחיתה על משבצות מיוחדות
        public void HandleSlotActions(int currentPos, Player currentPlayer)
        {
            switch (currentPos)
            {
                case 0:
                    currentPlayer.CurrentBalance += 200;
                    break;
                case 11:
                    currentPlayer.MoveToJail();
                    currentPlayer.CurrentPosition = 30;
                    AddPrisonUser(game.GameId, currentPlayer.PlayerId);
                    break;
                case 30:
                    currentPlayer.MoveToJail();
                    AddPrisonUser(game.GameId, currentPlayer.PlayerId);
                    break;
                default:
                    break;
            }
            currentPlayer.UpdatePosition();
        }

        public void HandleCardAction(int cardId, int playerId, string selectedAnswer, int currentPosition)
        {
            var card = Card.GetCardById(cardId);
            var player = new Player().GetPlayerDetails(playerId);

            if (player == null)
            {
                throw new Exception("Player not found.");
            }

            if (card is CommandCard commandCard)
            {
                ApplyCommandCardEffect(commandCard, player, currentPosition);
            }
            else if (card is SurpriseCard surpriseCard)
            {
                ApplySurpriseCardEffect(surpriseCard, player, currentPosition);
            }
            else if (card is DidYouKnowCard didYouKnowCard)
            {
                ApplyDidYouKnowCardEffect(didYouKnowCard, player, selectedAnswer, currentPosition);
            }
        }

        public void ApplyCommandCardEffect(CommandCard card, Player player, int currentPosition)
        {
            if (card.Description.Contains("לך לכלא"))
            {
                player.MoveToJail();
                AddPrisonUser(game.GameId, player.PlayerId);
                player.CurrentPosition = 30;
                player.UpdatePosition();
            }
            else if (card.Description.Contains("הרווחת"))
            {
                player.CurrentBalance += card.Amount;
            }
            else if (card.Description.Contains("הרוויח"))
            {
                player.CurrentBalance -= card.Amount;
            }
            else if (card.Description.Contains("שלם"))
            {
                player.CurrentBalance -= card.Amount;
            }
            else if (card.Description.Contains("הפסדת"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(card.Description, @"(\d+(\.\d+)?)%");
                if (match.Success)
                {
                    double percentage = double.Parse(match.Groups[1].Value);
                    double lossAmount = player.CurrentBalance * (percentage / 100.0);
                    player.CurrentBalance -= lossAmount;
                }
            }
            else if (card.Description.Contains("התקדם למשבצת הפקודה הקרובה"))
            {
                int nearestCommandSlot = FindNearestSlot(currentPosition, "פקודה");
                player.CurrentPosition = nearestCommandSlot;
                player.UpdatePosition();
            }
            else if (card.Description.Contains("זכית בתור כפול") || card.Description.Contains("זכית בתור נוסף"))
            {
                if (currentPlayerIndex == 0)
                {
                    GrantExtraTurn(player);
                }
                else
                {
                    GrantExtraTurn(aiPlayer);
                }
            }
            player.Update();
        }

        public void ApplySurpriseCardEffect(SurpriseCard card, Player player, int currentPosition)
        {
            if (card.Description.Contains("קבל") || card.Description.Contains("הרוויח") || card.Description.Contains("הרווחת") || card.Description.Contains("הרווח"))
            {
                player.CurrentBalance += card.Amount;
            }
            else if (card.Description.Contains("שלם"))
            {
                player.CurrentBalance -= card.Amount;
            }
            else if (card.Description.Contains("הפסדת"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(card.Description, @"(\ד+(\.\ד+)?)%");
                if (match.Success)
                {
                    double percentage = double.Parse(match.Groups[1].Value);
                    double lossAmount = player.CurrentBalance * (percentage / 100.0);
                    player.CurrentBalance -= lossAmount;
                }
            }
            else if (card.Description.Contains("התקדם למשבצת הפתעה הקרובה"))
            {
                int nearestSurpriseSlot = FindNearestSlot(currentPosition, "הפתעה");
                player.CurrentPosition = nearestSurpriseSlot;
                player.UpdatePosition();
            }
            else if (card.Description.Contains("זכית בתור כפול") || card.Description.Contains("זכית בתור נוסף"))
            {
                GrantExtraTurn(player);
                if (currentPlayerIndex == 0)
                {
                    extraTurn = true;
                }
                else
                {
                    skipNextTurn = true;
                }
            }
            player.Update();
        }

        public void ApplyDidYouKnowCardEffect(DidYouKnowCard card, Player player, string selectedAnswer, int currentPosition)
        {
            if (selectedAnswer == card.CorrectAnswer)
            {
                player.CurrentBalance += 300;
            }
            else if (card.Description.Contains("התקדם למשבצת הידעת הקרובה"))
            {
                int nearestDidYouKnowSlot = FindNearestSlot(currentPosition, "ידעת");
                player.CurrentPosition = nearestDidYouKnowSlot;
                player.UpdatePosition();
            }

            player.Update();
        }

        public int FindNearestSlot(int currentPosition, string slotType)
        {
            int[] slots;

            switch (slotType)
            {
                case "פקודה":
                    slots = new int[] { 3, 13, 21, 28, 37 };
                    break;
                case "הפתעה":
                    slots = new int[] { 6, 16, 26, 36 };
                    break;
                case "ידעת":
                    slots = new int[] { 8, 18, 23, 26, 33, 39 };
                    break;
                default:
                    slots = new int[] { };
                    break;
            }

            return slots.Where(p => p > currentPosition).OrderBy(p => p).FirstOrDefault();
        }

        private void GrantExtraTurn(Player currentPlayer)
        {
            if (currentPlayer.PlayerId == player.PlayerId)
            {
                extraTurn = true;
                Console.WriteLine("קיבלת תור נוסף!");
            }
            else
            {
                extraTurn = true;
                Console.WriteLine("AI קיבל תור נוסף!");
            }
        }

        private void SkipNextTurn(Player currentPlayer)
        {
            if (currentPlayer.PlayerId == player.PlayerId)
            {
                skipNextTurn = true;
                Console.WriteLine("השחקן קיבל דילוג תור!");
            }
            else
            {
                skipNextTurn = true;
                Console.WriteLine("AI קיבל דילוג תור!");
            }
        }

        private void AddPrisonUser(int gameId, int playerId)
        {
            prisonUsers.Add(new PrisonUser(gameId, playerId, "In Jail"));
        }

        // פעולה להפסקת משחקc
        public void PauseGame()
        {
            game.GameStatus = "Completed";
            Console.WriteLine("המשחק הסתיים.  !");
        }

        // פעולה להמשך משחק
        public void ContinueGame()
        {
            // לוגיקת המשך משחק
        }
    }
}
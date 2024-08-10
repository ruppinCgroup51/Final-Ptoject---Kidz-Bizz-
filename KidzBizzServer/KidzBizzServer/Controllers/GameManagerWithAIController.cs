using KidzBizzServer.BL;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KidzBizzServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameManagerWithAIController : ControllerBase
    {
        // GET: api/<GameManagerWithAIController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost("startnewgame")]
        public IActionResult StartNewGame(User user)
        {
            try
            {
                GameManagerWithAI gameManagerWithAI = new GameManagerWithAI();
                var (gameId, players) = gameManagerWithAI.StartNewGame(user);
                return Ok(new { gameId, players});
            }
            catch (Exception ex)
            {
                return BadRequest($"Error starting game: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("api/endgame")]
        public IActionResult EndGame(int gameId, int PlayerId, int PlayerAI)
        {
            try
            {
                Player player = new Player();
                Player aiPlayer = new Player();
                // Assuming request contains gameId, player, and aiPlayer info
                player = player.GetPlayerDetails(PlayerId);
                aiPlayer = aiPlayer.GetPlayerDetails(PlayerAI);

                GameManagerWithAI gameManagerWithAI = new GameManagerWithAI();

                Player winner = gameManagerWithAI.EndGame(gameId, player, aiPlayer);


                return Ok(winner);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("AIBuyProperty")]
        public ActionResult<double> GetAIBuyProperty(int playerType)
        {
            GameManagerWithAI gameManagerWithAI = new GameManagerWithAI();
            // Call the AIBuyProperty method
            double result = gameManagerWithAI.AIBuyProperty(playerType);

            // Return the result
            return Ok(result);
        }

        [HttpPost("rolldice")]
        public IActionResult RollDice([FromBody] Player player)
        {
            try
            {
                GameManagerWithAI gameManagerWithAI = new GameManagerWithAI();
                return Ok(gameManagerWithAI.RollDice(player));
            }
            catch (Exception ex)
            {
                return BadRequest($"Error rolling dice: {ex.Message}");
            }
        }
        // פעולה לפתיחת כרטיס

        [HttpPost("opencard")]
        public IActionResult OpenCard([FromBody] CardActionRequest request)
        {
            try
            {
                GameManagerWithAI gameManagerWithAI = new GameManagerWithAI();
                var card = Card.GetCardById(request.CardId);

                // בדיקה אם הכרטיס הוא מסוג "הידעת" ושהתשובה לא ריקה
                if (card is DidYouKnowCard && string.IsNullOrEmpty(request.SelectedAnswer))
                {
                    return BadRequest("Selected answer is required for 'Did You Know' cards.");
                }

                gameManagerWithAI.HandleCardAction(request.CardId, request.PlayerId, request.SelectedAnswer, request.CurrentPosition);
                return Ok("Card action handled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error handling card action: {ex.Message}");
            }
        }

        [HttpPost("payRent")]
        public IActionResult PayRent(int playerId, int propertyOwnerId, int propertyId)
        {
            try
            {
                //int playerId = jsonData.GetProperty("playerId").GetInt32();
                //int propertyOwnerId = jsonData.GetProperty("propertyOwnerId").GetInt32();
                //int propertyId = jsonData.GetProperty("propertyId").GetInt32();

                GameManagerWithAI gameManagerWithAI = new GameManagerWithAI();
                gameManagerWithAI.PayRent(playerId, propertyOwnerId, propertyId);
                return Ok("Rent paid successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }




        // GET api/<GameManagerWithAIController>/5
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                DBservices dbs = new DBservices();
                var player = dbs.GetPlayerById(id);
                if (player == null)
                {
                    return NotFound($"Player with ID {id} not found.");
                }
                return Ok(player);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving player: {ex.Message}");
            }
        }

        // POST api/<GameManagerWithAIController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<GameManagerWithAIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GameManagerWithAIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("updateplayerposition")]
        public IActionResult UpdatePlayerPosition([FromBody] PositionUpdateRequest request)
        {
            try
            {
                GameManagerWithAI gameManagerWithAI = new GameManagerWithAI();
                var player = gameManagerWithAI.Player.PlayerId == request.PlayerId ? gameManagerWithAI.Player : gameManagerWithAI.AiPlayer;
                gameManagerWithAI.HandleSlotActions(request.CurrentPosition, player);
                return Ok("Player position updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating player position: {ex.Message}");
            }
        }
    }


}
public class CardActionRequest
{
    public int CardId { get; set; }
    public int PlayerId { get; set; }
    public string SelectedAnswer { get; set; }
    public int CurrentPosition { get; set; }
}

public class PositionUpdateRequest
{
    public int PlayerId { get; set; }
    public int CurrentPosition { get; set; }
}



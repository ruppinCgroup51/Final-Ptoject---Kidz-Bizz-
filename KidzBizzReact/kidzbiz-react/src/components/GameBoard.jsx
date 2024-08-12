import React, {
  useState,
  useContext,
  useEffect,
  useCallback,
  useRef,
} from "react";
import UserContext from "./UserContext"; ////Context to get user data.
import { GameSquare } from "./GameSquare";
import "../css/gameboard.css";
import { SquareConfigData } from "./SquareData.jsx";
import PropertyModal from "./PropertyModal";
import { faDice, faDollarSign } from "@fortawesome/free-solid-svg-icons";
import SquareType from "./SquareType.jsx";
import SurpriseCardModal from "./SurpriseCardModal.jsx";
import ChanceCardModal from "./ChanceCardModal.jsx";
import { Modal as BootstrapModal, Button } from "react-bootstrap";
import {
  FaDiceOne,
  FaDiceTwo,
  FaDiceThree,
  FaDiceFour,
  FaDiceFive,
  FaDiceSix,
} from "react-icons/fa";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Modal from "react-modal";
import { toast } from "react-toastify";
import getBaseApiUrl from "./GetBaseApi";
import "react-toastify/dist/ReactToastify.css";
import { useNavigate } from "react-router-dom";
import DidYouKnowCardModal from "./DidYouKnow.jsx";

import AITypeModal from "./AITypeModal.jsx";

Modal.setAppElement("#root"); //sets the root element for the modals to improve accessibility

console.log("Component Mounting ~");

const GameBoard = () => {
  const numSquares = Array.from({ length: 40 }, (_, i) => i + 1); //Array representing the squares on the game board (1 to 40).
  const user = useContext(UserContext); //The current user, retrieved from context.
  const navigate = useNavigate();
  const [countdown, setCountdown] = useState(5); // Countdown from 5 seconds
  const [gameStarted, setGameStarted] = useState(false);

  const [players, setPlayers] = useState([]); // An array to store player data
  const [currentPlayerIndex, setCurrentPlayerIndex] = useState(0); // Index to track the current player's turn.
  const [isRollDiceDisabled, setIsRollDiceDisabled] = useState(false); //Control the state of the diceRoll and endTurn
  const [isEndTurnDisabled, setIsEndTurnDisabled] = useState(false); //Control the state of the diceRoll and endTurn
  const [displayDice, setDisplayDice] = useState(null); // The dice result to display.
  const [selectedPlayer, setSelectedPlayer] = useState(null); //The player selected for property display.
  const [isModalOpen, setIsModalOpen] = useState(false); //Control the visbility of various modals.
  const [modalSquareIsOpen, setModalSquareIsOpen] = useState(false); //Control the visbility of various modals.
  const [isModalVisible, setIsModalVisible] = useState(false); //Control the visbility of various modals.
  const [modalContent, setModalContent] = useState(""); //Content for the modal.
  const [cardData, setCardData] = useState(null); //Data for the surprise or chance card.
  const [showCard, setShowCard] = useState(false); //Control the visibility of the card and property modals.
  const [showDidYouKnowCard, setshowDidYouKnowCard] = useState(false);
  const [showPropertyModal, setShowPropertyModal] = useState(false); //Control the visibility of the card and property modals.
  const [currentProperty, setCurrentProperty] = useState(null); //The property that is currently being viewed or bought
  const [gameId, SetGameId] = useState(0); // Unique identifier for the game session.

  // new
  const [showAITypeModal, setShowAITypeModal] = useState(false);
  const [aiType, setAIType] = useState("");

  const isHandlingSquareLanding = useRef(false); //A ref to track if the square landing logic is currently being handled, preventing multiple simultaneous operations.
  const currentPlayerRef = useRef(currentPlayerIndex); //A ref to track the currentPlayer.

  //Initializes the game by fetching or loading player data from local storage or the server. Runs once when the component mounts.
  useEffect(() => {
    //setCurrentPlayerIndex(0);
    setIsRollDiceDisabled(false);
    setIsEndTurnDisabled(false);

    const fetchData = async () => {
      if (!user || !user.userId) {
        console.error("User context is missing userId");
        return;
      }

      const apiUrl = getApiUrl("startnewgame");
      try {
        const response = await fetch(apiUrl, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(user),
        });

        if (!response.ok)
          throw new Error(`HTTP error! status: ${response.status}`);

        const data = await response.json();
        setPlayers(data.players);
        SetGameId(data.gameId);
        localStorage.setItem("players", JSON.stringify(data));
        localStorage.setItem("gameId", data.gameId);
      } catch (error) {
        console.error("Error:", error);
      }
    };

    const storedPlayers = localStorage.getItem("players");
    const storedGameId = localStorage.getItem("gameId");
    if (storedPlayers && storedGameId) {
      setPlayers(JSON.parse(storedPlayers));
      SetGameId(storedGameId);

      //new
      const aiPlayer = JSON.parse(storedPlayers)[1];
      if (aiPlayer) {
        let typeDescription = "";
        switch (aiPlayer.playerType) {
          case 1:
            typeDescription = "הרפתקן";
            break;
          case 2:
            typeDescription = "מאוזן";
            break;
          case 3:
            typeDescription = "שמרן";
            break;
          default:
            typeDescription = "לא ידוע";
        }

        setAIType(typeDescription);
        setShowAITypeModal(true);
      }
    } else {
      fetchData();
    }
  }, [user]);

  //Generates the API URL based on the environment (localhost or production). Memoized with useCallback to avoid recreating the function on every render.
  const getApiUrl = useCallback((endpoint) => {
    if (
      location.hostname === "localhost" ||
      location.hostname === "127.0.0.1"
    ) {
      return `https://localhost:7034/api/GameManagerWithAI/${endpoint}`;
    } else {
      return `https://proj.ruppin.ac.il/cgroup51/test2/tar1/api/GameManagerWithAI/${endpoint}`;
    }
  }, []);

  const rollDiceOld = useCallback(
    async (currentIndex) => {
      let currentPlayerIndex = currentIndex;
      console.log("Current Player index - ", currentPlayerIndex);
      const apiUrl = getApiUrl("rolldice");
      try {
        const response = await fetch(apiUrl, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(players[currentPlayerIndex]),
        });

        if (!response.ok)
          throw new Error(`HTTP error! status: ${response.status}`);

        const data = await response.json();
        const updatedPlayers = [...players];
        updatedPlayers[currentPlayerIndex] = data;
        setPlayers(updatedPlayers);
        handleSquareLanding(updatedPlayers[currentPlayerIndex]);
      } catch (error) {
        console.error("Error:", error);
      }
    },
    [players]
  );

  const rollDice = async (currentIndex) => {
    let currentPlayerIndex = currentIndex;
    console.log("Current Player index - ", currentPlayerIndex);
    const apiUrl = getApiUrl("rolldice");
    try {
      const response = await fetch(apiUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(players[currentPlayerIndex]),
      });

      if (!response.ok)
        throw new Error(`HTTP error! status: ${response.status}`);

      const data = await response.json();
      const updatedPlayers = [...players];
      updatedPlayers[currentPlayerIndex] = data;
      setPlayers(updatedPlayers);
      handleSquareLanding(updatedPlayers[currentPlayerIndex]);
    } catch (error) {
      console.error("Error:", error);
    }
  }
  // Automatically rolls dice and ends the turn if the current player is an AI (with userId 1016).
  // Runs whenever currentPlayerIndex or players change.
  useEffect(() => {
    currentPlayerRef.current = currentPlayerIndex;
    console.log("This use effect happen outside");
    if (players[currentPlayerIndex]?.user.userId === 1016) {
      console.log("This use effect happen inside");
      setIsRollDiceDisabled(true);
      // const timer = setTimeout(() => {
      //   rollDice().then(() => {
      //     endTurn();
      //   });
      // }, 2000);
      // 15000 milliseconds = 15 seconds

      // Clear timeout if the component unmounts or if the effect runs again before the timer completes
      // return () => clearTimeout(timer);
    }
  }, [currentPlayerIndex /*, players*/]);

  //Saves the players' state to local storage whenever it changes.
  useEffect(() => {
    localStorage.setItem("players", JSON.stringify(players));
  }, [players]);

  //This function ends the current player's turn.
  // It resets various modal and card states and updates the currentPlayerIndex to the next player.
  // The next player is calculated by incrementing the index and wrapping around using the modulo operator.
  const endTurn = () => {
    setIsModalVisible(false);
    setShowCard(false);
    setshowDidYouKnowCard(false);
    setModalSquareIsOpen(false);
    setModalContent("");
    setCardData(null);
    setIsRollDiceDisabled(false);
    setShowPropertyModal(false);

    // Log before updating the index to track changes
    console.log("endTurn called");

    // Update the current player index
    
    setCurrentPlayerIndex((prev) => {
      const nextIndex = (prev+1) % players.length;
      if (players[nextIndex].user.userId == 1016) {
        rollDice(nextIndex);
      }
      return nextIndex;
    });

  
    // Log after updating to ensure the state is updated
  };

  //n: This function handles the event when a player clicks the "Roll Dice" button.
  // It disables the "Roll Dice" button, enables the "End Turn" button,
  //rolls the dice, and updates the displayed dice for the current player.
  const handleRollDiceClick = async () => {
    setIsRollDiceDisabled(true);
    setIsEndTurnDisabled(false);
    await rollDice(currentPlayerIndex);
    setDisplayDice(players[currentPlayerIndex].user.userId);
  };

  //This function handles the event when a player clicks the "End Turn" button.
  // It updates the players' dice values to zero if the current player is an AI,
  // ends the turn, and toggles the state of the "Roll Dice" and "End Turn" buttons.
  const handleEndTurnClick = () => {
    const updatedPlayers = [...players];
    if (updatedPlayers[currentPlayerIndex].user.userId === 1016) {
      updatedPlayers[currentPlayerIndex].dice1 = 0;
      updatedPlayers[currentPlayerIndex].dice2 = 0;
    }
    setPlayers(updatedPlayers);
    endTurn();
    setIsEndTurnDisabled(true);
    setDisplayDice(1016);
  };

  //This function handles ending the game. It constructs the API URL with the game ID and player IDs,
  // sends a request to end the game, and displays the winner's name in an alert.
  // After that, it navigates back to the lobby.
  const handleEndGame = async () => {
    // Display a confirmation modal
    const userConfirmed = window.confirm("האם אתה בטוח שאתה רוצה לצאת מהמשחק?");

    if (userConfirmed) {
      try {
        // Assuming players array has player and AI player at specific indexes
        const playerId = players[0]?.playerId; // Assuming the first player is the human player
        const playerAI = players[1]?.playerId; // Assuming the second player is the AI player
        const gameIdInt = parseInt(gameId, 10);

        // Construct the API URL with query parameters
        const apiUrl = getApiUrl(
          `api/endgame?gameId=${gameIdInt}&PlayerId=${playerId}&PlayerAI=${playerAI}`
        );

        // Call the API to end the game
        const response = await fetch(apiUrl, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
        });

        // Check if the response is okay
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        // Get the response data
        const winner = await response.json();

        // Display a message with the winner's name or other details
        alert(`${winner.user.username} המשחק נגמר !! המנצח הוא `);

        // After displaying the message, navigate back to the lobby
        navigate("/Lobi");
      } catch (error) {
        console.error("Error ending game:", error);
        alert("An error occurred while trying to end the game.");
      }
    } else {
      // If the user cancels, just close the pop-up and do nothing
      console.log("User chose to continue the game.");
    }
  };

  //This function displays the appropriate card modal (Surprise or Chance) based on the type and the player who landed on the square.
  // The modal is shown only if the current player matches the player passed to the function.
  // const handleShowCard = (player, type, data) => {
  //   if (currentPlayerIndex !== players.indexOf(player)) return;
  //   setCardData(data);
  //   if (type === "surprise") setIsModalVisible(true);
  //   else if (type === "chance") setShowCard(true);
  // };

  //This function closes any open card modals.
  const handleCloseCard = () => {
    setIsModalVisible(false);
    setShowCard(false);
    setshowDidYouKnowCard(false);
    setModalSquareIsOpen(false);
  };

  const numberToDiceIcon = (number, size) => {
    const diceIcons = {
      1: <FaDiceOne size={size} />,
      2: <FaDiceTwo size={size} />,
      3: <FaDiceThree size={size} />,
      4: <FaDiceFour size={size} />,
      5: <FaDiceFive size={size} />,
      6: <FaDiceSix size={size} />,
    };
    return diceIcons[number] || null;
  };

  const PlayerProperties = ({ player }) => {
    if (!player) return null;

    return (
      <Modal
        isOpen={isModalOpen}
        onRequestClose={() => setIsModalOpen(false)}
        style={{
          maxWidth: "600px",
          width: "50%",
          margin: "auto",
          position: "relative",
        }}
      >
        <h2
          style={{
            position: "relative",
            textAlign: "right",
            color: "red",
            fontFamily: "cursive",
          }}
        >
          {player.user.firstName} הנכסים של{" "}
        </h2>
        {player.properties && player.properties.length > 0 ? (
          player.properties.map((property, index) => (
            <div key={index} style={{ color: "black", textAlign: "right" }}>
              <p>מזהה נכס: {property.propertyId}</p>
              <p>שם נכס: {property.propertyName}</p>
              <p>מחיר נכס: {property.propertyPrice}</p>
            </div>
          ))
        ) : (
          <p
            style={{ position: "relative", textAlign: "right", color: "black" }}
          >
            אין נכסים
          </p>
        )}
        <button
          style={{
            position: "absolute",
            left: "10px",
            bottom: "10px",
            color: "white",
            backgroundColor: "red",
          }}
          onClick={() => setIsModalOpen(false)}
        >
          סגור
        </button>
      </Modal>
    );
  };

  const handlePropertySquareType = async (position, currentPlayer) => {
    const apiUrl = getBaseApiUrl();
    const fullUrl = `${apiUrl}Properties/CheckPropertyOwnership?propertyId=${position}&playerId=${
      currentPlayer.playerId
    }&playerAiId=${currentPlayer.playerId + 2}`;
    const response = await fetch(fullUrl, {
      method: "GET",
      headers: { Accept: "application/json" },
    });
    const responseText = await response.text();

    if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

    const result = JSON.parse(responseText);
    const owner = result.owner;

    if (owner === -1) {
      // Property has no owner, fetch property details
      //  console.log("this the currentPlayer :", currentPlayer);
      //  if (currentPlayer.user.userId == 1016) {
      //    throw new Error("this is error");
      //  }

      if (isPlayerAI(currentPlayer)) {
        setShowPropertyModal(false);
        // AI buying logic based on type
        const aiBuyProbability = await fetchAIBuyProperty(
          currentPlayer.playerType
        );
        if (Math.random() < aiBuyProbability) {
          // AI decides to buy the property
          await handleBuyPropertyAI(position, currentPlayer);
        }
      } else {
        // For human player, show property modal
        setShowPropertyModal(true);
        const propertyDetails = await fetchPropertyDetails(position);
        if (propertyDetails) {
          setCurrentProperty({
            propertyId: position,
            propertyName: propertyDetails.propertyName,
            propertyPrice: propertyDetails.propertyPrice,
            currentPlayer,
          });
        }
      }
    } /*else if (owner !== currentPlayer.playerId) {
    const rentUrl = `${apiUrl}GameManagerWithAI/payRent`;
    const rentResponse = await fetch(rentUrl, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        playerId: currentPlayer.playerId,
        propertyOwnerId: owner,
        propertyId: position,
      }),
    });

    if (rentResponse.ok) {
      /*toast("Rent paid successfully!", { type: "info" });

      const updatedPlayerResponse = await fetchPlayerData(
        currentPlayer.playerId
      );

      if (updatedPlayerResponse.ok) {
        const updatedPlayerData = await updatedPlayerResponse.json();
        
        // Prevent infinite loop by ensuring setPlayers is not called unnecessarily
        if (!deepEqual(players, updatedPlayerData)) { // deepEqual is a utility function to compare objects deeply
          updatePlayerDataInState(updatedPlayerData);
        }
      }
    } else {
      toast("Failed to pay rent.", { type: "error" });
    }
  }*/
  };

  const fetchAIBuyProperty = async (playerType) => {
    const apiUrl = getBaseApiUrl();
    const fullUrl = `${apiUrl}GameManagerWithAI/AIBuyProperty?playerType=${playerType}`;
    try {
      const response = await fetch(fullUrl, {
        method: "GET",
        headers: { Accept: "application/json" },
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const probability = await response.json();
      return probability;
    } catch (error) {
      console.error(
        "Failed to fetch AI buy property probability:",
        error.message
      );
      return 0; // If there's an error, default to 0 (won't buy)
    }
  };

  const deepEqual = (obj1, obj2) => {
    return JSON.stringify(obj1) === JSON.stringify(obj2);
  };

  const isPlayerAI = (player) => {
    return player.user.userId == 1016;
  };

  const fetchPropertyDetails = async (propertyId) => {
    const apiUrl = getBaseApiUrl();
    const fullUrl = `${apiUrl}Properties/GetPropertyDetails?propertyId=${propertyId}`;
    try {
      const response = await fetch(fullUrl, {
        method: "GET",
        headers: { Accept: "application/json" },
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      return await response.json(); // Directly parsing as JSON
    } catch (error) {
      console.error("Failed to fetch property details:", error.message);
      return null; // Optionally return more specific error information
    }
  };

  const handleBuyPropertyAI = async (propertyId, currentPlayer) => {
    const apiUrl = getBaseApiUrl();
    const buyUrl = `${apiUrl}Properties/BuyProperty?PlayerId=${currentPlayer.playerId}&PropertyId=${propertyId}`;
    const buyResponse = await fetch(buyUrl, { method: "POST" });

    if (buyResponse.ok) {
      toast("רכישת נכס בוצעה בהצלחה", { type: "success" });
      const updatedPlayerResponse = await fetchPlayerData(
        currentPlayer.playerId
      );
      if (updatedPlayerResponse.ok) {
        const updatedPlayerData = await updatedPlayerResponse.json();
        updatePlayerDataInState(updatedPlayerData);
      }
    } else {
      toast("רכישת נכס לא צלחה", { type: "error" });
    }

    setShowPropertyModal(false);
  };

  const handleBuyProperty = async () => {
    const { propertyId, currentPlayer } = currentProperty;
    const apiUrl = getBaseApiUrl();
    const buyUrl = `${apiUrl}Properties/BuyProperty?PlayerId=${currentPlayer.playerId}&PropertyId=${propertyId}`;
    const buyResponse = await fetch(buyUrl, { method: "POST" });

    if (buyResponse.ok) {
      toast("רכישת נכס בוצעה בהצלחה", { type: "success" });
      const updatedPlayerResponse = await fetchPlayerData(
        currentPlayer.playerId
      );
      if (updatedPlayerResponse.ok) {
        const updatedPlayerData = await updatedPlayerResponse.json();
        updatePlayerDataInState(updatedPlayerData);
      }
    } else {
      toast("רכישת נכס לא צלחה", { type: "error" });
    }

    setShowPropertyModal(false);
  };

  const fetchPlayerData = (playerId) => {
    const apiUrl = getBaseApiUrl();
    const fullUrl = `${apiUrl}Players/${playerId}`;
    return fetch(fullUrl, {
      method: "GET",
      headers: { Accept: "application/json" },
    });
  };

  const updatePlayerDataInState = (updatedPlayerData) => {
    setPlayers((prevPlayers) =>
      prevPlayers.map((player) =>
        player.playerId === updatedPlayerData.playerId
          ? updatedPlayerData
          : player
      )
    );
  };

  const handleSurpriseSquareType = async (position, currentPlayer) => {
    const apiUrl = getBaseApiUrl();
    const fullUrl = `${apiUrl}Cards/surprise`;
    const response = await fetch(fullUrl, {
      method: "GET",
      headers: { Accept: "application/json" },
    });
    const responseText = await response.text();

    if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

    const result = JSON.parse(responseText);
    setCardData(result);

    ///   console.log("this the currentPlayer :", currentPlayer);
    ///   if (currentPlayer.user.userId == 1016) {
    ///     throw new Error("this is error");
    ///   }
    isPlayerAI(currentPlayer)
      ? setIsModalVisible(false)
      : setIsModalVisible(true);
  };

  const handleChanceSquareType = async (position, currentPlayer) => {
    const apiUrl = getBaseApiUrl();
    const fullUrl = `${apiUrl}Cards/command`;
    const response = await fetch(fullUrl, {
      method: "GET",
      headers: { Accept: "application/json" },
    });
    const responseText = await response.text();

    if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

    const result = JSON.parse(responseText);
    console.log(typeof parseInt(result.moveTo));

    setCardData(result);

    // console.log("this the currentPlayer :", currentPlayer);
    // if (currentPlayer.user.userId == 1016) {
    //   throw new Error("this is error");
    // }
    // Show the card modal if the current player is not AI
    if (!isPlayerAI(currentPlayer)) {
      setShowCard(true);
    } else {
      // Handle AI logic if necessary
      await handleMovePlayer(parseInt(result.moveTo), currentPlayer);
    }
  };
  const handleDidYouKnowSquareType = async (position, currentPlayer) => {
    const apiUrl = getBaseApiUrl();
    const fullUrl = `${apiUrl}Cards/didyouknow`;
    const response = await fetch(fullUrl, {
      method: "GET",
      headers: { Accept: "application/json" },
    });
    const responseText = await response.text();

    if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

    const result = JSON.parse(responseText);
    setCardData(result);
    //   console.log("this the currentPlayer :", currentPlayer);
    //   if (currentPlayer.user.userId == 1016) {
    //     throw new Error("this is error");
    //   }
    isPlayerAI(currentPlayer)
      ? setshowDidYouKnowCard(false)
      : setshowDidYouKnowCard(true);
  };

  const handleMovePlayer = (newPosition, currentPlayer) => {
    if (currentPlayer) {
      setPlayers((prevPlayers) =>
        prevPlayers.map((player) =>
          player.playerId === currentPlayer.playerId
            ? { ...player, currentPosition: parseInt(newPosition) }
            : player
        )
      );
      toast(`שחקן ${currentPlayer.user.username} עבור ל-${newPosition}`, {
        type: "success",
      });
    }
  };
  const handleSquareLanding = async (currentPlayer) => {
    //if (isHandlingSquareLanding.current) return;
    //isHandlingSquareLanding.current = true;
    const position = currentPlayer.currentPosition;
    const squareType = SquareConfigData.get(position)?.type;

    try {
      switch (squareType) {
        case SquareType.Property:
          await handlePropertySquareType(position, currentPlayer);
          break;
        case SquareType.Present:
          await handleSurpriseSquareType(position, currentPlayer);
          break;
        case SquareType.Chance:
          await handleChanceSquareType(position, currentPlayer);
          break;
        case SquareType.DidYouKnow:
          await handleDidYouKnowSquareType(position, currentPlayer);
          break;
        case SquareType.Go:
          break;
        case SquareType.Jail:
          toast("עלייך להיכנס לכלא");
          break;
        case SquareType.GoToJail:
          toast("לך לכלא");
          break;
        default:
          toast(`נחתת על משבצת רגילה`, { type: "info" });
          break;
      }
    } catch (error) {
      console.error("Error during square landing actions:", error);
      toast("בעיה בטיפול משבצת נכס", { type: "error" });
    } finally {
      if (currentPlayer.user.userId == 1016) {
        // const timer = setTimeout(() => {
        //   endTurn();
        //   window.clearTimeout(timer);
        // }, 2000);
        endTurn();
      }
      isHandlingSquareLanding.current = false;
    }
  };

  useEffect(() => {
    const currentPlayer = players[currentPlayerIndex];
    if (currentPlayer) {
      //handleSquareLanding(currentPlayer);
    } else {
      console.log("There is no player.");
    }
    console.log("handleSquareLanding UseEffect is happen");
  }, [currentPlayerIndex]);

  // Handles the countdown timer before the game starts. Starts the game when the countdown reaches zero.
  useEffect(() => {
    let timerId;
    if (countdown > 0 && !gameStarted) {
      timerId = setTimeout(() => setCountdown(countdown - 1), 1000);
    } else {
      setGameStarted(true);
      // Game initialization logic here
    }
    return () => clearTimeout(timerId);
  }, [countdown, gameStarted]);

  const renderCountdown = () => {
    const radius = 45;
    const circumference = 2 * Math.PI * radius;
    const strokeDashoffset = ((5 - countdown) / 5) * circumference;

    return (
      <div className="countdown-container">
        <svg width="100" height="100">
          <circle
            r={radius}
            cx="50"
            cy="50"
            fill="transparent"
            stroke="#4caf50"
            strokeWidth="10"
            strokeDasharray={circumference}
            strokeDashoffset={strokeDashoffset}
            transform="rotate(-90 50 50)"
          />
          <text
            x="50"
            y="50"
            fill="white"
            textAnchor="middle"
            dy="8"
            fontSize="20"
          >
            {countdown}
          </text>
        </svg>
      </div>
    );
  };

  if (!gameStarted) {
    return renderCountdown();
  }

  return (
    <>
      {/* AI Type Modal */}
      <AITypeModal
        show={showAITypeModal}
        onHide={() => setShowAITypeModal(false)}
        aiType={aiType}
      />
      <button className="endgameBTN" onClick={handleEndGame}>
        סיים משחק
      </button>
      <div className="current-player">
        התור של שחקן - {players[currentPlayerIndex].user.username}
      </div>
      <div className="frame">
        <div className="board">
          {numSquares.map((num) => {
            const playersOnThisSquare = players.filter(
              (player) => player.currentPosition === num
            );
            return (
              <GameSquare key={num} id={num} players={playersOnThisSquare} />
            );
          })}
          <div className="center-square square">
            <div className="players-info">
              {players.map((player, index) => (
                <div key={index} className="player-info">
                  <h3>
                    {player.user.firstName} - שחקן {index + 1}
                  </h3>
                  <p>
                    <FontAwesomeIcon icon={faDollarSign} /> כמות כסף:{" "}
                    {player.currentBalance}
                    <br />
                  </p>
                  <button
                    className="propertyBTN"
                    onClick={() => {
                      setSelectedPlayer(player);
                      setIsModalOpen(true);
                    }}
                  >
                    ראה נכסים
                  </button>
                  {player.user.userId === displayDice &&
                    player.dice1 > 0 &&
                    player.dice2 > 0 && (
                      <p>
                        {numberToDiceIcon(player.dice1, 50)}{" "}
                        {numberToDiceIcon(player.dice2, 50)}
                      </p>
                    )}
                </div>
              ))}
            </div>
            <br />
            <div className="center-txt">
              <button
                onClick={handleRollDiceClick}
                disabled={isRollDiceDisabled}
              >
                <FontAwesomeIcon icon={faDice} /> הגרל קוביות
              </button>
              <br />
              <br />
              <button onClick={handleEndTurnClick} disabled={isEndTurnDisabled}>
                סיים תור
              </button>
              <br />
              <br />
            </div>
          </div>
          <PlayerProperties player={selectedPlayer} />
        </div>
      </div>

      <BootstrapModal show={modalSquareIsOpen} onHide={handleCloseCard}>
        <h2>{modalContent}</h2>
        <button onClick={handleCloseCard}>סגור</button>
      </BootstrapModal>

      {isModalVisible && (
        <SurpriseCardModal card={cardData} onClose={handleCloseCard} />
      )}
      {showCard && (
        <ChanceCardModal
          show={showCard}
          onHide={() => {
            setShowCard(false);
            handleMovePlayer(cardData.moveTo, players[currentPlayerIndex]);
          }}
          cardData={cardData}
        />
      )}
      {showDidYouKnowCard && (
        <DidYouKnowCardModal
          show={showDidYouKnowCard}
          onHide={handleCloseCard}
          cardData={cardData}
        />
      )}
      {currentProperty && (
        <PropertyModal
          show={showPropertyModal}
          onHide={() => setShowPropertyModal(false)}
          property={currentProperty}
          onBuy={handleBuyProperty}
        />
      )}
    </>
  );
};

export default GameBoard;

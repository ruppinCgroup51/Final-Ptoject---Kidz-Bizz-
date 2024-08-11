import React, { useEffect, useRef } from "react";
import BoardSection from "./BoardSection";
import { SquareConfigData } from "/src/components/SquareData.jsx";
import { SquareInfo } from "/src/components/SquareInfo.jsx";
import SquareType from "./SquareType";
import "../css/gamesquare.css";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

export const GameSquare = ({ id, players }) => {
  const section = SquareConfigData.get(id)?.section;
  const squareType = SquareConfigData.get(id)?.type;

  const sectionMap = new Map([
    [BoardSection.Top, "top"],
    [BoardSection.Right, "right"],
    [BoardSection.Left, "left"],
    [BoardSection.Bottom, "bottom"],
  ]);

  const squareTypeClass = new Map([
    [SquareType.Present, "הפתעה"],
    [SquareType.Chance, "סיכוי"],
    [SquareType.Go, "passgo"],
    [SquareType.GoToJail, "go-to-jail"],
    [SquareType.Jail, "jail"],
    [SquareType.Property, "נכס"],
    [SquareType.DidYouKnow, "הידעת ?"],
  ]);

  const prevPlayerPositions = useRef(
    players.map((player) => player.currentPosition)
  );

  const getContainerClassName = () => {
    return "container container-" + sectionMap.get(section);
  };

  const getSquareClassName = () => {
    return "square " + squareTypeClass.get(squareType);
  };

  const getSquareId = () => {
    return "game-square-" + id;
  };

  const getAvatarStyle = (position) => {
    if (position >= 1 && position <= 11) {
      return {
        top: "-30px",
      };
    } else if (position > 11 && position <= 21) {
      return {
        right: "110px",
        top: "-50px",
      };
    } else if (position > 21 && position <= 31) {
      return {
        top: "-50px",
      };
    } else if (position > 31 && position <= 40) {
      return {
        left: "65px",
        top: "-50px",
      };
    }
    return {};
  };

  useEffect(() => {
    players.forEach((player, index) => {
      if (
        player.currentPosition === id &&
        player.currentPosition !== prevPlayerPositions.current[index]
      ) {
        prevPlayerPositions.current[index] = player.currentPosition;
      }
    });
  }, [players, id, squareType, squareTypeClass]);

  return (
    <div className={getSquareClassName()} id={getSquareId()}>
      <div className={getContainerClassName()}>
        <SquareInfo id={id} />
      </div>
      <div className="player-container">
        {players &&
          players.map((player, index) => (
            <img
              key={player["user"]["userId"]}
              src={player["user"]["avatarPicture"]}
              alt="avatar"
              className="avatar"
              style={getAvatarStyle(player.currentPosition)}
            />
          ))}
      </div>
    </div>
  );
};

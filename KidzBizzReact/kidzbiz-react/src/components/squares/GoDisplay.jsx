import React from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHandPointLeft } from "@fortawesome/free-solid-svg-icons";

const GoDisplay = ({ id }) => {
  return (
    <React.Fragment>
      <div className="blank"></div>
      <div className="icon">
        <FontAwesomeIcon icon={faHandPointLeft} color="green" />
      </div>
      <div className="square-name">צא</div>
    </React.Fragment>
  );
};

export default GoDisplay;


import React, { useState } from "react";
import { Modal, Button } from "react-bootstrap";
import "../css/DidYouKnow.css"

const DidYouKnowCardModal = ({ show, onHide, cardData }) => {
  const safeCardData = cardData || {};

  return (
    <Modal show={show} onHide={onHide} className="chance-card-modal"> 
      <Modal.Header>
        <Modal.Title>קלף הידעת</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>
          <strong>תיאור:</strong>
          <br/> {safeCardData.description ?? "N/A"}
        </p>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          המשך
        </Button>
      </Modal.Footer>
    </Modal>
  );
};
export default DidYouKnowCardModal;

import React from "react";
import { Modal, Button } from "react-bootstrap";
import "../css/SurpriseCardModal.css"; // Ensure this CSS file exists and is correctly styled

const SurpriseCardModal = ({ show, card, onClose }) => {
  if (!card) return null;
  console.log("SurpriseCardModal mounted");

  return (
    <Modal show={show} onClose={onClose} className="chance-card-modal">
      <Modal.Header>
        <Modal.Title>קלף הפתעה</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>
          <strong>תיאור:</strong>
          <br /> {card.description ?? "N/A"}
        </p>
        <p>
          <strong>סכום:</strong> {card.amount ?? "N/A"}
        </p>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onClose}>
          סגור
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default SurpriseCardModal;

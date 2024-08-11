import React, { useState } from "react";
import { Modal, Button } from "react-bootstrap";
import "../css/ChanceCard.css";

const ChanceCardModal = ({ show, onHide, cardData }) => {
  const safeCardData = cardData || {};

  return (
    <Modal show={show} onHide={onHide} className="chance-card-modal">
      <Modal.Header>
        <Modal.Title>קלף סיכוי</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>
          <strong>תיאור:</strong>
          <br /> {safeCardData.description ?? "N/A"}
        </p>
        <p>
          <strong>עבור ל:</strong> {safeCardData.moveTo ?? "N/A"}
        </p>
        <p>
          <strong>פעולה:</strong> {safeCardData.action ?? "N/A"}
        </p>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          סגור
        </Button>
      </Modal.Footer>
    </Modal>
  );
};
export default ChanceCardModal;

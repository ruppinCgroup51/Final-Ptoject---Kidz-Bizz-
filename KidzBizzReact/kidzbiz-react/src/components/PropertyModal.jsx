import React from "react";
import { Modal, Button } from "react-bootstrap";
import "../css/PropertyModal.css" // Import the custom CSS

const PropertyModal = ({ show, onHide, property, onBuy }) => {
  return (
    <Modal show={show} onHide={onHide} className="property-modal">
      <Modal.Header>
        <Modal.Title>נכס זמין לרכישה</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>מספר נכס: {property.propertyId}</p>
        <p>שם נכס: {property.propertyName}</p>
        <p>מחיר נכס: {property.propertyPrice} $</p>
        <p>האם אתה מעוניין לרכוש נכס זה ? </p>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          לא
        </Button>
        <Button variant="primary" onClick={onBuy}>
          כן
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default PropertyModal;

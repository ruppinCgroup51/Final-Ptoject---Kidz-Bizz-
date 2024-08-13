import React from "react";
import { Modal, Button } from "react-bootstrap";
import "../css/PropertyModal.css"; // Import the custom CSS

const PropertyModal = ({ show, onHide, property, onBuy }) => {
  return (
    <Modal show={show} onHide={onHide} className="property-modal">
      <Modal.Header>
        <Modal.Title style={{ color: "#e74c3c", fontWeight: "bold" }}>
          נכס זמין לרכישה
        </Modal.Title>
      </Modal.Header>
      <Modal.Body
        style={{ textAlign: "right", fontSize: "16px", color: "#333" }}
      >
        <p>
          <strong>מספר נכס:</strong> {property.propertyId}
        </p>
        <p>
          <strong>שם נכס:</strong> {property.propertyName}
        </p>
        <p>
          <strong>מחיר נכס:</strong> {property.propertyPrice} $
        </p>
        <p style={{ marginTop: "20px" }}>
          <strong>האם אתה מעוניין לרכוש נכס זה?</strong>
        </p>
      </Modal.Body>
      <Modal.Footer
        style={{ borderTop: "none", justifyContent: "space-between" }}
      >
        <Button
          variant="primary"
          onClick={onBuy}
          className="rtl-button"
          style={{ backgroundColor: "green", borderColor: "#3498db" }}
        >
          כן
        </Button>
        <Button
          variant="secondary"
          onClick={onHide}
          className="rtl-button"
          style={{ backgroundColor: "red", borderColor: "#3498db" }}
        >
          לא
        </Button>
      </Modal.Footer>
    </Modal>
  );
};

export default PropertyModal;

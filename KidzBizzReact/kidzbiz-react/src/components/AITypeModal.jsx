import React from "react";
import { Modal as BootstrapModal, Button } from "react-bootstrap";
import '../css/AITypeModal.css'; // Import the CSS file

const AITypeModal = ({ show, onHide, aiType }) => {
  return (
    <BootstrapModal show={show} onHide={onHide} className="ai-type-modal">
      <BootstrapModal.Header className="modal-header">
        <BootstrapModal.Title className="modal-title">שחקן חכם</BootstrapModal.Title>
      </BootstrapModal.Header>
      <BootstrapModal.Body className="modal-body">
       אתה משחק נגד שחקן  {aiType} 
       <br/>
       הולך להיות מעניין!
      </BootstrapModal.Body>
      <BootstrapModal.Footer className="modal-footer">
        <Button variant="primary" onClick={onHide} className="start-game-button">
          התחל משחק
        </Button>
      </BootstrapModal.Footer>
    </BootstrapModal>
  );
};

export default AITypeModal;

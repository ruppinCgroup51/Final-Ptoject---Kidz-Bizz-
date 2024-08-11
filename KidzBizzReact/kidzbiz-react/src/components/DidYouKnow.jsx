import React, { useState, useEffect } from "react";
import { Modal, Button } from "react-bootstrap";
import "../css/DidYouKnow.css";

const DidYouKnow = ({ show, onHide }) => {
  const [showQuestion, setShowQuestion] = useState(false);
  const [cardData, setCardData] = useState(null);
  const [showCorrectAnswerModal, setShowCorrectAnswerModal] = useState(false);
  const [showIncorrectAnswerModal, setShowIncorrectAnswerModal] =
    useState(false);

  useEffect(() => {
    if (show) {
      fetchDidYouKnowCard();
    }
  }, [show]);

  const fetchDidYouKnowCard = async () => {
    try {
      const response = await fetch(
        "https://localhost:7034/api/Cards/didyouknow"
      );
      const data = await response.json();
      setCardData(data);
    } catch (error) {
      console.error("Error fetching DidYouKnow card data:", error);
    }
  };

  const handleContinue = () => {
    setShowQuestion(true);
  };

  const handleAnswerClick = (answer, correctAnswer) => {
    if (answer === correctAnswer) {
      console.log("Selected answer:", answer);
      console.log("cardData answer is: ", correctAnswer);
      setShowCorrectAnswerModal(true); // Show the correct answer modal
    } else {
      setShowIncorrectAnswerModal(true); // Show the incorrect answer modal
    }
  };

  const handleCloseCorrectAnswerModal = () => {
    setShowCorrectAnswerModal(false);
    onHide(); // Close the main modal as well
  };

  const handleCloseIncorrectAnswerModal = () => {
    setShowIncorrectAnswerModal(false);
    onHide(); // Close the main modal as well
  };

  return (
    <>
      <Modal show={show} onHide={onHide} className="chance-card-modal">
        <Modal.Header>
          <Modal.Title>קלף הידעת</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {!showQuestion ? (
            <div>
              <p>
                <strong>תיאור:</strong>
                <br /> {cardData?.description ?? "N/A"}
              </p>
            </div>
          ) : (
            <div>
              <p>
                <strong>שאלה:</strong>
                <br /> {cardData?.question ?? "N/A"}
              </p>
              <div className="answers">
                <Button
                  variant="secondary"
                  onClick={() =>
                    handleAnswerClick(cardData.answer1, cardData.correctAnswer)
                  }
                  className="answer-button"
                >
                  {cardData?.answer1}
                </Button>
                <Button
                  variant="secondary"
                  onClick={() =>
                    handleAnswerClick(cardData.answer2, cardData.correctAnswer)
                  }
                  className="answer-button"
                >
                  {cardData?.answer2}
                </Button>
              </div>
            </div>
          )}
        </Modal.Body>
        <Modal.Footer>
          {!showQuestion && (
            <Button variant="secondary" onClick={handleContinue}>
              המשך
            </Button>
          )}
        </Modal.Footer>
      </Modal>

      <Modal
        show={showCorrectAnswerModal}
        onHide={handleCloseCorrectAnswerModal}
      >
        <Modal.Header>
          <Modal.Title>תשובה נכונה!</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <p>כל הכבוד! ענית נכון על השאלה.</p>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="primary" onClick={handleCloseCorrectAnswerModal}>
            סגור
          </Button>
        </Modal.Footer>
      </Modal>

      <Modal
        show={showIncorrectAnswerModal}
        onHide={handleCloseIncorrectAnswerModal}
      >
        <Modal.Header>
          <Modal.Title>תשובה אינה נכונה</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <p>אוי, טעית. נסה שוב בפעם הבאה.</p>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="primary" onClick={handleCloseIncorrectAnswerModal}>
            סגור
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};

export default DidYouKnow;

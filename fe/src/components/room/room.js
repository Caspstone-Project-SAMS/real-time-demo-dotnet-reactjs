import { Col, Container, Row } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import WaitingRoom from './waitingroom';
import { useState } from 'react';
import ChatRoom from './ChatRoom';

function App({SendMessage, UserName, Room, Messages, RoomName, setRoom, Conn, setConn}) {

  const JoinRoom = (roomName) => {
    setRoom(roomName);
    setConn();
    const message = {
      Event: "JoinRoom",
      RoomName: roomName
    }
    SendMessage({ message: message});
  }

  const SendMessageToRoom = (message) => {
    const sendMsg = {
      RoomName: RoomName,
      Data: message,
      Event: "SendToRoom"
    };
    SendMessage({message: sendMsg});
  }

  return (
    <div>
      <main>
        <Container>
          <Row className='px-5 my-5'>
            <Col sm="12">
              <h1 className='font-weight-light'>{Room}</h1>
            </Col>
          </Row>
          { !Conn
           ? <WaitingRoom JoinChatRoom={JoinRoom} Username={UserName}></WaitingRoom>
           : <ChatRoom messages={Messages} sendMessage={SendMessageToRoom} roomName={RoomName}></ChatRoom>
          }
        </Container>
      </main>
    </div>
  );
}

export default App;

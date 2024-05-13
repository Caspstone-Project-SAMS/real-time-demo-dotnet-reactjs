import React from "react";
import Chat from "./chat/Chat";
import Room from "./room/room";
import { useEffect, useState } from "react";

var socket;
var room1 = "", room2 = "";
var conn1 = false, conn2 = false;

export default ({ Username }) => {
  const [information, setInformation] = useState("Ready to connect");
  const [chatMessages, setChatMessages] = useState([]);
  const [roomMessages1, setRoomMessages1] = useState([]);
  const [roomMessages2, setRoomMessages2] = useState([]);

  const sendMessage = ({message}) => {
    var jsonString = JSON.stringify(message);
    socket.send(jsonString);
  };

  const sendMessageToRoom = (message) => {
    switch (message.RoomName){
      case room1:
        setRoomMessages1(messages => [...messages, message])
        break;
      case room2:
        setRoomMessages2(messages => [...messages, message])
        break;
      default:
        break;
    }
  };

  function setRoom1(name) {
    room1 = name;
  }
  function setRoom2(name) {
    room2 = name;
  }
  function connectRoom1() {
    conn1 = true;
  }
  function connectRoom2() {
    conn2 = true;
  }

  function activeWebSocket() {
    socket = new WebSocket("wss://localhost:5000/ws?username=" + Username);
    setInformation("Connecting");

    socket.onopen = function (event) {
      setInformation("Connected");
    };

    socket.onclose = function (event) {
      setInformation("Connection closed");
    };

    socket.onmessage = function (event) {
      var message = JSON.parse(event.data);
      switch (message.Event) {
        case "JoinRoom":
          sendMessageToRoom(message);
          break;
        case "MessageToRoom":
          sendMessageToRoom(message);
          break;
        case "MessageToReceiver":
          setChatMessages(messages => [...messages, message]);
          break;
        case "MessageToAll":
          break;
        case "NotifyToRoom":
          break;
        case "NotifyToReceiver":
          break;
        case "NotifyToAll":
          break;
      }
    };
  }

  return (
    <div>
      <div>
        <label>WebSocket Server URL: </label>
        <input readOnly value="wss://localhost:5000/ws" />
        <input readOnly value={Username} />
        <button
          className="btn btn-primary btn-sm"
          onClick={(_) => activeWebSocket()}
        >
          Connect
        </button>
      </div>
      <h1>{information}</h1>
      <Room SendMessage={sendMessage} UserName={Username} Room={"Room 1"} Messages={roomMessages1} RoomName={room1} setRoom={setRoom1} Conn={conn1} setConn={connectRoom1}/>
      <Room SendMessage={sendMessage} UserName={Username} Room={"Room 2"} Messages={roomMessages2} RoomName={room2} setRoom={setRoom2} Conn={conn2} setConn={connectRoom2}/>
      <Chat SendMessage={sendMessage} UserName={Username} Messages={chatMessages} SetMessages={setChatMessages} />
    </div>
  );
};

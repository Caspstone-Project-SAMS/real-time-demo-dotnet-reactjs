import React, { useState, useEffect } from "react";
import "./Chat.css";
import Message from "./Message";

export default ({SendMessage, UserName, Messages, SetMessages}) => {

  const [message, setMessage] = useState("");
  const [username, setUsername] = useState("");

  return (
  <div>
    <div className="wrapper">
      <div className="card border-primary">
        <h5 className="card-header bg-primary text-white">
          <i className="fas fa-comment"></i> Personal Chat
        </h5>
        <div className="card-body overflow-auto">
          {Messages.map((msg, index) => (
            <Message
              key={index}
              userName={msg.From}
              message={msg.Data}
            />
          ))}
        </div>
        <div className="card-footer border-primary p-0">
          <div className="input-group">
            <input
              value={username}
              onChange={e => {
                setUsername(e.target.value);
              }}
              type="text"
              className="form-control input-sm"
              placeholder="Username..."
            />
            <input
              value={message}
              onChange={e => {
                setMessage(e.target.value);
              }}
              type="text"
              className="form-control input-sm"
              placeholder="Type your message here..."
            />
            <button
              className="btn btn-primary btn-sm"
              onClick={_ => {
                const msg = {
                  Event: "SendToReceiver",
                  Data: message,
                  Receiver: username
                };
                const sendMsg = {
                  Data: message,
                  From: UserName
                }
                setMessage("");
                SetMessages(messages => [...messages, sendMsg])
                SendMessage({ message: msg});
              }}
            >
              Send
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
  );
};

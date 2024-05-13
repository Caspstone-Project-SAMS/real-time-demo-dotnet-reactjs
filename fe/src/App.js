import React from "react";
import Test from "./components/test";
import { useEffect, useState } from "react";

export default () => {
  const [userName, setUserName] = useState("");

  useEffect(() => {
    const uName = prompt("Name?");
    if (uName) {
      setUserName(uName);
    }
  }, []);

  return <Test Username={userName}/>;
};

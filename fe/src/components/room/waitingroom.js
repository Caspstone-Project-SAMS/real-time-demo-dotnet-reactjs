import { useState } from "react";
import { Form, Col, Button, Row } from 'react-bootstrap';

const WaitingRoom = ({ JoinChatRoom, Username }) => {
    const[chatroom, setChatroom] = useState("");

    return <Form onSubmit={e => {
        e.preventDefault();
        JoinChatRoom(chatroom);
    }}>
        <Row className="px-5 py-5">
            <Col sm={12}>
                <Form.Group>
                    {/* <Form.Control value={Username}/> */}

                    <Form.Control placeholder='Chat Room' 
                        onChange={e => setChatroom(e.target.value)}/>
                </Form.Group>
            </Col>
            <Col sm={12}>
                <hr/>
                <Button variant='success' type='submit'>Join</Button>
            </Col>
        </Row>
    </Form>
}

export default WaitingRoom;
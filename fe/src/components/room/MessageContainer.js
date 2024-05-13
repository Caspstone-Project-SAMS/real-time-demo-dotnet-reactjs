const MessageContainer = ({messages}) => {
    return (<div>
        <table>
            {messages?.map((msg, index) => 
                <tbody key={index}>
                    <tr>
                        <td>{msg.Data} - {msg.From}</td>
                    </tr>
                </tbody>
            )}
        </table>
    </div>)
}

export default MessageContainer;
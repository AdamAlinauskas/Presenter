window.onload = () => {
    Notification.requestPermission();
    
    const messageForm = document.getElementById('add-message');
    messageForm.onsubmit = (event) => {
        event.preventDefault();

        const data = new FormData(messageForm);
        if (!data.get('message')) return;

        fetch(
            "Conversation/AddMessage",
            {
                method: 'POST',
                body: data,
                credentials: 'include'
            }
        );

        document.getElementById('message').value = '';
    };

    // This bit could be much better done by react, angular, whatever
    function render(messages) {
        messages.forEach(message => {
            let elem = document.getElementById(`message-${message.id}`);

            if (elem) {
                populateMessageNode(elem, message);
            } else {
                elem = document.getElementById('message-template').content;
                populateMessageNode(elem, message);
                const messagesElem = document.getElementById('messages');
                messagesElem.appendChild(document.importNode(elem, true));
            }
        });
    }

    function populateMessageNode(elem, message) {
        elem.querySelector('.mt-author').innerText = message.author_name;
        elem.querySelector('.mt-message').innerText = message.text;
    }

    let lastSeenEvent = -1;
    function receiveMessages() {
        let success = true;

        fetch(
            `Conversation/GetMessages?conversationId=${conversationId}&since=${lastSeenEvent}`,
            {
                credentials: 'include'
            }
        ).then(
            raw => raw.json().then(response => {
                lastSeenEvent = response.lastEventId;
                render(response.messages);
                setTimeout(receiveMessages, 500);
            })
        ).catch(
            (err) => { 
                console.log(err);
                new Notification('Something went wrong');
            }
        );
    };
    setTimeout(receiveMessages, 500);
};
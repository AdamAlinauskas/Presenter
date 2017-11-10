window.onload = () => {
    Notification.requestPermission();
    let connection;

    // TODO AP : Can refactor these two methods into one
    const messageForm = document.getElementById('add-message');
    messageForm.onsubmit = (event) => {
        event.preventDefault();

        const messageBox = document.getElementById('mt-message');
        const text = messageBox.value;
        connection.invoke(
            'PostMessage',
            conversationInfo.id,
            conversationInfo.schema,
            text
        );

        messageBox.value = '';
    };

    function addReply(event) {
        event.preventDefault();
        var messageForm = event.target;
        var messageInput = messageForm.querySelector('.mt-message-input');

        const data = new FormData(messageForm);
        if (!data.get('message')) return;

        fetch(
            'Conversation/AddReply',
            {
                method: 'POST',
                body: data,
                credentials: 'include'
            }
        );

        messageInput.value = '';
        messageForm.classList.add('is-hidden');
    }

    function boost(messageId, button) {
        return (event) => {
            const isBoosted = button.classList.contains('mt-boosted');
            const action = isBoosted ? 'RemoveBoost' : 'Boost';
            const url = `Conversation/${action}/${messageId}`;
            
            connection.invoke(
                'BoostMessage',
                conversationInfo.id,
                conversationInfo.schema,
                messageId
            );
        };
    }

    // This bit could be much better done by react, angular, whatever
    function render(messages) {
        let newMessage = false;
        const messagesElem = document.getElementById('messages');

        messages.forEach(message => {
            const messageId = `message-${message.messageId}`;
            let elem = document.getElementById(messageId);

            if (elem) {
                populateMessageNode(elem, message);
            } else {
                if (message.repliesToId) {
                    elem = document.getElementById('message-reply-template').content;
                    populateMessageNode(elem, message);
                    const parentElem = document.querySelector(`#message-${message.repliesToId} .media-content`);
                    parentElem.appendChild(document.importNode(elem, true));
                } else {
                    elem = document.getElementById('message-template').content;
                    populateMessageNode(elem, message);
                    messagesElem.appendChild(document.importNode(elem, true));
                    wireUpMessage(messagesElem.querySelector(`#${messageId}`), message);
                    newMessage = true;
                }
            }
        });

        if (newMessage) {
            messagesElem.scrollTop = messagesElem.scrollHeight;
        }
    }

    function populateMessageNode(elem, message) {
        elem.firstElementChild.setAttribute('id', `message-${message.messageId}`);
        elem.querySelector('.mt-author').innerText = message.author;
        elem.querySelector('.mt-message').innerText = message.text;
        elem.querySelector('.mt-author-picture').setAttribute('src', message.authorPicture);
        if (!message.repliesToId) {
            elem.querySelector('.mt-boost-count').innerText = message.boosts || '';
            elem.querySelector('.mt-boost').classList.toggle('mt-boosted', message.boostedByCurrentUser);
        }
    }

    function wireUpMessage(elem, message) {
        if (conversationInfo.isMod) {
            elem.querySelector('.mt-message-id').value = message.messageId;
            var replyForm = elem.querySelector('.mt-reply-form');
            replyForm.onsubmit = addReply;
            elem.querySelector('.mt-reply').onclick = (event) => {
                event.preventDefault();
                replyForm.classList.toggle('is-hidden');
            }
        } else {
            elem.querySelector('.mt-reply-button').classList.add('is-hidden');
            const boostButton = elem.querySelector('.mt-boost');
            boostButton.onclick = boost(message.messageId, boostButton);
        }
    }

    // -----------------------------

    connection = new signalR.HubConnection('/JoinConversation');
    connection.on('message', message => render([message]));
    connection.start().then(() => connection.invoke('JoinConversation', conversationInfo.id));

    fetch(
        `Conversation/GetMessages?conversationId=${conversationInfo.id}&since=-1`,
        {
            credentials: 'include'
        }
    ).then(
        raw => raw.json().then(response => {
            render(response.messages);
        })
        ).catch(
        (err) => {
            console.log(err);
            new Notification('Something went wrong');
        }
    );
};
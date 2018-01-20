window.onload = () => {
    Notification.requestPermission();
    let connection;

    // TODO AP : Can refactor send message and reply methods into one
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

    function addReply(messageId) {
        return function (event) {
            event.preventDefault();
            const messageForm = event.target;
            const messageInput = messageForm.querySelector('.mt-message-input');
            const message = messageInput.value;
            
            if (!message) return;

            connection.invoke(
                'AddReply',
                conversationInfo.id,
                conversationInfo.schema,
                messageId,
                message
            );

            messageInput.value = '';
            messageForm.classList.add('is-hidden');
        }
    }

    function boost(messageId, button) {
        return (event) => {
            const isBoosted = button.classList.contains('mt-boosted');
            const action = isBoosted ? 'RemoveBoost' : 'BoostMessage';
            
            connection.invoke(
                action,
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
            var replyForm = elem.querySelector('.mt-reply-form');
            replyForm.onsubmit = addReply(message.messageId);
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

    function start() {
        connection = new signalR.HubConnection('/JoinConversation');

        connection.on('message', message => render([message]));
        connection.connection.onclose = () => {
            setTimeout(start, 1000);
        };
        connection.start()
            .then(() => connection.invoke('JoinConversation', conversationInfo.id))
            .catch(() => setTimeout(start, 1000));
    }
    start();

    fetch(
        `/Deck/GetMessages?conversationId=${conversationInfo.id}&since=-1`,
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
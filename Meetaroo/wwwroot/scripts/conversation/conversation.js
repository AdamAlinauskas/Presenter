window.onload = () => {
    Notification.requestPermission();

    // TODO AP : Can refactor these two methods into one
    const messageForm = document.getElementById('add-message');
    messageForm.onsubmit = (event) => {
        event.preventDefault();

        const data = new FormData(messageForm);
        if (!data.get('message')) return;

        fetch(
            'Conversation/AddMessage',
            {
                method: 'POST',
                body: data,
                credentials: 'include'
            }
        );

        document.getElementById('message').value = '';
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

    function boost(messageId) {
        return (event) => {
            const isBoosted = event.target.classList.contains('mt-boosted');
            const action = isBoosted ? 'RemoveBoost' : 'Boost';
            const url = `Conversation/${action}/${messageId}`;
            
            fetch(url, { credentials: 'include' });
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
            elem.querySelector('.mt-boost').onclick = boost(message.messageId);
        }
    }

    let lastSeenEvent = -1;
    function receiveMessages() {
        let success = true;

        fetch(
            `Conversation/GetMessages?conversationId=${conversationInfo.id}&since=${lastSeenEvent}`,
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
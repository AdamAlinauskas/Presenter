window.onload = () => {
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
    };
};
window.addEventListener('load', () => {
    document.getElementById('mt-file').onchange = (evt) => {
        document.getElementById('mt-filename').innerText = evt.target.value.replace('C:\\fakepath\\', '');
        document.getElementById('mt-create-presentation').attributes.removeNamedItem('disabled');
    };
});
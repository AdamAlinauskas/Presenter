window.addEventListener('load', () => {
    const badChars = /[^a-zA-Z0-9]/g

    const orgnameForm = document.getElementById('mt-orgname-form');
    const orgnameInput = document.getElementById('mt-orgname-input');
    const schemaDisplay = document.getElementById('mt-schema-display');

    const presentationForm = document.getElementById('mt-presentation-form');

    const schemaInputs = Array.prototype.slice.call(document.getElementsByClassName('mt-schema'));

    orgnameInput.onchange = orgnameInput.onkeyup = (event) => {
        const orgName = orgnameInput.value;
        const schemaName = orgName.replace(badChars, '').toLowerCase();
        schemaInputs.forEach(input => input.value = schemaName);
        schemaDisplay.innerText = schemaName;
    };

    orgnameForm.addEventListener('submit', (e) => {
        e.preventDefault();
        orgnameForm.classList.add('hidden');
        fetch(
            `/Signup/CreateOrg`,
            {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    displayName: orgnameInput.value,
                    schemaName: schemaInputs[0].value
                })
            }
        ).then(() => {
            presentationForm.classList.remove('hidden');
        });
    });
});
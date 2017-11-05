window.onload = () => {
    const badChars = /[^a-zA-Z0-9]/g

    const orgInput = document.getElementById('mt-org-name');
    const schemaInput = document.getElementById('mt-schema');
    const schemaDisplay = document.getElementById('mt-schema-display');

    orgInput.onchange = orgInput.onkeyup = (event) => {
        const orgName = orgInput.value;
        const schemaName = orgName.replace(badChars, '').toLowerCase();
        schemaInput.value = schemaName;
        schemaDisplay.innerText = schemaName;
    };
};

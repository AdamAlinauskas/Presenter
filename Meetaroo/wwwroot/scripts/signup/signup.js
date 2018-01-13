window.addEventListener('load', () => {
    const badChars = /[^a-zA-Z0-9]/g;
    let schemaName = '';

    const orgnameForm = document.getElementById('mt-orgname-form');
    const orgnameInput = document.getElementById('mt-orgname-input');
    const schemaDisplay = document.getElementById('mt-schema-display');

    const uploadPresentationForm = document.getElementById('mt-upload-presentation');

    const namePresentationForm = document.getElementById('mt-name-presentation-form');
    const presentationNameInput = document.getElementById('mt-presentation-name');

    const doneText = document.getElementById('mt-done');
    const dashboardLink = document.getElementById('mt-dashboard-link');

    let documentId;

    // -------
    const svgDoc = document.getElementById('signup-image').contentDocument;
    const orgnameArea = svgDoc.getElementById('orgname_container').getBoundingClientRect();
    orgnameInput.style.left = orgnameArea.x + 'px';
    orgnameInput.style.top = orgnameArea.y + 'px';
    orgnameInput.style.width = orgnameArea.width + 'px';
    orgnameInput.style.height = orgnameArea.height + 'px';
    orgnameInput.focus();
    const urlPreview = svgDoc.getElementById('url_preview');
    const companyNamePreview = svgDoc.getElementById('company_name_preview');

    const baseLayer = svgDoc.getElementById('base_layer');
    const objectsLayer = svgDoc.getElementById('objects_layer');

    const animateOutOrg = () => {
        baseLayer.style['animation-name'] = 'slideLeftBase';
        objectsLayer.style['animation-name'] = 'slideLeftObjects';
    };
    
    // -------

    const updateOrgName = (event) => {
        const orgName = orgnameInput.value;
        schemaName = orgName.replace(badChars, '').toLowerCase();
        // schemaDisplay.innerText = schemaName;
        urlPreview.textContent = schemaName.length > 0 ? schemaName + ".findecks.com" : "findecks.com";
        companyNamePreview.textContent = orgName;
    };

    const createOrg = (e) => {
        e.preventDefault();
        orgnameForm.classList.add('hidden');

        fetch('/Signup/CreateOrg', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                displayName: orgnameInput.value,
                schemaName
            })
        }).then(uploadPresentation);
    };

    const uploadPresentation = () => {
        const dz = new Dropzone('#mt-upload-presentation', {
            url: '/Signup/UploadPresentation?schemaName=' + schemaName,
            uploadMultiple: false
        });
        dz.on('success', (file, response) => {
            documentId = response.documentId;
            uploadPresentationForm.classList.add('hidden');
            namePresentationForm.classList.remove('hidden');
        });
        uploadPresentationForm.classList.remove('hidden');
    };

    const namePresentation = (e) => {
        e.preventDefault();
        fetch('/Signup/CreatePresentation', {
            method: 'POST',
            credentials: 'include',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                presentationName: presentationNameInput.value,
                documentId,
                schemaName
            })
        }).then(() => {
            namePresentationForm.classList.add('hidden');
            doneText.classList.remove('hidden');
            dashboardLink.setAttribute('href', `${location.protocol}//${schemaName}.${location.host}`);
        });
    };

    orgnameInput.onchange = orgnameInput.onkeyup = updateOrgName;
    orgnameForm.addEventListener('submit', createOrg);
    orgnameForm.addEventListener('submit', animateOutOrg);
    namePresentationForm.addEventListener('submit', namePresentation);
});
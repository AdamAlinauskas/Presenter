window.addEventListener('load', () => {
    const badChars = /[^a-zA-Z0-9]/g;
    let schemaName = '';
    let orgName;
    let presentationName;
    let documentId;

    const orgnameForm = document.getElementById('mt-orgname-form');
    const orgnameInput = document.getElementById('mt-orgname-input');
    const schemaDisplay = document.getElementById('mt-schema-display');

    const uploadPresentationForm = document.getElementById('mt-upload-presentation');

    const namePresentationForm = document.getElementById('mt-name-presentation-form');
    const presentationNameInput = document.getElementById('mt-presentation-name');

    // -------
    const svgDoc = document.getElementById('signup-image').contentDocument;
    fitHtmlElementToSvgElement(orgnameInput, svgDoc.getElementById('orgname_container'));
    orgnameInput.focus();
    const urlPreview = svgDoc.getElementById('url_preview');
    const companyNamePreview = svgDoc.getElementById('company_name_preview');
    const dropArea = svgDoc.getElementById('drop_area');
    const dropText = svgDoc.getElementById('drop_text');
    const presentationNameArea = svgDoc.getElementById('presentation_name_area');
    const presentationNameText = svgDoc.getElementById('presentation_name_text');
    const presentationHintText = svgDoc.getElementById('presentation_hint_text');
    const progressBar = svgDoc.getElementById('progress_bar');
    const uploadDone = svgDoc.getElementById('upload_done');

    const doneOrgname = svgDoc.getElementById('done_orgname');
    const donePresentationname = svgDoc.getElementById('done_presentationname');
    const doneLink = svgDoc.getElementById('done_link');
    const doneButton = svgDoc.getElementById('done_button');

    const baseLayer = svgDoc.getElementById('base_layer');
    const objectsLayer = svgDoc.getElementById('objects_layer');

    const animateOutOrg = () => {
        baseLayer.style['animation-name'] = 'slideLeftBase';
        objectsLayer.style['animation-name'] = 'slideLeftObjects';
        baseLayer.addEventListener('animationend', () => {
            fitHtmlElementToSvgElement(uploadPresentationForm, svgDoc.getElementById('drop_target'));
            fitHtmlElementToSvgElement(presentationNameInput, presentationNameArea);
            console.log('animation finished');
        });
    };
    
    function fitHtmlElementToSvgElement(htmlElement, svgElement) {
        const area = svgElement.getBoundingClientRect();
        htmlElement.style.left = area.x + 'px';
        htmlElement.style.top = area.y + 'px';
        htmlElement.style.width = area.width + 'px';
        htmlElement.style.height = area.height + 'px';
    }
    // -------

    const updateOrgName = (event) => {
        orgName = orgnameInput.value;
        schemaName = orgName.replace(badChars, '').toLowerCase();
        urlPreview.textContent = schemaName.length > 0 ? schemaName + ".findecks.com" : "findecks.com";
        companyNamePreview.textContent = orgName;
    };

    const updatePresentationName = (event) => {
        presentationName = presentationNameInput.value;
        presentationNameText.textContent = presentationName;
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
            presentationNameInput.focus();
            presentationHintText.textContent = "And give it a title…";
            progressBar.style.strokeOpacity = 0;
            uploadDone.style.opacity = 1;
            presentationNameArea.opacity = 1;
        });
        dz.on('dragenter', () => {
            dropArea.style.fillOpacity = 0.3;
        });
        dz.on('dragleave', () => {
            dropArea.style.fillOpacity = 0;
        });
        dz.on('sending', () => {
            dropArea.style.fillOpacity = 0;
            dropText.style.opacity = 0;
            progressBar.style.strokeOpacity = 1;
        });
        dz.on('uploadprogress', (e, progress) => {
            const strokeLength = progress * 7;
            progressBar.style.strokeDasharray = strokeLength + ',1000';
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

            doneOrgname.textContent = orgName;
            donePresentationname.textContent = presentationName;
            doneLink.textContent = `${location.protocol}//${schemaName}.${location.host}/p/${documentId}`;
            doneButton.onclick = () => window.location = `${location.protocol}//${schemaName}.${location.host}`;

            baseLayer.style['animation-name'] = 'slideLeftBase2';
            objectsLayer.style['animation-name'] = 'slideLeftObjects2';
        });
    };

    orgnameInput.onchange = orgnameInput.onkeyup = updateOrgName;
    presentationNameInput.onchange = presentationNameInput.onkeyup = updatePresentationName;
    orgnameForm.addEventListener('submit', createOrg);
    orgnameForm.addEventListener('submit', animateOutOrg);
    namePresentationForm.addEventListener('submit', namePresentation);
});
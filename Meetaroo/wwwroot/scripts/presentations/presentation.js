var PdfDocument = function (options) {
    var me = this;

    var url = options.url;
    var targetDiv = options.targetDiv;
    var presentationName = options.presentationName;
    var hasNextPrevious = options.hasNextPrevious;
    var presentationKey = options.presentationKey;
    var connection = null;
    this.onPageChange = null;
    this.onRenderLastPage = null;

    me.showPdf = function () {
        $(targetDiv).show();
        me.reRenderCurrentPage();//re-render to fit div.
    }

    me.hidePdf = function () {
        $(targetDiv).hide();
    }

    /*returns promoise */
    me.renderToView = function (renderHidden) {
        if (renderHidden) {
            $(targetDiv).hide();
        }

        var canvas = document.createElement('canvas');
        canvas.className = 'pdf-canvas';
        var canvasContainer = document.createElement('div');
        canvasContainer.id = 'canvas-container';
        canvasContainer.append(canvas);
        var nextButton = null;
        var previousButton = null;
        var buttonAreaWithPageNumber = document.createElement("div");

        if (hasNextPrevious) {
            nextButton = document.createElement('button');
            nextButton.id = 'next';
            nextButton.innerText = "Next";

            previousButton = document.createElement('button');
            previousButton.id = 'previous';
            previousButton.innerText = "Previous";

            buttonAreaWithPageNumber.append(previousButton);
            buttonAreaWithPageNumber.append(nextButton);
        }

        var currentPageNumberArea = document.createElement('span');
        buttonAreaWithPageNumber.append(currentPageNumberArea);
        var pageTitle = document.createElement('h1');
        pageTitle.innerText = presentationName;

        targetDiv.append(pageTitle);
        targetDiv.append(buttonAreaWithPageNumber);
        targetDiv.append(canvasContainer);

        return RenderPdf(canvasContainer, canvas, currentPageNumberArea, nextButton, previousButton);
    }

    /*Returns a promoise*/
    var RenderPdf = function (canvasContainer, canvas, currentPageNumberArea, nextButton, previousButton) {
        // The workerSrc property shall be specified.
        PDFJS.workerSrc = '/scripts/pdfjs/pdf.worker.js';

        var pdfDoc = null,
            pageNum = 1,
            pageRendering = false,
            pageNumPending = null,
            scale = 1.0,
            ctx = canvas.getContext('2d');

        /**
        * Get page info from document, resize canvas accordingly, and render page.
        * param num Page number.
        */
        function renderPage(num) {

            if (pdfDoc == null)
                return;

            if (num == pdfDoc.numPages && me.onRenderLastPage) {
                me.onRenderLastPage();
            }

            pageRendering = true;
            pageNum = num;
            if (me.onPageChange) {
                me.onPageChange(num);
            }
            // Using promise to fetch the page
            pdfDoc.getPage(num).then(function (page) {
                /*Responsiveness from 
                https://stackoverflow.com/questions/35987398/pdf-js-how-to-make-pdf-js-viewer-canvas-responsive/37870384#37870384
                */
                /*Sets the current page when the pdf is rendered.
                We could re-render the PDF when the screen size changes*/
                var viewport = page.getViewport(1);
                var scale = canvasContainer.clientWidth / viewport.width;
                viewport = page.getViewport(scale);
                canvas.height = viewport.height;
                canvas.width = viewport.width;

                // Render PDF page into canvas context
                var renderContext = {
                    canvasContext: ctx,
                    viewport: viewport
                };
                var renderTask = page.render(renderContext);

                // Wait for rendering to finish
                renderTask.promise.then(function () {
                    pageRendering = false;
                    if (pageNumPending !== null) {
                        // New page rendering is pending
                        renderPage(pageNumPending);
                        pageNumPending = null;
                    }
                });
            });

            // Update page counters
            updatePageCount();
        }

        function updatePageCount() {
            currentPageNumberArea.textContent = "Page " + pageNum + " / " + pdfDoc.numPages;
        }

        /**
        * If another page rendering in progress, waits until the rendering is
        * finised. Otherwise, executes rendering immediately.
        */
        function queueRenderPage(num) {
            if (pageRendering) {
                pageNumPending = num;
            } else {
                renderPage(num);
            }
        }

        me.render = queueRenderPage;

        me.reRenderCurrentPage = function () {
            queueRenderPage(pageNum);
        }

        /**
        * Displays previous page.
        */
        function onPrevPage() {
            if (pageNum <= 1) {
                return;
            }
            pageNum--;
            queueRenderPage(pageNum);
        }

        if (previousButton)
            previousButton.addEventListener('click', onPrevPage);

        /**
        * Displays next page.
        */
        function onNextPage() {
            console.log("next page");
            if (pageNum >= pdfDoc.numPages) {
                return;
            }
            pageNum++;
            queueRenderPage(pageNum);
        }


        if (nextButton)
            nextButton.addEventListener('click', onNextPage);

        /**
        * Asynchronously downloads PDF.
        */
        return PDFJS.getDocument(url).then(function (pdfDoc_) {
            pdfDoc = pdfDoc_;

            updatePageCount();
            // Initial/first page rendering
            renderPage(pageNum);
        });
    }
}

var Presentation = function (options) {
    var connection;

    this.start = function () {
        statusChanged(options.presentationStatus);
        joinPresenation();
        wireupCallbacks();
    }

    const start = this.start;

    var joinPresenation = function () {
        connection = new signalR.HubConnection('/ViewPresentation');

        connection.on('setPage', pageNumber => {
            //Not a presenter then follow along
            if (!options.isPresenter) {
                console.log(pageNumber);
                options.pdfDocument.render(pageNumber);
            }
        });

        connection.on('StatusChangedTo', newStatus => {
            statusChanged(newStatus);
        });

        connection.connection.onclose = () => {
            setTimeout(start, 1000);
        }

        connection.start()
            .then(() => connection.invoke('JoinPresentation', options.schema, options.presentationId, options.presentationKey))
            .catch(() => setTimeout(start, 1000));
    }

    var statusChanged = function (newStatus) {
        if (newStatus == 1) {
            hideAllAreas();
            if (options.isPresenter) {
                $(options.startPresentationArea).removeClass('fd-hidden');
            }
            else {
                $(options.presentationWillStartShortlyArea).removeClass('fd-hidden');
            }
        }

        if (newStatus == 2) {
            hideAllAreas();
            if (options.pdfDocument != null) {
                options.pdfDocument.showPdf();
            }
            if (options.inProgressStartedCallBack)
                options.inProgressStartedCallBack();
        }

        if (newStatus == 3) {
            hideAllAreas();
            //record duration
            if (options.presentationCompleteCallBack)
                options.presentationCompleteCallBack();
            if (options.isPresenter) {
                options.pdfDocument.hidePdf();
                $(options.presentationIsCompleteArea).removeClass('fd-hidden');
            }
            else {
                options.pdfDocument.hidePdf();
                $(options.presentationIsCompleteArea).removeClass('fd-hidden');
            }
        }
    }

    var hideAllAreas = function () {
        $(options.presentationWillStartShortlyArea).addClass('fd-hidden');
        $(options.presentationIsCompleteArea).addClass('fd-hidden');
        $(options.startPresentationArea).addClass('fd-hidden');
        $(options.stopPresentationArea).addClass('fd-hidden');
    }

    var wireupCallbacks = function () {
        if (options.isPresenter) {

            options.pdfDocument.onPageChange = function (page) {
                connection.invoke('SetCurrentPage', options.schema, options.presentationId, page, options.presentationKey);
            }

            options.pdfDocument.onRenderLastPage = function () {
                $(options.stopPresentationArea).removeClass('fd-hidden');
            }
        }
    }

    var init = function () {
        //statusChanged(options.presentationStatus);

        if (options.isPresenter) {
            $(options.startPresentationButton).click(function () {
                connection.invoke('ChangePresentationStatusToStarted', options.schema, options.presentationId, options.presentationKey);
            });

            $(options.stopPresentationButton).click(function () {
                connection.invoke('ChangePresentationStatusToPostPresentation', options.schema, options.presentationId, options.presentationKey);
            });
        }
    }

    init();


}


class Analytics {

    constructor(presentationId, trackPresentationUrl, updateSessionDurationUrl) {
        this.presentationId = presentationId;
        this.trackPresentationUrl = trackPresentationUrl;
        this.analyticsId = 0;
        this.updateSessionDurationUrl = updateSessionDurationUrl;
    }

    createAnalyticsRecord(position) {
        let latitude = null;
        let longitude = null;
        if (position && position.coords.latitude) {
            latitude = position.coords.latitude;
            longitude = position.coords.longitude;
            console.log("lat " + latitude + " long " + longitude);
        }
        $.post(
            this.trackPresentationUrl,
            { presentationId: this.presentationId, Latitude: latitude, Longitude: longitude },
            (data) => { console.log(data.analyticsId); this.analyticsId = data.analyticsId; }
        )
    }

    init() {
        if (navigator.geolocation && window.isSecureContext) {
            navigator.geolocation.getCurrentPosition(
                this.createAnalyticsRecord.bind(this),
                this.createAnalyticsRecord.bind(this)
            );
        }
        else {
            this.createAnalyticsRecord(null);
        }
    }

    stopRecordingDuration() {
        if (this.start == null)
            return;
        var end = new Date();
        var duration = end - this.start;
        //No support in IE, should add polyfill
        fetch(this.updateSessionDurationUrl, {
            method:"POST",
            body: JSON.stringify({ analyticsId: this.analyticsId, duration: duration }),
            headers: {"Content-Type":"application/json"},
            credentials: 'include'
        })
    }

    startRecordingDuration() {
        this.start = new Date();
    }
}
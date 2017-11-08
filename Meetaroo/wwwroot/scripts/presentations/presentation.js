var PdfDocument = function (options) {
    var me = this;

    var url = options.url;
    var targetDiv = options.targetDiv;
    var presentationName = options.presentationName;
    var hasNextPrevious = options.hasNextPrevious;
    var presentationKey = options.presentationKey;
    var connection = null;
    this.onPageChange = null;

    me.render = null;

    me.renderToView = function () {
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

        RenderPdf(canvasContainer, canvas, currentPageNumberArea, nextButton, previousButton);
    }

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
        PDFJS.getDocument(url).then(function (pdfDoc_) {
            pdfDoc = pdfDoc_;

            updatePageCount();
            // Initial/first page rendering
            renderPage(pageNum);
        });
    }
}

var Presentation = function (pdfDocument, isPresenter, presentationKey, presentationId, schema) {
    var connection;

    this.start = function () {
        joinPresenation();
        wireupCallbacks();
    }

    var joinPresenation = function () {
        connection = new signalR.HubConnection('/ViewPresentation');

        //Not a presenter then follow along
        if (!isPresenter) {
            connection.on('setPage', pageNumber => {
                console.log(pageNumber);
                pdfDocument.render(pageNumber);
            });
        }

        connection.start()
            .then(() => connection.invoke('JoinPresentation', schema, presentationId, presentationKey));
    }

    var wireupCallbacks = function () {
        if (isPresenter) {
            pdfDocument.onPageChange = function (page) {
                connection.invoke('SetCurrentPage', schema, presentationId, page, presentationKey);
            }
        }
    }
}

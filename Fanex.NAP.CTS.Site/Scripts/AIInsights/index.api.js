function AIInsightsAPI() {
    let idleTimer;
    let activeRequests = [];
    let globalTimeoutId = null;

    this.UploadFile = function (file, {
        onStart,
        onSuccess,
        onComplete,
        onError
    } = {}) {
        const formData = new FormData();
        const fileName = file.name;
        formData.append("uploadFile", file);
        variable.domElements.fileNameLabel.text(fileName);

        const doRequest = () => $.ajax({
            url: common.modifyRelativeUrl('AIInsights/UploadFile'),
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: onStart,
            error: function (xhr) {
                console.error('UploadFile error:', xhr);
                const message = xhr?.responseJSON?.message ?? "";
                onError?.(message);
            }
        });

        return RetryOnNetworkClose(
            doRequest,
            variable.CONFIG.RETRY_TIME,
            variable.CONFIG.RETRY_DELAY_IN_SECONDS * 1000,
            onError
        )
            .done(response => onSuccess?.(response, file.name))
            .fail(response => onError?.(response?.message ?? "Upload failed."))
            .always(onComplete);
    }

    this.AnalyzeSummary = function ({
        onStart,
        onSuccess,
        onComplete,
        onError
    } = {}) {
        const doRequest = () => $.ajax({
            url: common.modifyRelativeUrl('AIInsights/AnalyzeSummary'),
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            beforeSend: onStart,
            error: function (xhr) {
                console.error('AnalyzeSummary error:', xhr);
                const message = xhr?.responseJSON?.message ?? "";
                onError?.(message);
            }
        });

        return RetryOnNetworkClose(
            doRequest,
            variable.CONFIG.RETRY_TIME,
            variable.CONFIG.RETRY_DELAY_IN_SECONDS * 1000,
            onError
        )
            .done(response => onSuccess?.(response))
            .fail(response => onError?.(response?.message ?? "AnalyzeSummary failed."))
            .always(onComplete);
    }

    this.StartAnalyzeCharts = function ({
        onStart,
        onSuccess,
        onComplete,
        onError
    } = {}) {
        const doRequest = () => $.ajax(common.modifyRelativeUrl('AIInsights/StartAnalyzeCharts'), {
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            beforeSend: onStart
        });

        return RetryOnNetworkClose(
            doRequest,
            variable.CONFIG.RETRY_TIME,
            variable.CONFIG.RETRY_DELAY_IN_SECONDS * 1000,
            onError
        )
            .done(response => {
                onSuccess?.(response);
            })
            .fail(response => {
                const msg = response?.message ?? "Network or server error.";
                onError?.(msg);
            })
            .always(onComplete);
    };

    this.AskChatBot = function ({
        data,
        onStart,
        onSuccess,
        onComplete,
        onError
    } = {}) {
        const doRequest = () => $.ajax(common.modifyRelativeUrl('AIInsights/AskChatbot'), {
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            data: data,
            beforeSend: onStart,
            success: (response) => onSuccess?.(response),
            complete: onComplete,
            error: onError
        });

        return WrapAjaxWithGlobalTimeout(doRequest());
    };

    this.LoadHistoryChatMessage = function ({
        onSuccess,
        onError
    } = {}) {
        const doRequest = () => $.ajax(common.modifyRelativeUrl('AIInsights/LoadHistoryChatMessage'), {
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            success: (response) => onSuccess?.(response),
            error: (errMsg) => onError?.(errMsg)
        });

        return WrapAjaxWithGlobalTimeout(doRequest());
    }

    this.GetFileName = function ({
        onSuccess
    } = {}) {
        const doRequest = () =>
            $.ajax(common.modifyRelativeUrl('AIInsights/GetFileName'), {
                type: 'GET',
                async: false,
                headers: { "X-Tab-Id": common.getTabId() },
                success: (response) => onSuccess?.(response)
            });

        return WrapAjaxWithGlobalTimeout(doRequest());
    }

    this.AnalyzeChartsSSEConnection = function ({
        onMessage,
        onError,
        retries,
        delay,
        idleTimeout = 300,
    } = {}) {
        let retryCount = 0;
        let isClosed = false;

        let stopEventSource = this.StopEventSource;

        function resetIdleTimer() {
            if (idleTimer) clearTimeout(idleTimer);
            idleTimer = setTimeout(() => {
                console.warn(`⚠ Idle timeout (${idleTimeout}s). Retrying...`);
                handleEventSourceError(true); // true = timeout
            }, idleTimeout * 1000);
        }

        function onConnect() {
            isClosed = false;

            if (variable.evtSource) {
                variable.evtSource.close();
            }

            variable.evtSource = new EventSource(
                common.modifyRelativeUrl(`AIInsights/StreamAnalyzeCharts?streamToken=${variable.streamToken}`)
            );

            // variable.evtSource.onopen = () => console.log("✅ Connected");
            variable.evtSource.onerror = () => handleEventSourceError(false);
            variable.evtSource.onmessage = (e) => {
                if (!e.data.includes('"event: keepalive\\ndata: ping\\n\\n"')) {
                    resetIdleTimer();
                }
                onMessage?.(e);
            };
        }

        function handleEventSourceError(isTimeout) {
            if (isClosed) return;
            isClosed = true;

            stopEventSource();

            if (retryCount < retries) {
                retryCount++;
                console.warn(`⚠ Connection closed by ${isTimeout ? 'idle timeout' : 'error'}. Retry ${retryCount} of ${retries}`);
                setTimeout(onConnect, delay * 1000);
            } else {
                console.error("❌ Max retry reached. Connection failed.");
                onError?.();
            }
        }

        setTimeout(() => {
            onConnect();
        }, 500);
    };

    this.StopEventSource = function () {
        if (idleTimer) {
            clearTimeout(idleTimer);
            idleTimer = null;
        }

        if (variable.evtSource) {
            variable.evtSource.close();
            variable.evtSource = null;
        }
    }

    /**
     * Retry AJAX call if response.statuscode == HttpNetworkClose
     * @param {function} ajaxFn
     * @param {number} retries
     * @param {number} delay
     * @param {onError} handle Error
     * @returns {Promise} jQuery Deferred Promise
     */
    function RetryOnNetworkClose(ajaxFn, retries, delay, onError) {
        const deferred = $.Deferred();
        let currentRequest = null;

        function attempt(remaining) {
            currentRequest = ajaxFn();

            WrapAjaxWithGlobalTimeout(
                currentRequest
                    .done(response => {
                        if (response?.statuscode === variable.AIHttpStatusCode.Success) {
                            deferred.resolve(response);
                        }
                        else if (response?.statuscode === variable.AIHttpStatusCode.HttpNetworkClose && remaining > 0) {
                            console.warn(`⚠️ Network closed, retrying... (${retries - remaining + 1}/${retries})`);
                            setTimeout(() => attempt(remaining - 1), delay);
                        }
                        else {
                            deferred.reject(response);
                        }
                    })
                    .fail(xhr => {
                        console.error("AJAX failed:", xhr);
                        const msg = xhr?.responseJSON?.message ?? "Network or server error.";
                        onError?.(msg);
                        deferred.reject(xhr);
                    }));
        }

        attempt(retries);
        return deferred.promise();
    }

    this.StartGlobalTimeout = function (seconds = 600, onTimeout) {
        this.StopGlobalTimeout(); // cancel previous timer if running
        variable.globalTimeoutReached = false;

        globalTimeoutId = setTimeout(() => {
            variable.globalTimeoutReached = true;
            console.warn(`⛔ Global timeout reached (${seconds} seconds)!`);

            // Stop all active requests
            activeRequests.forEach(jqXHR => {
                if (jqXHR && jqXHR.readyState !== 4) { // readyState 4 = done
                    jqXHR.abort();
                }
            });
            activeRequests = [];
            // Stop SSE connection
            this.StopEventSource();

            if (typeof onTimeout === "function") onTimeout();
        }, seconds * 1000);
    };

    this.StopGlobalTimeout = function () {
        if (globalTimeoutId) {
            clearTimeout(globalTimeoutId);
            globalTimeoutId = null;
        }
        variable.globalTimeoutReached = false;

        // Stop SSE connection if manually stopping timeout
        this.StopEventSource();
    };

    function WrapAjaxWithGlobalTimeout(jqXHR) {
        activeRequests.push(jqXHR);

        // when request completes, remove it from activeRequests
        jqXHR.always(() => {
            const index = activeRequests.indexOf(jqXHR);
            if (index > -1) activeRequests.splice(index, 1);
        });

        return jqXHR;
    }
}

var api = new AIInsightsAPI();
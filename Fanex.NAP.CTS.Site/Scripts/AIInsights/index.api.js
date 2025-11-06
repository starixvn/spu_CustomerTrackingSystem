function AIInsightsAPI() {
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

        return RetryOnNetworkClose(doRequest, variable.CONFIG.RETRY_TIME, variable.CONFIG.RETRY_DELAY_IN_SECONDS * 1000, onError)
            .done(response => onSuccess?.(response, fileName))
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

        return RetryOnNetworkClose(doRequest, variable.CONFIG.RETRY_TIME, variable.CONFIG.RETRY_DELAY_IN_SECONDS * 1000, onError)
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

        return RetryOnNetworkClose(doRequest, variable.CONFIG.RETRY_TIME, variable.CONFIG.RETRY_DELAY_IN_SECONDS * 1000, onError)
            .done(response => {
                onSuccess?.(response);
            })
            .fail(response => {
                const msg = response?.message ?? "Network or server error.";
                onError?.(msg);
            })
            .always(onComplete);
    };

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

        function attempt(remaining) {
            ajaxFn()
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
                });
        }

        attempt(retries);
        return deferred.promise();
    }
}

var api = new AIInsightsAPI();
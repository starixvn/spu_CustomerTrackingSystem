function AIInsights() {
    // Initialization
    function Initialize() {
        variable.$contentWrapper = EnsureContentWrapperExists();

        BlockedLibraryPlotly();
        AttachEventListeners();
        UpdateFileNameFromServer();
        AnalyzeFile();
        LoadHistoryChatMessage();
    }

    // DOM Management
    function EnsureContentWrapperExists() {
        let $wrapper = variable.domElements.container.find('.ai-insights-content');

        if ($wrapper.length === 0) {
            $wrapper = $('<div class="ai-insights-content"></div>');
            MoveChildrenToWrapper($wrapper);
            variable.domElements.container.append($wrapper);
        }

        return $wrapper;
    }

    function MoveChildrenToWrapper($wrapper) {
        variable.domElements.container.children().each(function () {
            const $child = $(this);
            const isChatbotToggle = $child.hasClass('chatbot-toggle');
            const isChatbotPanel = $child.attr('id') === 'chatbotPanel';

            if (!isChatbotToggle && !isChatbotPanel) {
                $wrapper.append($child);
            }
        });
    }

    // View Management
    function ToggleSections(viewMode) {
        const isVisualization = viewMode === variable.ViewMode.VISUALIZATION;

        variable.domElements.divVisualization.toggleClass('hidden', !isVisualization);
        variable.domElements.divStatistics.toggleClass('hidden', isVisualization);

        if (!isVisualization) {
            ResizePlotlyCharts(variable.domElements.divStatistics);
        }
    }

    function ResizePlotlyCharts(container) {
        if (container) {
            container.find('.js-plotly-plot').each(function () {
                Plotly.Plots.resize(this);
            });
        }
    }

    // Event Listeners
    function AttachEventListeners() {
        variable.domElements.viewOptions.on('change', function () {
            ToggleSections($(this).val());
        });

        AttachFileUploadEvents();
        AttachChatbotEvents();

        ToggleSections(variable.ViewMode.VISUALIZATION);

        // Button event handlers
        variable.domElements.chatbotToggle.on('click', ToggleChatbot);
        variable.domElements.closeChatbot.on('click', ToggleChatbot);
        variable.domElements.btnGenerate.on('click', AnalyzeFile);
        variable.domElements.fileTitle.on('click', SaveLog);
        variable.domElements.fileNameLabel.on('click', ExportExcelConvertCsv);
    }

    // Chatbot Management
    function ToggleChatbot() {
        if (!variable.domElements.chatbotPanel.length || !variable.domElements.container.length) {
            return;
        }

        const isOpen = variable.domElements.chatbotPanel.hasClass('active');

        variable.domElements.chatbotPanel.toggleClass('active');
        variable.$contentWrapper.toggleClass('chatbot-open', !isOpen);
        variable.domElements.chatbotToggle.css('display', isOpen ? 'flex' : 'none');
        variable.domElements.closeChatbot.css('display', isOpen ? 'none' : 'flex');

        if (!isOpen) {
            setTimeout(ScrollToBottomChat, variable.CONFIG.CHAT_SCROLL_DELAY);
        }

        setTimeout(() => {
            ResizePlotlyCharts(variable.domElements.divVisualization);
        }, variable.CONFIG.CHART_RESIZE_DELAY);
    }

    // File Upload Management
    function AttachFileUploadEvents() {
        if (!variable.domElements.fileInput.length || !variable.domElements.fileNameLabel.length) {
            return;
        }

        const initialFileName = variable.domElements.fileNameLabel.text() || variable.CONFIG.NO_FILE_UPLOADED;
        variable.domElements.fileNameLabel.text(initialFileName);
        UpdateTitle();

        variable.domElements.fileInput.on('change', HandleFileSelection);
    }

    function HandleFileSelection(e) {
        variable.fileUpload = e.target.files[0];
        const file = variable.fileUpload;

        ResetDefault();
        variable.domElements.contentCardSection.hide();

        if (!file) {
            SetNoFileState();
            return;
        }

        if (!ValidateFile(file)) {
            return;
        }

        UploadFile(file);
    }

    function ValidateFile(file) {
        if (!variable.CONFIG.SUPPORTED_FILE_EXTENSIONS.test(file.name)) {
            common.showError('Please upload a .xls or .xlsx file!');
            variable.domElements.fileInput.val('');
            return false;
        }

        if (file.size > variable.CONFIG.MAX_FILE_SIZE_BYTES) {
            common.showError(`Exceeds the allowed limit of ${variable.CONFIG.MAX_FILE_SIZE_MB} MB`);
            variable.domElements.fileInput.val('');
            return false;
        }

        return true;
    }

    function SetNoFileState() {
        variable.domElements.fileNameLabel.text(variable.CONFIG.NO_FILE_UPLOADED);
        UpdateTitle();
    }

    function UpdateTitle() {
        const currentText = variable.domElements.fileNameLabel.text();
        variable.domElements.fileNameLabel.attr('title', currentText || variable.CONFIG.NO_FILE_UPLOADED);
    }

    function UploadFile(file) {
        api.UploadFile(file,
            {
                onStart: onStart,
                onSuccess: (res, fileName) => onSuccess(res, fileName),
                onComplete: onComplete,
                onError: (message) => onError(message)
            });

        function onStart() {
            common.showLoading();
            variable.domElements.btnGenerate.prop('disabled', true);
            variable.domElements.sendButton.prop('disabled', true);
        }

        function onSuccess(response, fileName) {
            variable.domElements.btnGenerate.prop('disabled', false);
            variable.domElements.sendButton.prop('disabled', false);
            variable.domElements.fileNameLabel.text(fileName);
            common.showSuccess('File uploaded successfully: ' + fileName, 2000);

            UpdateTitle();
        }

        function onComplete() {
            common.closeLoading();
            CleanUploadedFileInput();
        }

        function onError(msg) {
            common.closeLoading();

            variable.domElements.fileNameLabel.text(variable.CONFIG.NO_FILE_UPLOADED);
            common.showError("Upload failed " + msg);
        }

        function CleanUploadedFileInput() {
            if (variable.domElements.fileInput && variable.domElements.fileInput[0]) {
                variable.domElements.fileInput[0].value = "";
            }
            if (variable.domElements.fileInput) {
                variable.domElements.fileInput.val("");
            }
        }
    }

    // Analysis Functions
    function AnalyzeFile() {
        const fileName = variable.domElements.fileNameLabel.text().trim();

        if (fileName === variable.CONFIG.NO_FILE_UPLOADED || fileName === '') {
            return;
        }

        ResetDefault();
        ShowAnalysisLoading();
        SkipReCheckin();

        AnalyzeSummary()
            .done(function () {
                AnalyzeChartsGetSSEId()
                    .done(function () {
                        AnalyzeChartsSSEConnection();
                    });
                EnableReCheckin();
            });
    }

    function AnalyzeSummary() {
        return api.AnalyzeSummary({
            onStart: () => { },
            onSuccess: (res) => onSuccess(res),
            onComplete: onComplete,
            onError: (xhr) => onError(xhr)
        });

        function onSuccess(res) {
            variable.dataLogSentry.analyzeSummary = res;
            let response = res?.data?.response;

            if (response.summary) {
                RenderVisualizationSummary(response.summary);
            }

            RenderStatistics(response.statistics_html);

            variable.domElements.contentCardSection.show();
        }

        function onComplete() {
            common.closeLoading();
            variable.domElements.headerSummarySection.removeClass('loading');
            ResizePlotlyCharts();
        }

        function onError(msg) {
            common.closeLoading();
            variable.domElements.headerSummarySection.removeClass('loading');
            variable.domElements.btnGenerate.prop('disabled', false);
            variable.domElements.sendButton.prop('disabled', false);
            EnableReCheckin();

            common.showError('Sorry Analyze Summary failed: ' + msg);
        }

    }

    function ShowAnalysisLoading() {
        common.showLoading();
        variable.domElements.headerSummarySection.addClass('loading');
        variable.domElements.headerChartSection.addClass('loading');
        variable.domElements.btnGenerate.prop('disabled', true);
        variable.domElements.sendButton.prop('disabled', true);
    }

    function AnalyzeChartsGetSSEId() {
        return api.StartAnalyzeCharts({
            onStart: () => { },
            onSuccess: onSuccess,
            onComplete: () => { },
            onError: (msg) => onError(msg)
        });

        function onSuccess(res) {
            variable.streamToken = res?.data || null;
        }

        function onError(msg) {
            common.showError('Sorry Analyze Chart failed: ' + msg);
        }
    }

    // Server-Sent Events AnalyzeCharts
    function AnalyzeChartsSSEConnection() {
        if (variable.evtSource) {
            variable.evtSource.close();
        }

        variable.evtSource = new EventSource(
            common.modifyRelativeUrl(`AIInsights/StreamAnalyzeCharts?streamToken=${variable.streamToken}`)
        );

        // variable.evtSource.onopen = () => console.log("✅ Connected");
        variable.evtSource.onerror = HandleEventSourceError;
        variable.evtSource.onmessage = HandleEventSourceMessage;
    }

    function HandleEventSourceError() {
        if (variable.evtSource) {
            variable.evtSource.close();
            variable.evtSource = null;
            variable.domElements.headerChartSection.removeClass('loading');
            variable.domElements.btnGenerate.prop('disabled', false);
            variable.domElements.sendButton.prop('disabled', false);
            // console.log("✅ Connection closed");
        }
    }

    function HandleEventSourceMessage(e) {
        try {
            const response = JSON.parse(e.data);
            // console.log("Realtime data:", response);
            ProcessEventResponse(response);
        } catch (error) {
            console.error("Error parsing SSE message:", error);
        }
    }

    function ProcessEventResponse(response) {
        switch (response.EventName) {
            case variable.AIInsightEventName.AnalyzeCharts:
                HandleAnalyzeChartsEvent(response);
                break;
        }
    }

    function HandleAnalyzeChartsEvent(response) {
        switch (response.EventStatus) {
            case variable.AIInsightEventStatus.Started:
                // console.log("Analyze Charts started");
                $('#chart-section .card-body').remove();
                break;
            case variable.AIInsightEventStatus.Processing:
                HandleChartsProcessing(response.Data);
                break;
            case variable.AIInsightEventStatus.Completed:
                // console.log("Analyze Charts completed");
                variable.domElements.headerChartSection.removeClass('loading');
                variable.domElements.btnGenerate.prop('disabled', false);
                variable.domElements.sendButton.prop('disabled', false);
                variable.evtSource.close();
                break;
            case variable.AIInsightEventStatus.Failed:
                // common.showError('Sorry Analyze Charts failed: ');
                variable.domElements.headerChartSection.removeClass('loading');
                variable.domElements.btnGenerate.prop('disabled', false);
                variable.domElements.sendButton.prop('disabled', false);
                variable.evtSource.close();
                break;
        }
    }

    function HandleChartsProcessing(chart) {
        // console.log("Analyze Charts processing");
        const $chartSection = variable.domElements.chartSection;
        const $card = CreateChartCard(chart);
        $chartSection.append($card);
    }

    // Rendering Functions
    function RenderVisualizationSummary(visualizationSummary) {
        const $ul = variable.domElements.summarySection;
        $ul.empty();

        if (visualizationSummary) {
            visualizationSummary.forEach(summary => {
                const normalized = NormalizeLine(summary);
                const $li = CreateSummaryListItem(normalized);
                $ul.append($li);
            });
        }
    }

    function CreateSummaryListItem(normalized) {
        const { content, isNumbered, isBulleted, numberValue } = normalized;
        const className = isNumbered ? 'numbered' : isBulleted ? 'bulleted' : '';
        const numberAttr = isNumbered ? `data-number="${numberValue}"` : '';

        return `<li class="${className}" ${numberAttr}>${content}</li>`;
    }

    function CreateChartCard(chart) {
        const analysisItems = CreateAnalysisItems(chart.analysis);
        const suggestionItems = CreateSuggestionItems(chart.suggestions);

        return $(`
            <div class="card-body">
                <div class="chart-item">
                    <div class="chart-title">
                        <h3>${chart.title}</h3>
                    </div>
                    <div class="chart-image">
                        ${chart.base64_image}
                    </div>
                    <div class="chart-content">
                        <div class="chart-analysis">
                            <p><strong>Analysis:</strong></p>
                            <ul>${analysisItems}</ul>
                        </div>
                        <div class="chart-suggestion">
                            <p><strong>Suggestion:</strong></p>
                            <ul>${suggestionItems}</ul>
                        </div>
                    </div>
                </div>
                <div class="divider"></div>
            </div>
        `);
    }

    function CreateAnalysisItems(analysis) {
        return (analysis || '')
            .split('\n')
            .filter(a => a.trim() !== '')
            .map(a => CreateListItem(NormalizeLine(a)))
            .join('');
    }

    function CreateSuggestionItems(suggestions) {
        return (suggestions || [])
            .filter(s => s.trim() !== '')
            .map(s => CreateListItem(NormalizeLine(s)))
            .join('');
    }

    function CreateListItem(normalized) {
        const { content, isNumbered, isBulleted, numberValue } = normalized;
        const className = isNumbered ? 'numbered' : isBulleted ? 'bulleted' : 'bulleted';
        const numberAttr = isNumbered ? `data-number="${numberValue}"` : '';

        return `<li class="${className}" ${numberAttr}>${content}</li>`;
    }

    function NormalizeLine(line) {
        const trimmed = line.trim();

        // Check for numbered list
        const numberMatch = trimmed.match(/^(\d+)\.(?!\d)\s*(.*)$/);
        if (numberMatch) {
            return {
                content: numberMatch[2],
                isNumbered: true,
                numberValue: numberMatch[1],
                isBulleted: false
            };
        }

        // Check for bullet points
        const bulletMatch = trimmed.match(/^[-–•]\s*(.*)$/);
        if (bulletMatch) {
            return {
                content: bulletMatch[1].trim(),
                isNumbered: false,
                isBulleted: true
            };
        }

        // Default to bulleted
        return {
            content: trimmed,
            isNumbered: false,
            isBulleted: true
        };
    }

    function RenderStatistics(statisticsHtml) {
        variable.domElements.divStatistics.html(statisticsHtml);

        if (statisticsHtml) {
            AdjustStatsItemsLayout();
            ResizePlotlyCharts(variable.domElements.divStatistics);
        }
    }

    function AdjustStatsItemsLayout() {
        const $items = $('.column-statistics-section .stats-item');
        const total = $items.length;
        const fullRows = Math.floor(total / 4);
        const remaining = total % 4;

        $items.removeClass('last-1 last-2 last-3');

        if (remaining > 0) {
            const startIndex = fullRows * 4;
            const $lastItems = $items.slice(startIndex);
            $lastItems.each(function () {
                $(this).addClass(`last-${remaining}`);
            });
        }
    }

    // Chatbot Functions
    function LoadHistoryChatMessage() {
        $.ajax(common.modifyRelativeUrl('AIInsights/LoadHistoryChatMessage'), {
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            success: (response) => {
                if (response) {
                    RenderChatHistory(response);
                }
            },
            error: (errMsg) => console.log(errMsg)
        });
    }

    function RenderChatHistory(history) {
        const $chatMessages = variable.domElements.chatMessages;

        $.each(history, function (index, item) {
            const $message = CreateChatMessage(item.Sender, item.Message);
            $chatMessages.append($message);
        });

        ScrollToBottomChat();
    }

    function CreateChatMessage(sender, message) {
        const isUser = sender === 'u';
        const iconHtml = isUser
            ? '<div class="icon-placeholder"></div>'
            : '<div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>';

        const messageClass = isUser ? 'user-message' : 'bot-message';

        return $(`
            <div class="message ${messageClass}">
                ${iconHtml}
                <div class="message-text">${message}</div>
            </div>
        `);
    }

    function SendChatMessage() {
        const message = variable.domElements.chatInput.val().trim();
        if (!message) return;

        AddUserMessage(message);
        ClearChatInput();
        ShowLoadingMessage();

        $.ajax(common.modifyRelativeUrl('AIInsights/AskChatbot'), {
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            data: { question: message },
            beforeSend: () => SkipReCheckin(),
            success: (response) => HandleChatbotResponse(response),
            complete: () => EnableReCheckin(),
            error: (xhr, status, error) => HandleChatbotError()
        });
    }

    function AddUserMessage(message) {
        const $userMessage = CreateChatMessage('u', message);
        variable.domElements.chatMessages.append($userMessage);
    }

    function ClearChatInput() {
        variable.domElements.chatInput.val('');
        const textarea = variable.domElements.chatInput[0];
        textarea.style.height = 'auto';
        textarea.style.height = '36px';
    }

    function ShowLoadingMessage() {
        const $loadingDots = CreateLoadingDots();
        const $loadingMessage = CreateLoadingMessage($loadingDots);

        variable.domElements.chatMessages.append($loadingMessage);
        ScrollToBottomChat();

        return StartDotLoading($loadingDots);
    }

    function CreateLoadingDots() {
        return $('<span class="dot-loading dot-1"><i class="fas fa-circle dot"></i><i class="fas fa-circle dot"></i><i class="fas fa-circle dot"></i></span>');
    }

    function CreateLoadingMessage($loadingDots) {
        return $(`
            <div class="message bot-message loading-message">
                <div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>
                <div class="message-text"></div>
            </div>
        `).find('.message-text').append($loadingDots).end();
    }

    function HandleChatbotResponse(response) {
        variable.dataLogSentry.askChatBot = response;
        $('.loading-message').remove();

        const answer = response?.answer || 'No response from bot.';
        const $botMessage = CreateChatMessage('bot', answer);

        variable.domElements.chatMessages.append($botMessage);
        ScrollToBottomChat();

        if (response.chart_infos) {
            RenderVisualizationChart(response.chart_infos);
        }
    }

    function HandleChatbotError() {
        $('.loading-message').remove();

        const $errorMessage = CreateChatMessage('bot', 'Oops! Something went wrong.');
        variable.domElements.chatMessages.append($errorMessage);
        ScrollToBottomChat();
    }

    function AttachChatbotEvents() {
        variable.domElements.sendButton.on('click', SendChatMessage);

        variable.domElements.chatInput.on('keypress', function (e) {
            if (e.which === 13 && !e.shiftKey) {
                e.preventDefault();
                if (variable.domElements.sendButton.prop('disabled') === false) {
                    SendChatMessage();
                }
            }
        });

        variable.domElements.chatInput.on('input', AutoResizeTextarea);
        AutoResizeTextarea();
    }

    function AutoResizeTextarea() {
        const textarea = variable.domElements.chatInput[0];
        if (!textarea) return;

        textarea.style.height = 'auto';
        const maxHeight = window.innerHeight / variable.CONFIG.MAX_CHAT_HEIGHT_RATIO;

        if (textarea.scrollHeight > maxHeight) {
            textarea.style.height = maxHeight + 'px';
            textarea.style.overflowY = 'auto';
        } else {
            textarea.style.height = textarea.scrollHeight + 'px';
            textarea.style.overflowY = 'hidden';
        }
    }

    function ScrollToBottomChat() {
        const $chatMessages = variable.domElements.chatMessages;
        $chatMessages.scrollTop($chatMessages[0].scrollHeight);
    }

    function StartDotLoading($el) {
        if (!$el || !$el.length || !$el.find('.dot').length) {
            console.error('StartDotLoading: Invalid or missing $el or .dot elements', $el);
            return () => { };
        }

        let step = 1;
        const maxDots = 3;
        const $dots = $el.find('.dot');

        const interval = setInterval(() => {
            $dots.css('opacity', '0.2');
            for (let i = 0; i < step; i++) {
                $dots.eq(i).css('opacity', '1');
            }
            step = step < maxDots ? step + 1 : 1;
        }, 400);

        return () => clearInterval(interval);
    }

    function ExportExcelConvertCsv() {
        // handle file
        if (variable.fileUpload) {
            const file = variable.fileUpload;
            const formData = new FormData();
            const fileName = file.name.replace(/\.[^/.]+$/, "");

            formData.append("uploadFile", file);

            $.ajax({
                url: common.modifyRelativeUrl('AIInsights/ConvertToCsvFile'),
                type: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                xhrFields: {
                    responseType: 'blob'
                },
                success: function (blob, status, xhr) {
                    const disposition = xhr.getResponseHeader('Content-Disposition');
                    let downloadFileName = fileName + ".csv";
                    if (disposition && disposition.indexOf('filename=') !== -1) {
                        const matches = /filename="?([^"]+)"?/.exec(disposition);
                        if (matches != null && matches[1]) downloadFileName = matches[1];
                    }

                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = downloadFileName;
                    document.body.appendChild(a);
                    a.click();
                    a.remove();
                    window.URL.revokeObjectURL(url);
                },
                error: function (error) {
                    console.log('ConvertToCsvFile error:', error);
                }
            });
        }
    }

    // Utility Functions
    function SaveLog() {
        const filename = common.getUID() + '.json';
        const content = JSON.stringify(variable.dataLogSentry);
        SaveTextFile(filename, content);
    }

    function SaveTextFile(filename, content) {
        const blob = new Blob([content], { type: 'text/plain' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        link.click();
        URL.revokeObjectURL(link.href);
    }

    function UpdateFileNameFromServer() {
        $.ajax(common.modifyRelativeUrl('AIInsights/GetFileName'), {
            type: 'GET',
            async: false,
            headers: { "X-Tab-Id": common.getTabId() },
            success: (response) => {
                if (response) {
                    variable.domElements.fileNameLabel.text(response);
                }
            }
        });
    }

    function ResetDefault() {
        RenderVisualizationSummary('');
        RenderVisualizationChart('');
        RenderStatistics('');
    }

    function RenderVisualizationChart(visualizationChart) {
        const $chartSection = variable.domElements.chartSection;
        $('#chart-section .card-body').remove();

        if (visualizationChart && visualizationChart.length > 0) {
            visualizationChart.forEach(chart => {
                const $card = CreateChartCard(chart);
                $chartSection.append($card);
            });
        }
    }

    function BlockedLibraryPlotly() {
        const PLOTLY_KEYWORD = "cdn.plot.ly/plotly";
        const HAS_PLOTLY = !!window.Plotly;

        if (HAS_PLOTLY) {
            console.log("✅ Plotly already loaded, blocking future loads of any version.");
        }

        function isPlotlyScript(node) {
            if (!node || node.tagName !== "SCRIPT") return false;
            const src = node.src || "";
            return src.includes(PLOTLY_KEYWORD);
        }

        const observer = new MutationObserver((mutations) => {
            for (const mutation of mutations) {
                for (const node of mutation.addedNodes) {
                    if (isPlotlyScript(node)) {
                        console.warn("🚫 Blocked duplicate Plotly load:", node.src);
                        node.remove();
                    }
                }
            }
        });

        observer.observe(document.body, { childList: true, subtree: true });

        const origAppendChild = Element.prototype.appendChild;
        Element.prototype.appendChild = function (el) {
            if (isPlotlyScript(el)) {
                console.warn("🚫 Prevented Plotly append:", el.src);
                return el;
            }
            return origAppendChild.call(this, el);
        };

        const origCreateElement = document.createElement;
        document.createElement = function (tagName, ...args) {
            const el = origCreateElement.call(document, tagName, ...args);
            if (tagName.toLowerCase() === "script") {
                const origSetAttribute = el.setAttribute;
                el.setAttribute = function (name, value) {
                    if (name === "src" && value.includes(PLOTLY_KEYWORD)) {
                        console.warn("🚫 Prevented setting Plotly src:", value);
                        return;
                    }
                    origSetAttribute.call(el, name, value);
                };
            }
            return el;
        };
    }

    // Initialize the application
    $(document).ready(Initialize);

    $(window).on('beforeunload', function () {
        if (variable.evtSource) {
            variable.evtSource.close();
            variable.evtSource = null;
        }
    });

    $(window).on('unload', function () {
        if (variable.evtSource) {
            variable.evtSource.close();
            variable.evtSource = null;
        }
    });
}

$(AIInsights);

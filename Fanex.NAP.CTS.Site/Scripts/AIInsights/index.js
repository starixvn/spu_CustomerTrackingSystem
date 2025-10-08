function AIInsights() {
    // Constants and Configuration
    const CONFIG = {
        MAX_FILE_SIZE_MB: 50,
        SUPPORTED_FILE_EXTENSIONS: /\.(xls|xlsx)$/i,
        NO_FILE_UPLOADED: "No file uploaded",
        RECONNECT_DELAY: 3000,
        CHAT_SCROLL_DELAY: 100,
        CHART_RESIZE_DELAY: 300,
        MAX_CHAT_HEIGHT_RATIO: 3
    };

    const AIInsightEventName = {
        AnalyzeStart: 'AnalyzeStart',
        AnalyzeSummary: 'AnalyzeSummary',
        AnalyzeCharts: 'AnalyzeCharts',
        AnalyzeComplete: 'AnalyzeComplete',
        Chatbot: 'Chatbot',
        Error: 'Error',
    };

    const AIInsightEventStatus = {
        Started: 'Started',
        Processing: 'Processing',
        Completed: 'Completed',
        Failed: 'Failed',
    };

    const ViewMode = {
        VISUALIZATION: "visualization",
        STATISTICS: "statistics"
    };

    // State Management
    let dataLogSentry = {
        analyzeSummary: null,
        analyzeCharts: null,
        askChatBot: null
    };

    // DOM Elements Cache
    const domElements = {
        btnGenerate: $('#btnGenerate'),
        chatbotToggle: $('.chatbot-toggle'),
        closeChatbot: $('.close-chatbot'),
        fileInput: $('#fileInput'),
        fileNameLabel: $('#file-name-label'),
        chatbotPanel: $('#chatbotPanel'),
        container: $('.ai-insights-container'),
        divVisualization: $('#divVisualization'),
        divStatistics: $('#divStatistics'),
        chatInput: $('#chatInput'),
        chatMessages: $('#chatMessages'),
        sendButton: $('#sendButton'),
        viewOptions: $('input[name="view"]'),
        summarySection: $('#summary-section ul'),
        headerSummarySection: $('#summary-section .card-header'),
        chartSection: $('#chart-section'),
        headerChartSection: $('#chart-section .card-header'),
        contentCardSection: $('.content-card-section'),
        fileTitle: $('.file-title')
    };

    let $contentWrapper = EnsureContentWrapperExists();
    let evtSource = null;

    // Initialization
    function Initialize() {
        AttachEventListeners();
        UpdateFileNameFromServer();
        AnalyzeFile();
        LoadHistoryChatMessage();
    }

    // DOM Management
    function EnsureContentWrapperExists() {
        let $wrapper = domElements.container.find('.ai-insights-content');

        if ($wrapper.length === 0) {
            $wrapper = $('<div class="ai-insights-content"></div>');
            MoveChildrenToWrapper($wrapper);
            domElements.container.append($wrapper);
        }

        return $wrapper;
    }

    function MoveChildrenToWrapper($wrapper) {
        domElements.container.children().each(function () {
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
        const isVisualization = viewMode === ViewMode.VISUALIZATION;

        domElements.divVisualization.toggleClass('hidden', !isVisualization);
        domElements.divStatistics.toggleClass('hidden', isVisualization);

        if (!isVisualization) {
            ResizePlotlyCharts(domElements.divStatistics);
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
        domElements.viewOptions.on('change', function () {
            ToggleSections($(this).val());
        });

        AttachFileUploadEvents();
        AttachChatbotEvents();

        ToggleSections(ViewMode.VISUALIZATION);

        // Button event handlers
        domElements.chatbotToggle.on('click', ToggleChatbot);
        domElements.closeChatbot.on('click', ToggleChatbot);
        domElements.btnGenerate.on('click', AnalyzeFile);
        domElements.fileTitle.on('click', SaveLog);
    }

    // Chatbot Management
    function ToggleChatbot() {
        if (!domElements.chatbotPanel.length || !domElements.container.length) {
            return;
        }

        const isOpen = domElements.chatbotPanel.hasClass('active');

        domElements.chatbotPanel.toggleClass('active');
        $contentWrapper.toggleClass('chatbot-open', !isOpen);
        domElements.chatbotToggle.css('display', isOpen ? 'flex' : 'none');
        domElements.closeChatbot.css('display', isOpen ? 'none' : 'flex');

        if (!isOpen) {
            setTimeout(ScrollToBottomChat, CONFIG.CHAT_SCROLL_DELAY);
        }

        setTimeout(() => {
            ResizePlotlyCharts(domElements.divVisualization);
        }, CONFIG.CHART_RESIZE_DELAY);
    }

    // File Upload Management
    function AttachFileUploadEvents() {
        if (!domElements.fileInput.length || !domElements.fileNameLabel.length) {
            return;
        }

        const initialFileName = domElements.fileNameLabel.text() || CONFIG.NO_FILE_UPLOADED;
        domElements.fileNameLabel.text(initialFileName);
        UpdateTitle();

        domElements.fileInput.on('change', HandleFileSelection);
    }

    function HandleFileSelection(e) {
        const file = e.target.files[0];

        ResetDefault();
        domElements.contentCardSection.hide();

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
        if (!CONFIG.SUPPORTED_FILE_EXTENSIONS.test(file.name)) {
            common.showError('Please upload a .xls or .xlsx file!');
            domElements.fileInput.val('');
            return false;
        }

        const maxSize = CONFIG.MAX_FILE_SIZE_MB * 1024 * 1024;
        if (file.size > maxSize) {
            common.showError(`Exceeds the allowed limit of ${CONFIG.MAX_FILE_SIZE_MB} MB`);
            domElements.fileInput.val('');
            return false;
        }

        return true;
    }

    function SetNoFileState() {
        domElements.fileNameLabel.text(CONFIG.NO_FILE_UPLOADED);
        UpdateTitle();
    }

    function UpdateTitle() {
        const currentText = domElements.fileNameLabel.text();
        domElements.fileNameLabel.attr('title', currentText || CONFIG.NO_FILE_UPLOADED);
    }

    function UploadFile(file) {
        const formData = new FormData();
        const fileName = file.name;

        formData.append("uploadFile", file);
        domElements.fileNameLabel.text(fileName);

        $.ajax(common.modifyRelativeUrl('AIInsights/UploadFile'), {
            type: 'POST',
            headers: { "X-Tab-Id": common.getTabId() },
            data: formData,
            processData: false,
            contentType: false,
            beforeSend: HandleUploadStart,
            success: (response) => HandleUploadSuccess(response, fileName),
            complete: HandleUploadComplete,
            error: HandleUploadError
        });
    }

    function HandleUploadStart() {
        common.showLoading();
        domElements.btnGenerate.prop('disabled', true);
    }

    function HandleUploadSuccess(response, fileName) {
        if (response.success === false) {
            domElements.fileNameLabel.text(CONFIG.NO_FILE_UPLOADED);
            common.showError("Upload failed " + response.message);
        } else {
            domElements.btnGenerate.prop('disabled', false);
            domElements.fileNameLabel.text(fileName);
            common.showSuccess('File uploaded successfully: ' + fileName, 2000);
        }
        UpdateTitle();
    }

    function HandleUploadComplete() {
        common.closeLoading();
        CleanUploadedFileInput();
    }

    function HandleUploadError() {
        common.closeLoading();
    }

    function CleanUploadedFileInput() {
        if (domElements.fileInput && domElements.fileInput[0]) {
            domElements.fileInput[0].value = "";
        }
        if (domElements.fileInput) {
            domElements.fileInput.val("");
        }
    }

    // Analysis Functions
    function AnalyzeFile() {
        const fileName = domElements.fileNameLabel.text().trim();

        if (fileName === CONFIG.NO_FILE_UPLOADED || fileName === '') {
            return;
        }

        ResetDefault();
        ShowAnalysisLoading();
        SkipReCheckin();

        AnalyzeSummary()
            .always(function () {
                AnalyzeChartsSSEConnection();
                EnableReCheckin();
            });
    }

    function AnalyzeSummary() {
        return $.ajax(common.modifyRelativeUrl('AIInsights/AnalyzeSummary'), {
            type: 'POST',
            headers: {
                "X-Tab-Id": common.getTabId()
            },
            success: function (data) {
                dataLogSentry.analyzeSummary = data;

                if (data && data.status_code != -1) {
                    if (data.response.summary) {
                        RenderVisualizationSummary(data.response.summary);
                    }

                    RenderStatistics(data.response.statistics_html);

                    domElements.contentCardSection.show();
                } else {
                    common.showError('Sorry Analyze Summary failed: ' + data.error);
                }
            },
            complete: function () {
                common.closeLoading();
                domElements.headerSummarySection.removeClass('loading');
                ResizePlotlyCharts();
            },
            error: function (errMsg) {
                common.closeLoading();
                domElements.headerSummarySection.removeClass('loading');
                domElements.btnGenerate.prop('disabled', false);
                EnableReCheckin();
            }
        });
    }

    function ShowAnalysisLoading() {
        common.showLoading();
        domElements.headerSummarySection.addClass('loading');
        domElements.headerChartSection.addClass('loading');
        domElements.btnGenerate.prop('disabled', true);
    }

    function HandleAnalysisComplete() {
        common.closeLoading();
        ResizePlotlyCharts(domElements.divVisualization);
    }

    function HandleAnalysisError() {
        common.closeLoading();
    }

    // Server-Sent Events AnalyzeCharts
    function AnalyzeChartsSSEConnection() {
        if (evtSource) {
            evtSource.close();
        }

        evtSource = new EventSource(
            common.modifyRelativeUrl(`AIInsights/StreamAnalyzeCharts?tabId=${common.getTabId()}&t=${Date.now()}`)
        );

        evtSource.onopen = () => console.log("✅ Connected");
        evtSource.onerror = HandleEventSourceError;
        evtSource.onmessage = HandleEventSourceMessage;
    }

    function HandleEventSourceError() {
        if (evtSource) {
            evtSource.close();
            evtSource = null;
            console.log("✅ Connection closed");
        }
    }

    function HandleEventSourceMessage(e) {
        try {
            const response = JSON.parse(e.data);
            console.log("Realtime data:", response);
            ProcessEventResponse(response);
        } catch (error) {
            console.error("Error parsing SSE message:", error);
        }
    }

    function ProcessEventResponse(response) {
        switch (response.EventName) {
            case AIInsightEventName.AnalyzeCharts:
                HandleAnalyzeChartsEvent(response);
                break;
        }
    }

    function HandleAnalyzeChartsEvent(response) {
        switch (response.EventStatus) {
            case AIInsightEventStatus.Started:
                console.log("Analyze Charts started");
                $('#chart-section .card-body').remove();
                break;
            case AIInsightEventStatus.Processing:
                HandleChartsProcessing(response.Data);
                break;
            case AIInsightEventStatus.Completed:
                console.log("Analyze Charts completed");
                domElements.headerChartSection.removeClass('loading');
                domElements.btnGenerate.prop('disabled', false);
                evtSource.close();
                break;
            case AIInsightEventStatus.Failed:
                common.showError('Sorry Analyze Charts failed: ');
                evtSource.close();
                break;
        }
    }

    function HandleChartsProcessing(chart) {
        console.log("Analyze Charts processing");
        const $chartSection = domElements.chartSection;
        const $card = CreateChartCard(chart);
        $chartSection.append($card);
    }

    // Rendering Functions
    function RenderVisualizationSummary(visualizationSummary) {
        const $ul = domElements.summarySection;
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
        domElements.divStatistics.html(statisticsHtml);

        if (statisticsHtml) {
            AdjustStatsItemsLayout();
            ResizePlotlyCharts(domElements.divStatistics);
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
        const $chatMessages = domElements.chatMessages;

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
        const message = domElements.chatInput.val().trim();
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
        domElements.chatMessages.append($userMessage);
    }

    function ClearChatInput() {
        domElements.chatInput.val('');
        const textarea = domElements.chatInput[0];
        textarea.style.height = 'auto';
        textarea.style.height = '36px';
    }

    function ShowLoadingMessage() {
        const $loadingDots = CreateLoadingDots();
        const $loadingMessage = CreateLoadingMessage($loadingDots);

        domElements.chatMessages.append($loadingMessage);
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
        dataLogSentry.askChatBot = response;
        $('.loading-message').remove();

        const answer = response?.answer || 'No response from bot.';
        const $botMessage = CreateChatMessage('bot', answer);

        domElements.chatMessages.append($botMessage);
        ScrollToBottomChat();

        if (response.chart_infos) {
            RenderVisualizationChart(response.chart_infos);
        }
    }

    function HandleChatbotError() {
        $('.loading-message').remove();

        const $errorMessage = CreateChatMessage('bot', 'Oops! Something went wrong.');
        domElements.chatMessages.append($errorMessage);
        ScrollToBottomChat();
    }

    function AttachChatbotEvents() {
        domElements.sendButton.on('click', SendChatMessage);

        domElements.chatInput.on('keypress', function (e) {
            if (e.which === 13 && !e.shiftKey) {
                e.preventDefault();
                SendChatMessage();
            }
        });

        domElements.chatInput.on('input', AutoResizeTextarea);
        AutoResizeTextarea();
    }

    function AutoResizeTextarea() {
        const textarea = domElements.chatInput[0];
        if (!textarea) return;

        textarea.style.height = 'auto';
        const maxHeight = window.innerHeight / CONFIG.MAX_CHAT_HEIGHT_RATIO;

        if (textarea.scrollHeight > maxHeight) {
            textarea.style.height = maxHeight + 'px';
            textarea.style.overflowY = 'auto';
        } else {
            textarea.style.height = textarea.scrollHeight + 'px';
            textarea.style.overflowY = 'hidden';
        }
    }

    function ScrollToBottomChat() {
        const $chatMessages = domElements.chatMessages;
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

    // Utility Functions
    function SaveLog() {
        const filename = common.getUID() + '.json';
        const content = JSON.stringify(dataLogSentry);
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
                    domElements.fileNameLabel.text(response);
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
        const $chartSection = domElements.chartSection;
        $('#chart-section .card-body').remove();

        if (visualizationChart && visualizationChart.length > 0) {
            visualizationChart.forEach(chart => {
                const $card = CreateChartCard(chart);
                $chartSection.append($card);
            });
        }
    }

    // Initialize the application
    $(document).ready(Initialize);

    $(window).on('beforeunload', function () {
        if (evtSource) {
            evtSource.close();
            evtSource = null;
        }
    });

    $(window).on('unload', function () {
        if (evtSource) {
            evtSource.close();
            evtSource = null;
        }
    });
}

$(AIInsights);

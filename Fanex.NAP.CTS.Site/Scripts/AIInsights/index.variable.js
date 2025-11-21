function AIInsightsVariable() {
    this.AIHttpStatusCode =
    {
        Success: 200,
        HttpNetworkClose: 440,
    }

    // Constants and Configuration
    this.CONFIG = (() => {
        const maxFileSizeBytes = parseInt($('#divAIMaxFileSizeBytes').text()) || 50 * 1024 * 1024; // Default to 50MB
        const maxFileSizeMB = parseInt($('#divAIUploadMaxFileSizeMB').text()) || 50; // Default to 50MB
        const allowedFileExtensions = $('#divAIAllowedExtensions').text() || '.xls,.xlsx,.csv';
        const timeoutGlobal = parseInt($('#divAITimeOutGlobal').text()) || 10;
        const exts = allowedFileExtensions.replace(/\./g, '').replace(/,/g, '|');
        const supportedFileExtensions = new RegExp(`\\.(${exts})$`, 'i');

        return {
            MAX_FILE_SIZE_BYTES: maxFileSizeBytes,
            MAX_FILE_SIZE_MB: maxFileSizeMB,
            SUPPORTED_FILE_EXTENSIONS: supportedFileExtensions,
            NO_FILE_UPLOADED: "No file uploaded",
            CHAT_SCROLL_DELAY: 100,
            CHART_RESIZE_DELAY: 300,
            MAX_CHAT_HEIGHT_RATIO: 3,
            RETRY_TIME: 2,
            RETRY_DELAY_IN_SECONDS: 5,
            TIME_OUT_GLOBAL_SECONDS: timeoutGlobal,
        };
    })();

    this.AIInsightEventName = {
        AnalyzeStart: 'AnalyzeStart',
        AnalyzeSummary: 'AnalyzeSummary',
        AnalyzeCharts: 'AnalyzeCharts',
        AnalyzeComplete: 'AnalyzeComplete',
        Chatbot: 'Chatbot',
        Error: 'Error',
    };

    this.AIInsightEventStatus = {
        Started: 'Started',
        Processing: 'Processing',
        Completed: 'Completed',
        Failed: 'Failed',
    };

    this.ViewMode = {
        VISUALIZATION: "visualization",
        STATISTICS: "statistics"
    };

    // State Management
    this.dataLogSentry = {
        analyzeSummary: null,
        analyzeCharts: null,
        askChatBot: null
    };

    this.fileUpload = null;

    // DOM Elements Cache
    this.domElements = {
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

    this.$contentWrapper = null;
    this.streamToken = null;
    this.evtSource = null;
    this.globalTimeoutReached = false;
}

var variable = new AIInsightsVariable();
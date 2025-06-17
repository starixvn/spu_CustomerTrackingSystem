function AIInsights() {
    let domElements = {    
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
        chatMessages: $('.chat-messages'),
        sendButton: $('#sendButton'),
        viewOptions: $('input[name="view"]'),
        summarySection: $('#summary-section ul'),
        chartSection: $('#chart-section')
    }    

    const ViewMode = {
        VISUALIZATION: "visualization",
        STATISTICS: "statistics"
    };

    let $contentWrapper = EnsureContentWrapperExists();

    function EnsureContentWrapperExists() {
        let $wrapper = domElements.container.find('.ai-insights-content');

        if ($wrapper.length === 0) {
            $wrapper = $('<div class="ai-insights-content"></div>');

            domElements.container.children().each(function () {
                const $child = $(this);
                const isChatbotToggle = $child.hasClass('chatbot-toggle');
                const isChatbotPanel = $child.attr('id') === 'chatbotPanel';

                if (!isChatbotToggle && !isChatbotPanel) {
                    $wrapper.append($child);
                }
            });

            domElements.container.append($wrapper);
        }

        return $wrapper;
    }

    function ToggleSections(viewMode) {        
        switch (viewMode) {
            case ViewMode.VISUALIZATION:               
                domElements.divVisualization.removeClass('hidden');
                domElements.divStatistics.addClass('hidden');

                break;
            case ViewMode.STATISTICS:             
                domElements.divVisualization.addClass('hidden');
                domElements.divStatistics.removeClass('hidden');
                break;
            default:
                domElements.divVisualization.removeClass('hidden');
                domElements.divStatistics.addClass('hidden');
        }
    }
  
    function AttachEventListeners() {
        domElements.viewOptions.on('change', function () {
            ToggleSections($(this).val());
        });
    }

    function ToggleChatbot() {
        if (domElements.chatbotPanel.length && domElements.container.length) {
            const isOpen = domElements.chatbotPanel.hasClass('active');

            domElements.chatbotPanel.toggleClass('active');
            $contentWrapper.toggleClass('chatbot-open', !isOpen);
            domElements.chatbotToggle.css('display', isOpen ? 'flex' : 'none');
            domElements.closeChatbot.css('display', isOpen ? 'none' : 'flex');
        }
    }
  
    function UpdateTitle() {
        let currentText = domElements.fileNameLabel.text();
        domElements.fileNameLabel.attr('title', currentText || 'No file uploaded');
    }

    if (domElements.fileInput.length && domElements.fileNameLabel.length) {
        let initialFileName = domElements.fileNameLabel.text() || 'No file uploaded';

        domElements.fileNameLabel.text(initialFileName);

        UpdateTitle();

        domElements.fileInput.on('change', function (e) {
            const file = e.target.files[0];

            if (file && !file.name.toLowerCase().endsWith('.xls')) {                
                common.showError('Please upload a .xls file!');

                $(this).val('');

                return;
            }

            if (file) {
                const formData = new FormData();
                domElements.fileNameLabel.text(file.name);

                UpdateTitle();

                formData.append("uploadFile", file);

                fetch("/AIInsights/UploadFile", {
                    method: "POST",
                    body: formData
                })
                    .then(response => {
                        if (!response.ok) throw new Error("Upload failed");
                        return response.text();
                    })
                    .then(fileName => {
                        console.log("Upload success, file name:", fileName);
                        domElements.fileNameLabel.text(fileName);

                        UpdateTitle();
                    })
                    .catch(error => {
                        console.error("Upload error:", error);
                    });
            } else {
                domElements.fileNameLabel.text("No file uploaded");
                UpdateTitle();
            }
        });
    }

    function AnalyzeFile() { 
        $.ajax(common.modifyRelativeUrl('AIInsights/AnalyzeFile'), {
            type: 'POST',       
            beforeSend: function () {       
                common.showLoading();                
                domElements.btnGenerate.prop('disabled', true);
            },
            success: function (response) {              
                domElements.btnGenerate.prop('disabled', false);

                if (response) {
                    if (response.visualization_response) {
                        RenderVisualizationSummary(response.visualization_response.summary_list);
                        RenderVisualizationChart(response.visualization_response.chart_infos);
                    }
                    
                    RenderStatistics(response.statistics_html);
                }
            },
            complete: function () {
                common.closeLoading();
            },
            error: function (errMsg) {  
                common.closeLoading();
                domElements.btnGenerate.prop('disabled', false);
                console.log(errMsg);
            }
        });
    }

    function RenderVisualizationSummary(visualizationSummary) {
        if (visualizationSummary) {            
            var $ul = domElements.summarySection;
            $ul.empty();

            visualizationSummary.forEach(function (summary) {
                $ul.append(`<li>${summary}</li>`);
            });
        }
    }

    function RenderVisualizationChart(visualizationChart) {
        if (visualizationChart.length > 0) {
            var $chartSection = domElements.chartSection;

            $('#chart-section .card-body').remove();

            visualizationChart.forEach(function (chart) {
                var $card = $(`
                    <div class="card-body">
                        <div class="chart-item">
                            <div class="chart-title">
                                <h3>${chart.title}</h3>
                            </div>
                        </div>
                        <div class="chart-image">
                            <img src="data:image/png;base64,${chart.base64_image}" alt="${chart.name}" />
                        </div>
                        <div class="chart-content">
                            <div class="chart-analysis">
                                <p><strong>Analysis:</strong></p>
                                <ul>
                                    <li>${chart.analysis}</li>
                                </ul>
                            </div>
                            <div class="chart-suggestion">
                                <p><strong>Suggestion:</strong></p>
                                <ul>
                                    ${chart.suggestions.map(s => `<li>${s}</li>`).join('')}
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div class="divider"></div>
                `);

                $chartSection.append($card);
            });
        }
    }

    function RenderVisualization(visualizationData) {
        if (visualizationData) {
            var summaries = visualizationData.summary_list;

            var $ul = domElements.summarySection;
            $ul.empty();

            summaries.forEach(function (summary) {
                $ul.append(`<li>${summary}</li>`);
            });

            var $chartSection = domElements.chartSection;

            visualizationData.chart_infos.forEach(function (chart) {
                var $card = $(`
                    <div class="card-body">
                        <div class="chart-item">
                            <div class="chart-title">
                                <h3>${chart.title}</h3>
                            </div>
                        </div>
                        <div class="chart-image">
                            <img src="data:image/png;base64,${chart.base64_image}" alt="${chart.name}" style="display: block; margin: auto; max-width: 80%; height: auto;" />
                        </div>
                        <div class="chart-content">
                            <div class="chart-analysis">
                                <p><strong>Analysis:</strong></p>
                                <ul>
                                    <li>${chart.analysis}</li>
                                </ul>
                            </div>
                            <div class="chart-suggestion">
                                <p><strong>Suggestion:</strong></p>
                                <ul>
                                    ${chart.suggestions.map(s => `<li>${s}</li>`).join('')}
                                </ul>
                            </div>
                        </div>
                        <div class="divider"></div>
                    </div>
                `);

                $chartSection.append($card);
            });
        }      
    }

    function RenderStatistics(statisticsHtml) {
        if (statisticsHtml) {
            domElements.divStatistics.html(statisticsHtml);
        }      
    }

    function LoadHistoryChatMessage() {
        $.ajax(common.modifyRelativeUrl('AIInsights/LoadHistoryChatMessage'), {
            type: 'POST',
            beforeSend: function () { },
            success: function (response) {                
                if (response) {
                    let $chatMessages = domElements.chatMessages;
                    $.each(response, function (index, item) {                       
                        let $message = item.Sender == 'u' ?
                            $(`
                                <div class="message user-message">
                                    <div class="icon-placeholder"></div>
                                    <div class="message-text">${item.Message}</div>
                                </div>
                            `) : 
                            $(`
                                <div class="message bot-message">
                                    <div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>
                                    <div class="message-text">${item.Message}</div>
                                </div>
                            `);

                        $chatMessages.append($message);
         
                    });                                     
                }
            },
            complete: function () { },
            error: function (errMsg) {               
                console.log(errMsg);
            }
        });
    }

    function SendChatMessage() {
        let $chatMessages = domElements.chatMessages;

        let message = domElements.chatInput.val().trim();        

        if (!message) return;

        let $userMessage = $(`
            <div class="message user-message">
                <div class="icon-placeholder"></div>
                <div class="message-text">${message}</div>
            </div>
        `);

        $chatMessages.append($userMessage);

        domElements.chatInput.val('');

        let requestParam = {          
            question: message
        };

        $.ajax(common.modifyRelativeUrl('AIInsights/AskChatbot'), {
            type: 'POST',
            contentType: 'application/json',
            dataType: 'json',
            data: JSON.stringify(requestParam),
            beforeSend: function () {
            },
            success: function (response) {
                const answer = response?.answer || 'No response from bot.';

                const $botMessage = $(`
                    <div class="message bot-message">
                        <div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>
                        <div class="message-text">${answer}</div>
                    </div>
                `);

                $chatMessages.append($botMessage);

                $chatMessages.scrollTop($chatMessages[0].scrollHeight);

                RenderVisualizationChart(response.chart_infos);
            },
            complete: function () {
            },
            error: function (xhr, status, error) {
                console.error('Chatbot error:', error);

                const $errorMessage = $(`
                    <div class="message bot-message">
                        <div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>
                        <div class="message-text">Oops! Something went wrong.</div>
                    </div>
                `);
                $chatMessages.append($errorMessage);
            }
        });
    }

    function AttachChatbotEvents() {
        domElements.sendButton.on('click', SendChatMessage);

        domElements.chatInput.on('keypress', function (e) {
            if (e.which === 13) {
                e.preventDefault();

                SendChatMessage();
            }
        });
    }

    function Initial() {
        AttachEventListeners();

        AttachChatbotEvents();

        ToggleSections(ViewMode.VISUALIZATION);

        domElements.chatbotToggle.on('click', function () { ToggleChatbot(); });

        domElements.closeChatbot.on('click', function () { ToggleChatbot(); });      

        AnalyzeFile();

        domElements.btnGenerate.on('click', function () { AnalyzeFile(); });

        LoadHistoryChatMessage();
    }

    $(document).ready(function () {
        Initial();
    })
}

$(AIInsights);

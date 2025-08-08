function AIInsights() {
    let dataLogSentry = {
        analyzeData: null,
        askChatBot: null
    }

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
        chatMessages: $('#chatMessages'),
        sendButton: $('#sendButton'),
        viewOptions: $('input[name="view"]'),
        summarySection: $('#summary-section ul'),
        chartSection: $('#chart-section'),
        contentCardSection: $('.content-card-section'),
        fileTitle: $('.file-title')
    }    

    const ViewMode = {
        VISUALIZATION: "visualization",
        STATISTICS: "statistics"
    };

    const NoFileUpload = "No file uploaded";

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
                $('#divStatistics .js-plotly-plot').each(function () {
                    Plotly.Plots.resize(this);
                });
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

            if (!isOpen) {
                setTimeout(ScrollToBottomChat, 100);
            }

            setTimeout(function () {
                $('#divVisualization .js-plotly-plot').each(function () {
                    Plotly.Plots.resize(this);
                });
            }, 300);
            //setTimeout(function () {
            //    $('#divVisualization .js-plotly-plot').each(function () {
            //        const plotEl = this;
            //        let count = 0;

            //        function resizeLoop() {
            //            Plotly.Plots.resize(plotEl);
            //            count++;
            //            if (count < 5) requestAnimationFrame(resizeLoop);
            //        }

            //        resizeLoop();
            //    });
            //}, 300);

        }
    }
  
    function UpdateTitle() {
        let currentText = domElements.fileNameLabel.text();
        domElements.fileNameLabel.attr('title', currentText || NoFileUpload);
    }

    if (domElements.fileInput.length && domElements.fileNameLabel.length) {
        let initialFileName = domElements.fileNameLabel.text() || NoFileUpload;

        domElements.fileNameLabel.text(initialFileName);

        UpdateTitle();

        domElements.fileInput.on('change', function (e) {
            const file = e.target.files[0];

            RenderVisualizationSummary('');
            RenderVisualizationChart('');
            RenderStatistics('');
            domElements.contentCardSection.hide();

            if (file && !/\.(xls|xlsx)$/i.test(file.name)) {
                common.showError('Please upload a .xls or .xlsx file!');
                $(this).val('');
                return;
            }

            if (file) {
                const maxMBSize = 50;
                const maxSize = maxMBSize * 1024 * 1024; // 50MB in bytes
                
                if (file.size > maxSize) {
                    common.showError('Exceeds the allowed limit of ' + maxMBSize + " MB");
                    fileInput.val('');
                    return;
                }

                const formData = new FormData();
                let fileName = file.name;
                domElements.fileNameLabel.text(fileName);
               
                formData.append("uploadFile", file);    

                $.ajax(common.modifyRelativeUrl('AIInsights/UploadFile'), {
                    type: 'POST',
                    data: formData,
                    processData: false, 
                    contentType: false, 
                    beforeSend: function () {
                        common.showLoading();
                        domElements.btnGenerate.prop('disabled', true);
                    },
                    success: function (response) {
                        if (response.success == false) {
                            domElements.fileNameLabel.text(NoFileUpload);

                            common.showError("Upload failed " + response.message);

                        } else {
                            domElements.btnGenerate.prop('disabled', false);
                            domElements.fileNameLabel.text(fileName);
                            common.showSuccess('File uploaded successfully: ' + fileName, 2000);
                        }
                        
                        UpdateTitle();
                    },
                    complete: function () {
                        common.closeLoading();
                    },
                    error: function (errMsg) {
                        common.closeLoading();
                        domElements.btnGenerate.prop('disabled', false);
                    }
                });

            } else {
                if (domElements.fileNameLabel.text().trim() != NoFileUpload) {
                    domElements.fileNameLabel.text(NoFileUpload);
                    UpdateTitle();
                }               
            }
        });
    }

    function AnalyzeFile() { 

        let fileName = domElements.fileNameLabel.text().trim();

        if (fileName == NoFileUpload || fileName == '') {
            return;
        }

        $.ajax(common.modifyRelativeUrl('AIInsights/AnalyzeFile'), {
            type: 'POST',       
            beforeSend: function () {       
                SkipReCheckin();
                common.showLoading();                
                domElements.btnGenerate.prop('disabled', true);
            },
            success: function (response) {
                dataLogSentry.analyzeData = response;

                domElements.btnGenerate.prop('disabled', false);

                if (response && response.ErrorCode != -1) {
                    if (response.visualization_response) {
                        RenderVisualizationSummary(response.visualization_response.summary_list);
                        RenderVisualizationChart(response.visualization_response.chart_infos);
                    }
                    
                    RenderStatistics(response.statistics_html);                    

                    domElements.contentCardSection.show();
                } else {
                    common.showError('Sorry Analyze File failed: ' + response.ErrorMessage);
                }
            },
            complete: function () {
                common.closeLoading();
                EnableReCheckin();
            },
            error: function (errMsg) {  
                common.closeLoading();
                domElements.btnGenerate.prop('disabled', false);
                EnableReCheckin();
            }
        });
    }

    function RenderVisualizationSummary(visualizationSummary) {
        const $ul = domElements.summarySection;
        $ul.empty();

        if (visualizationSummary) {
            visualizationSummary.forEach(function (summary) {
                const { content, isNumbered, isBulleted, numberValue } = NormalizeLine(summary);
                const className = isNumbered ? 'numbered' : isBulleted ? 'bulleted' : '';
                const numberAttr = isNumbered ? `data-number="${numberValue}"` : '';
                $ul.append(`<li class="${className}" ${numberAttr}>${content}</li>`);
            });
        }
    }

    function RenderVisualizationChart(visualizationChart) {
        var $chartSection = domElements.chartSection;
        $('#chart-section .card-body').remove();

        if (visualizationChart.length > 0) {
            visualizationChart.forEach(function (chart) {
                var analysisItems = (chart.analysis || '')
                    .split('\n')
                    .filter(a => a.trim() !== '')
                    .map(a => {
                        const { content, isNumbered, isBulleted, numberValue } = NormalizeLine(a);
                        const className = isNumbered ? 'numbered' : isBulleted ? 'bulleted' : '';
                        const numberAttr = isNumbered ? `data-number="${numberValue}"` : '';

                        return `<li class="${className}" ${numberAttr}>${content}</li>`;
                    })
                    .join('');

                var suggestionItems = (chart.suggestions || [])
                    .filter(s => s.trim() !== '')
                    .map(s => {
                        const { content, isNumbered, isBulleted, numberValue } = NormalizeLine(s);
                        let className = isNumbered ? 'numbered' : isBulleted ? 'bulleted' : 'bulleted'; 
                        const numberAttr = isNumbered ? `data-number="${numberValue}"` : '';

                        return `<li class="${className}" ${numberAttr}>${content}</li>`;
                    }).join('');

                var $card = $(`
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

                $chartSection.append($card);
            });
        }
    }

    function NormalizeLine(line) {
        const trimmed = line.trim();
        const numberMatch = trimmed.match(/^(\d+)\.\s*(.*)$/);

        if (numberMatch) {
            return {
                content: numberMatch[2],
                isNumbered: true,
                numberValue: numberMatch[1],
                isBulleted: false
            };
        }

        const bulletMatch = trimmed.match(/^[-–•]\s*(.*)$/);
        if (bulletMatch) {
            return {
                content: bulletMatch[1].trim(),
                isNumbered: false,
                isBulleted: true
            };
        }

        const weirdBulletMatch = trimmed.match(/^[-–•]\.?\s*(.*)$/);
        if (weirdBulletMatch) {
            return {
                content: weirdBulletMatch[1].trim(),
                isNumbered: false,
                isBulleted: true
            };
        }

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

            $('#divStatistics .js-plotly-plot').each(function () {
                Plotly.Plots.resize(this);
            });
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

                    ScrollToBottomChat();
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

        let $userMessage = $('<div class="message user-message">');
        $userMessage.append('<div class="icon-placeholder"></div>');
        $userMessage.append($('<div class="message-text">').text(message));
        $chatMessages.append($userMessage);

        domElements.chatInput.val('');
        let textarea = domElements.chatInput[0];
        textarea.style.height = 'auto';
        textarea.style.height = '36px';

        const $loadingDots = $('<span class="dot-loading dot-1"><i class="fas fa-circle dot"></i><i class="fas fa-circle dot"></i><i class="fas fa-circle dot"></i></span>');

        const $loadingMessage = $(`
        <div class="message bot-message loading-message">
            <div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>
            <div class="message-text"></div>
        </div>
        `);

        $loadingMessage.find('.message-text').append($loadingDots);
        $chatMessages.append($loadingMessage);
        ScrollToBottomChat();

        const stopLoading = StartDotLoading($loadingDots);       

        $.ajax(common.modifyRelativeUrl('AIInsights/AskChatbot'), {
            type: 'POST',           
            data: {
                question: message
            },
            beforeSend: function () {       
                SkipReCheckin();
            },
            success: function (response) {
                dataLogSentry.askChatBot = response;
                stopLoading();
                $loadingMessage.remove(); 

                const answer = response?.answer || 'No response from bot.';

                const $botMessage = $(`
                    <div class="message bot-message">
                        <div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>
                        <div class="message-text">${answer}</div>
                    </div>
                `);

                $chatMessages.append($botMessage);
                ScrollToBottomChat();

                RenderVisualizationChart(response.chart_infos);
            },
            complete: function () {
                EnableReCheckin();
            },
            error: function (xhr, status, error) {
                stopLoading();
                $loadingMessage.remove(); 

                const $errorMessage = $(`
                    <div class="message bot-message">
                        <div class="icon-wrapper bot-icon"><i class="fas fa-robot"></i></div>
                        <div class="message-text">Oops! Something went wrong.</div>
                    </div>
                `);
                $chatMessages.append($errorMessage);
                ScrollToBottomChat();
                EnableReCheckin();
            }
        });
    }

    function AttachChatbotEvents() {
        domElements.sendButton.on('click', SendChatMessage);

        domElements.chatInput.on('keypress', function (e) {
            if (e.which === 13 && !e.shiftKey) {
                e.preventDefault();
                SendChatMessage();
            }
        });

        function autoResizeTextarea() {
            let textarea = domElements.chatInput[0];
            if (!textarea) return;

            textarea.style.height = 'auto'; 
            const maxHeight = window.innerHeight / 3;

            if (textarea.scrollHeight > maxHeight) {
                textarea.style.height = maxHeight + 'px';
                textarea.style.overflowY = 'auto';
            } else {
                textarea.style.height = textarea.scrollHeight + 'px';
                textarea.style.overflowY = 'hidden';
            }
        }

        domElements.chatInput.on('input', autoResizeTextarea);
        autoResizeTextarea();
    }

    function ScrollToBottomChat() {
        let $chatMessages = domElements.chatMessages;
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

    function SaveLog() {
        saveTextFile(generateUID() + '.json', JSON.stringify(dataLogSentry))
    }

    function saveTextFile(filename, content) {
        const blob = new Blob([content], { type: 'text/plain' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = filename;
        link.click();
        URL.revokeObjectURL(link.href);
    }

    function generateUID() {
        return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11)
            .replace(/[018]/g, c =>
                (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
            );
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

        domElements.fileTitle.on('click', function () { SaveLog(); });
    }

    $(document).ready(function () {
        Initial();
    })
}

$(AIInsights);

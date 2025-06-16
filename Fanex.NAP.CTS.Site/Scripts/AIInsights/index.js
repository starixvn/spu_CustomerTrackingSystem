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
        divStatistics: $('#divStatistics')
    }

    let $sections = {
        summary: $('#summary-section'),
        chart: $('#chart-section'),
        overview: $('#overview-section'),
        columnStats: $('#column-statistics-section')
    };
  
    const $viewOptions = $('input[name="view"]');
    let $contentWrapper = domElements.container.find('.ai-insights-content');

    if ($contentWrapper.length === 0) {
        $contentWrapper = $('<div class="ai-insights-content"></div>');
        domElements.container.children().each(function () {
            const $child = $(this);
            if (!$child.hasClass('chatbot-toggle') && $child.attr('id') !== 'chatbotPanel') {
                $contentWrapper.append($child);
            }
        });
        domElements.container.append($contentWrapper);
    }    

    function ToggleSections(view) {
        switch (view) {
            case 'visualization':
                //ShowSections(['summary', 'chart']);
                //HideSections(['overview', 'columnStats']);
                domElements.divVisualization.show();
                domElements.divStatistics.hide();

                break;
            case 'statistics':
                //ShowSections(['overview', 'columnStats']);
                //HideSections(['summary', 'chart']);
                domElements.divVisualization.show();
                domElements.divStatistics.hide();
                break;
            default:               
                //ShowSections(['summary', 'chart']);
                //HideSections(['overview', 'columnStats']);
                domElements.divVisualization.show();
                domElements.divStatistics.hide();
        }
    }

    function ShowSections(sectionIds) {
        $.each($sections, (key, $section) => {
            $section.hide();
        });

        sectionIds.forEach(id => {
            if ($sections[id]) $sections[id].show();
        });
    }

    function HideSections(sectionIds) {
        sectionIds.forEach(id => {
            if ($sections[id]) $sections[id].hide();
        });
    }

    function AttachEventListeners() {
        $viewOptions.on('change', function () {
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
          /*  data: params,*/
            beforeSend: function () {                
            },
            success: function (response) {
                if (response) {
                    RenderVisualization(response.visualization_response);
                    RenderStatistics(response.statistics_html);
                }
            },
            complete: function () {                
            },
            error: function (errMsg) {              
                console.log(errMsg);
            }
        });
    }

    function RenderVisualization(visualizationData) {
        if (visualizationData) {
            var summaries = visualizationData.summary_list;

            var $ul = $('#summary-section ul');
            $ul.empty();

            summaries.forEach(function (summary) {
                $ul.append(`<li>${summary}</li>`);
            });

            var $chartSection = $('#chart-section');

            visualizationData.chart_infos.forEach(function (chart) {
                var $card = $(`
                    <div class="card-body">
                        <div class="chart-item">
                            <div class="chart-title">
                                <h3>${chart.title}</h3>
                            </div>
                        </div>
                        <div class="chart-image">
                            <img src="data:image/png;base64,${ chart.base64_image }" alt="${chart.name}" />
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
                `);

                $chartSection.append($card);
            });

        } else {
            domElements.divVisualization.html('<p>No visualization data available.</p>');
        }
    }

    function RenderStatistics(statisticsHtml) {
        if (statisticsHtml) {
            domElements.divStatistics.html(statisticsHtml);
        } else {
            domElements.divStatistics.html('<p>No statistics data available.</p>');
        }
    }

    function Initial() {
        AttachEventListeners();

        ToggleSections('visualization');

        domElements.chatbotToggle.on('click', function () { ToggleChatbot(); });

        domElements.closeChatbot.on('click', function () { ToggleChatbot(); });      

        AnalyzeFile();

        domElements.btnGenerate.on('click', function () { AnalyzeFile(); });
    }

    $(document).ready(function () {
        Initial();
    })
}

$(AIInsights);

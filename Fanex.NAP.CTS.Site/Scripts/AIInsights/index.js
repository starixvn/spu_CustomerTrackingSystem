function AIInsights() {
    const container = document.querySelector('.ai-insights-container');
    const fileInput = document.getElementById('fileInput');
    const fileNameLabel = document.getElementById('file-name-label');
    const chatbotPanel = document.getElementById('chatbotPanel');
    const chatbotToggle = document.querySelector('.chatbot-toggle');
    const closeChatbot = document.querySelector('.close-chatbot');

    const viewOptions = document.querySelectorAll('input[name="view"]');
    const contentWrapper = container.querySelector('.ai-insights-content') || document.createElement('div');

    const sections = {
        summary: document.getElementById('summary-section'),
        chart: document.getElementById('chart-section'),
        overview: document.getElementById('overview-section'),
        columnStats: document.getElementById('column-statistics-section')
    };

    if (!container.querySelector('.ai-insights-content')) {
        contentWrapper.classList.add('ai-insights-content');
        while (container.firstChild) {
            if (container.firstChild.className !== 'chatbot-toggle' && container.firstChild.id !== 'chatbotPanel') {
                contentWrapper.appendChild(container.firstChild);
            }
        }
        container.appendChild(contentWrapper);
    }

    function ToggleSections(view) {
        switch (view) {
            case 'visualization':
                ShowSections(['summary', 'chart']);
                HideSections(['overview', 'columnStats']);
                break;
            case 'statistics':
                ShowSections(['overview', 'columnStats']);
                HideSections(['summary', 'chart']);
                break;
            default:
                ShowSections(['summary', 'chart']);
                HideSections(['overview', 'columnStats']);
        }
    }

    function ShowSections(sectionIds) {
        Object.values(sections).forEach(section => {
            section.style.display = 'none';
        });
        sectionIds.forEach(id => {
            if (sections[id]) sections[id].style.display = 'block';
        });
    }

    function HideSections(sectionIds) {
        sectionIds.forEach(id => {
            if (sections[id]) sections[id].style.display = 'none';
        });
    }

    function AttachEventListeners() {
        viewOptions.forEach(option => {
            option.addEventListener('change', (e) => {
                ToggleSections(e.target.value);
            });
        });
    }

    function ToggleChatbot() {
        if (chatbotPanel && container) {
            const isOpen = chatbotPanel.classList.contains('active');

            chatbotPanel.classList.toggle('active');
            contentWrapper.classList.toggle('chatbot-open', !isOpen);
            chatbotToggle.style.display = isOpen ? 'flex' : 'none';
            closeChatbot.style.display = isOpen ? 'none' : 'flex';
        }
    }


    if (chatbotToggle) {
        chatbotToggle.addEventListener('click', ToggleChatbot);
    }

    if (closeChatbot) {
        closeChatbot.addEventListener('click', ToggleChatbot);
    }

    function UpdateTitle() {
        const currentText = fileNameLabel.textContent;
        fileNameLabel.setAttribute('title', currentText || 'No file uploaded');
    }

    if (fileInput && fileNameLabel) {
        const initialFileName = fileNameLabel.textContent || 'No file uploaded';
        fileNameLabel.textContent = initialFileName;
        UpdateTitle();

        fileInput.addEventListener('change', (e) => {
            const file = e.target.files[0];

            if (file) {
                const formData = new FormData();

                fileNameLabel.textContent = file.name;
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
                        fileNameLabel.textContent = fileName;
                        UpdateTitle();
                    })
                    .catch(error => {
                        console.error("Upload error:", error);
                    });
            } else {
                fileNameLabel.textContent = "No file uploaded";
                UpdateTitle();
            }
        });
    }

    function Initial() {
        AttachEventListeners();
        ToggleSections('visualization');

        if (chatbotToggle) {
            chatbotToggle.addEventListener('click', ToggleChatbot);
        }

        if (closeChatbot) {
            closeChatbot.addEventListener('click', ToggleChatbot);
        }
    }

    $(document).ready(function () {
        Initial();
    })
}

$(AIInsights);
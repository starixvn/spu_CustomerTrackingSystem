document.addEventListener('DOMContentLoaded', () => {
    const container = document.querySelector('.ai-insights-container');
    const fileInput = document.getElementById('fileInput');
    const fileNameLabel = document.getElementById('file-name-label');
    const chatbotPanel = document.getElementById('chatbotPanel');
    const chatbotToggle = document.querySelector('.chatbot-toggle');
    const closeChatbot = document.querySelector('.close-chatbot');

    const viewOptions = document.querySelectorAll('input[name="view"]');

    const sections = {
        summary: document.getElementById('summary-section'),
        chart: document.getElementById('chart-section'),
        overview: document.getElementById('overview-section'),
        columnStats: document.getElementById('column-statistics-section')
    };

    function toggleSections(view) {
        switch (view) {
            case 'visualization':
                showSections(['summary', 'chart']);
                hideSections(['overview', 'columnStats']);
                break;
            case 'statistics':
                showSections(['overview', 'columnStats']);
                hideSections(['summary', 'chart']);
                break;
            default:
                showSections(['summary', 'chart']);
                hideSections(['overview', 'columnStats']);
        }
    }

    function showSections(sectionIds) {
        Object.values(sections).forEach(section => {
            section.style.display = 'none'; 
        });
        sectionIds.forEach(id => {
            if (sections[id]) sections[id].style.display = 'block';
        });
    }

    function hideSections(sectionIds) {
        sectionIds.forEach(id => {
            if (sections[id]) sections[id].style.display = 'none';
        });
    }

    function attachEventListeners() {
        viewOptions.forEach(option => {
            option.addEventListener('change', (e) => {
                toggleSections(e.target.value);
            });
        });
    }

    function toggleChatbot() {
        if (chatbotPanel && container) {
            const isOpen = chatbotPanel.classList.contains('active');

            if (isOpen) {
                chatbotPanel.classList.remove('active');
                container.classList.remove('chatbot-open');
            } else {
                chatbotPanel.classList.add('active');
                container.classList.add('chatbot-open');
            }
            console.log('Chatbot state:', isOpen ? 'closed' : 'opened');
        }
    }

    if (chatbotToggle) {
        chatbotToggle.addEventListener('click', toggleChatbot);
    }

    if (closeChatbot) {
        closeChatbot.addEventListener('click', toggleChatbot);
    }

    function updateTitle() {
        const currentText = fileNameLabel.textContent;
        fileNameLabel.setAttribute('title', currentText || 'No file uploaded');
    }

    if (fileInput && fileNameLabel) {
        const initialFileName = fileNameLabel.textContent || 'No file uploaded';
        fileNameLabel.textContent = initialFileName;
        updateTitle();

        fileInput.addEventListener('change', (e) => {
            const file = e.target.files[0];

            if (file) {
                const formData = new FormData();

                fileNameLabel.textContent = file.name;
                updateTitle();

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
                        updateTitle();
                    })
                    .catch(error => {
                        console.error("Upload error:", error);
                    });
            } else {
                fileNameLabel.textContent = "No file uploaded";
                updateTitle();
            }
        });
    }

    function initialize() {
        attachEventListeners();
        toggleSections('visualization'); 
    }

    initialize();
});
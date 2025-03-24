
/**
 * Survey Builder JavaScript Module
 * Encapsulates the functionality for creating and managing surveys
 */
const SurveyBuilder = (function() {
    // Private variables
    let questions = [];
    let currentQuestionIndex = -1;
    
    // DOM elements
    const questionContainer = document.getElementById('questions-container');
    const addQuestionBtn = document.getElementById('add-question-btn');
    const surveyForm = document.getElementById('survey-form');
    
    /**
     * Generates a unique ID for new questions
     */
    function generateQuestionId() {
        return 'q-' + (typeof uuidv4 !== 'undefined' ? uuidv4() : Date.now());
    }
    
    /**
     * Creates a new question object
     */
    function createNewQuestion() {
        return {
            id: generateQuestionId(),
            title: 'Nueva pregunta',
            description: '',
            type: 'text',
            required: true,
            options: []
        };
    }
    
    /**
     * Adds sample options for choice-based questions
     */
    function addSampleOptions(questionType) {
        if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(questionType)) {
            return ['Opción 1', 'Opción 2'];
        }
        return [];
    }
    
    /**
     * Creates default settings for special question types
     */
    function createDefaultSettings(questionType) {
        if (questionType === 'rating') {
            return { min: 1, max: 5 };
        } else if (questionType === 'nps') {
            return { min: 0, max: 10 };
        }
        return null;
    }
    
    /**
     * Renders a question to the DOM
     */
    function renderQuestion(question, index) {
        // Create question element based on the template in the DOM
        const template = document.getElementById('question-template');
        if (!template) return;
        
        const questionEl = template.content.cloneNode(true).querySelector('.question-card');
        
        // Set data attributes
        questionEl.setAttribute('data-question-id', question.id);
        questionEl.setAttribute('data-question-index', index);
        
        // Set question title and description
        questionEl.querySelector('.question-title-input').value = question.title;
        
        const descriptionInput = questionEl.querySelector('.question-description-input');
        if (descriptionInput) {
            descriptionInput.value = question.description || '';
        }
        
        // Set question type
        const typeSelect = questionEl.querySelector('.question-type-select');
        if (typeSelect) {
            typeSelect.value = question.type;
        }
        
        // Set required toggle
        const requiredToggle = questionEl.querySelector('.question-required-toggle');
        if (requiredToggle) {
            requiredToggle.checked = question.required;
        }
        
        // Enable move up/down buttons based on position
        const moveUpBtn = questionEl.querySelector('.move-up-btn');
        const moveDownBtn = questionEl.querySelector('.move-down-btn');
        
        if (moveUpBtn) {
            moveUpBtn.disabled = index === 0;
        }
        
        if (moveDownBtn) {
            moveDownBtn.disabled = index === questions.length - 1;
        }
        
        // Update the options container if needed
        updateOptionsContainer(questionEl, question);
        
        // Append the question element to the container
        questionContainer.appendChild(questionEl);
        
        // Add event listeners to the new question element
        setupQuestionEventListeners(questionEl, question, index);
    }
    
    /**
     * Updates the options container based on question type
     */
    function updateOptionsContainer(questionEl, question) {
        const optionsContainer = questionEl.querySelector('.question-options-container');
        if (!optionsContainer) return;
        
        // Clear the container
        optionsContainer.innerHTML = '';
        
        // Only show options for certain question types
        if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(question.type)) {
            // Make the container visible
            optionsContainer.classList.remove('hidden');
            
            // Add each option
            if (question.options && question.options.length > 0) {
                question.options.forEach((option, optIndex) => {
                    addOptionElement(optionsContainer, option, optIndex, question.id);
                });
            }
            
            // Add button to add more options
            const addBtn = document.createElement('button');
            addBtn.type = 'button';
            addBtn.className = 'bg-transparent hover:bg-primary/5 text-primary hover:text-primary-dark px-3 py-1 rounded text-sm flex items-center mt-2';
            addBtn.innerHTML = '<svg class="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path></svg> Agregar opción';
            
            addBtn.addEventListener('click', () => {
                const optionIndex = question.options.length;
                question.options.push(`Opción ${optionIndex + 1}`);
                addOptionElement(optionsContainer, `Opción ${optionIndex + 1}`, optionIndex, question.id);
                
                // Insert before the add button
                optionsContainer.insertBefore(
                    optionsContainer.lastElementChild, 
                    addBtn
                );
            });
            
            optionsContainer.appendChild(addBtn);
        } else {
            // Hide the container for other question types
            optionsContainer.classList.add('hidden');
        }
        
        // Show preview for special question types
        updateQuestionPreview(questionEl, question);
    }
    
    /**
     * Updates the question preview based on type
     */
    function updateQuestionPreview(questionEl, question) {
        const previewContainer = questionEl.querySelector('.question-preview-container');
        if (!previewContainer) return;
        
        // Clear the container
        previewContainer.innerHTML = '';
        
        if (question.type === 'rating') {
            previewContainer.classList.remove('hidden');
            const ratingPreview = document.createElement('div');
            ratingPreview.className = 'question-preview star-rating';
            
            // Create 5 stars
            for (let i = 1; i <= 5; i++) {
                const star = document.createElement('div');
                star.innerHTML = `<svg class="${i <= 3 ? 'filled' : ''}" xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon></svg>`;
                ratingPreview.appendChild(star);
            }
            
            previewContainer.appendChild(ratingPreview);
        } else if (question.type === 'nps') {
            previewContainer.classList.remove('hidden');
            const npsPreview = document.createElement('div');
            npsPreview.className = 'question-preview nps-rating';
            
            // Create 11 options (0-10)
            for (let i = 0; i <= 10; i++) {
                const option = document.createElement('div');
                option.className = `nps-option ${i === 7 ? 'selected' : ''}`;
                option.textContent = i;
                npsPreview.appendChild(option);
            }
            
            previewContainer.appendChild(npsPreview);
        } else {
            previewContainer.classList.add('hidden');
        }
    }
    
    /**
     * Adds an option element to the options container
     */
    function addOptionElement(container, optionText, index, questionId) {
        const optionDiv = document.createElement('div');
        optionDiv.className = 'option-item mb-2';
        optionDiv.innerHTML = `
            <input type="text" class="option-input form-control mr-2" value="${optionText}" 
                   data-option-index="${index}" data-question-id="${questionId}">
            <button type="button" class="option-remove btn btn-sm text-danger" 
                    data-option-index="${index}" data-question-id="${questionId}">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path>
                </svg>
            </button>
        `;
        
        // Add event listener to the option input
        optionDiv.querySelector('.option-input').addEventListener('input', (e) => {
            const index = parseInt(e.target.getAttribute('data-option-index'));
            const questionId = e.target.getAttribute('data-question-id');
            const question = questions.find(q => q.id === questionId);
            
            if (question && question.options && index < question.options.length) {
                question.options[index] = e.target.value;
            }
        });
        
        // Add event listener to the remove button
        optionDiv.querySelector('.option-remove').addEventListener('click', (e) => {
            const index = parseInt(e.target.getAttribute('data-option-index'));
            const questionId = e.target.getAttribute('data-question-id');
            const question = questions.find(q => q.id === questionId);
            
            if (question && question.options && index < question.options.length) {
                // Remove the option
                question.options.splice(index, 1);
                
                // Re-render the question
                const questionEl = document.querySelector(`[data-question-id="${questionId}"]`);
                const questionIndex = questions.findIndex(q => q.id === questionId);
                
                if (questionEl && questionIndex !== -1) {
                    updateOptionsContainer(questionEl, question);
                }
            }
        });
        
        container.appendChild(optionDiv);
    }
    
    /**
     * Sets up event listeners for a question element
     */
    function setupQuestionEventListeners(questionEl, question, index) {
        // Title input event
        const titleInput = questionEl.querySelector('.question-title-input');
        if (titleInput) {
            titleInput.addEventListener('input', (e) => {
                question.title = e.target.value;
            });
        }
        
        // Description input event
        const descriptionInput = questionEl.querySelector('.question-description-input');
        if (descriptionInput) {
            descriptionInput.addEventListener('input', (e) => {
                question.description = e.target.value;
            });
        }
        
        // Type select event
        const typeSelect = questionEl.querySelector('.question-type-select');
        if (typeSelect) {
            typeSelect.addEventListener('change', (e) => {
                const newType = e.target.value;
                question.type = newType;
                
                // Add sample options for choice-based questions
                if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(newType) && 
                    (!question.options || question.options.length === 0)) {
                    question.options = addSampleOptions(newType);
                }
                
                // Add default settings for special question types
                if (['rating', 'nps'].includes(newType)) {
                    question.settings = createDefaultSettings(newType);
                } else {
                    question.settings = null;
                }
                
                // Update the options container
                updateOptionsContainer(questionEl, question);
            });
        }
        
        // Required toggle event
        const requiredToggle = questionEl.querySelector('.question-required-toggle');
        if (requiredToggle) {
            requiredToggle.addEventListener('change', (e) => {
                question.required = e.target.checked;
            });
        }
        
        // Delete button event
        const deleteBtn = questionEl.querySelector('.delete-question-btn');
        if (deleteBtn) {
            deleteBtn.addEventListener('click', () => {
                if (confirm('¿Estás seguro de que quieres eliminar esta pregunta?')) {
                    // Remove from the array
                    questions.splice(index, 1);
                    
                    // Remove from the DOM
                    questionEl.remove();
                    
                    // Re-render all questions to update their indices
                    renderAllQuestions();
                }
            });
        }
        
        // Move up button event
        const moveUpBtn = questionEl.querySelector('.move-up-btn');
        if (moveUpBtn) {
            moveUpBtn.addEventListener('click', () => {
                if (index > 0) {
                    // Swap in the array
                    [questions[index], questions[index - 1]] = [questions[index - 1], questions[index]];
                    
                    // Re-render all questions
                    renderAllQuestions();
                }
            });
        }
        
        // Move down button event
        const moveDownBtn = questionEl.querySelector('.move-down-btn');
        if (moveDownBtn) {
            moveDownBtn.addEventListener('click', () => {
                if (index < questions.length - 1) {
                    // Swap in the array
                    [questions[index], questions[index + 1]] = [questions[index + 1], questions[index]];
                    
                    // Re-render all questions
                    renderAllQuestions();
                }
            });
        }
        
        // Toggle question visibility event (expand/collapse)
        const toggleBtn = questionEl.querySelector('.toggle-question-btn');
        const contentContainer = questionEl.querySelector('.question-content');
        
        if (toggleBtn && contentContainer) {
            toggleBtn.addEventListener('click', () => {
                contentContainer.classList.toggle('hidden');
                
                // Update the icon
                const icon = toggleBtn.querySelector('svg');
                if (icon) {
                    if (contentContainer.classList.contains('hidden')) {
                        icon.innerHTML = '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>';
                    } else {
                        icon.innerHTML = '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7"></path>';
                    }
                }
            });
        }
    }
    
    /**
     * Renders all questions
     */
    function renderAllQuestions() {
        // Clear the container
        questionContainer.innerHTML = '';
        
        // Render each question
        questions.forEach((question, index) => {
            renderQuestion(question, index);
        });
    }
    
    /**
     * Adds a new question
     */
    function addQuestion() {
        // Create a new question
        const newQuestion = createNewQuestion();
        
        // Add to the array
        questions.push(newQuestion);
        
        // Render the question
        renderQuestion(newQuestion, questions.length - 1);
    }
    
    /**
     * Prepares the form data for submission
     */
    function prepareFormData() {
        // Create hidden inputs for each question
        questions.forEach((question, index) => {
            // Create hidden fields for question data
            appendHiddenInput(`Questions[${index}].Id`, question.id);
            appendHiddenInput(`Questions[${index}].Title`, question.title);
            appendHiddenInput(`Questions[${index}].Description`, question.description || '');
            appendHiddenInput(`Questions[${index}].Type`, question.type);
            appendHiddenInput(`Questions[${index}].Required`, question.required);
            
            // Add options if present
            if (question.options && question.options.length > 0) {
                question.options.forEach((option, optIndex) => {
                    appendHiddenInput(`Questions[${index}].Options[${optIndex}]`, option);
                });
            }
            
            // Add settings if present
            if (question.settings) {
                if (question.settings.min !== undefined) {
                    appendHiddenInput(`Questions[${index}].Settings.Min`, question.settings.min);
                }
                if (question.settings.max !== undefined) {
                    appendHiddenInput(`Questions[${index}].Settings.Max`, question.settings.max);
                }
            }
        });
        
        return true;
    }
    
    /**
     * Appends a hidden input to the form
     */
    function appendHiddenInput(name, value) {
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = name;
        input.value = value || '';
        surveyForm.appendChild(input);
    }
    
    /**
     * Initializes the survey builder
     */
    function init() {
        // Check if we're on the survey builder page
        if (!questionContainer || !addQuestionBtn || !surveyForm) {
            return;
        }
        
        // Add initial question if none exist
        if (questions.length === 0) {
            addQuestion();
        }
        
        // Add event listener to the add question button
        addQuestionBtn.addEventListener('click', addQuestion);
        
        // Add event listener to the form for submission
        surveyForm.addEventListener('submit', prepareFormData);
    }
    
    // Return public methods
    return {
        init: init,
        addQuestion: addQuestion,
        getQuestions: () => questions
    };
})();

// Initialize the survey builder when the DOM is loaded
document.addEventListener('DOMContentLoaded', SurveyBuilder.init);

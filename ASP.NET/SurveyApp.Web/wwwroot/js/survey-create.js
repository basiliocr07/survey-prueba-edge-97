
// Survey Creation JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Main elements
    const surveyForm = document.getElementById('surveyForm');
    const questionsContainer = document.getElementById('questionsContainer');
    const emptyQuestionsState = document.getElementById('emptyQuestionsState');
    const questionBuilderTemplate = document.getElementById('questionBuilderTemplate').innerHTML;
    const optionTemplate = document.getElementById('optionTemplate').innerHTML;
    
    // Buttons
    const addQuestionBtn = document.getElementById('addQuestionBtn');
    const addFirstQuestionBtn = document.getElementById('addFirstQuestionBtn');
    const addSampleQuestionsBtn = document.getElementById('addSampleQuestionsBtn');
    const saveSurveyBtn = document.getElementById('saveSurveyBtn');
    
    // Current question count and mapping
    let questionCount = document.querySelectorAll('.question-card').length;
    
    // Question types mapping
    const questionTypes = [
        { type: 'text', name: 'Text Input', description: 'Collect open-ended responses', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="3" y1="12" x2="21" y2="12"></line><line x1="3" y1="6" x2="16" y2="6"></line><line x1="3" y1="18" x2="14" y2="18"></line></svg>' },
        { type: 'multiple-choice', name: 'Multiple Choice', description: 'Allow multiple selections', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="9 11 12 14 22 4"></polyline><path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"></path></svg>' },
        { type: 'single-choice', name: 'Single Choice', description: 'Allow only one selection', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><circle cx="12" cy="12" r="4"></circle></svg>' },
        { type: 'rating', name: 'Rating Scale', description: 'Collect ratings on a scale', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon></svg>' },
        { type: 'dropdown', name: 'Dropdown', description: 'Selection from a dropdown', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="6 9 12 15 18 9"></polyline></svg>' },
        { type: 'matrix', name: 'Matrix', description: 'Grid-based responses', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><line x1="3" y1="9" x2="21" y2="9"></line><line x1="3" y1="15" x2="21" y2="15"></line><line x1="9" y1="3" x2="9" y2="21"></line><line x1="15" y1="3" x2="15" y2="21"></line></svg>' },
        { type: 'ranking', name: 'Ranking', description: 'Order items by preference', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 20V10"></path><path d="M18 14l-6-6-6 6"></path></svg>' },
        { type: 'nps', name: 'Net Promoter Score', description: 'Measure satisfaction', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"></polyline></svg>' },
        { type: 'date', name: 'Date', description: 'Collect date information', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>' },
        { type: 'file-upload', name: 'File Upload', description: 'Allow file uploads', icon: '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path><polyline points="17 8 12 3 7 8"></polyline><line x1="12" y1="3" x2="12" y2="15"></line></svg>' }
    ];
    
    // Initialize event handlers
    function initEventHandlers() {
        // Add question handlers
        if (addQuestionBtn) addQuestionBtn.addEventListener('click', addNewQuestion);
        if (addFirstQuestionBtn) addFirstQuestionBtn.addEventListener('click', addNewQuestion);
        if (addSampleQuestionsBtn) addSampleQuestionsBtn.addEventListener('click', addSampleQuestions);
        if (saveSurveyBtn) saveSurveyBtn.addEventListener('click', function(e) {
            e.preventDefault();
            document.getElementById('surveyForm').submit();
        });
        
        // Delegate events for dynamically created elements
        document.addEventListener('click', handleDelegatedEvents);
        document.addEventListener('input', handleDelegatedInputEvents);
        
        // Submit form handler
        if (surveyForm) {
            surveyForm.addEventListener('submit', validateAndSubmit);
        }
        
        // Initialize existing questions if any
        initExistingQuestions();
    }
    
    // Initialize existing questions
    function initExistingQuestions() {
        const questionCards = document.querySelectorAll('.question-card');
        questionCards.forEach((card, index) => {
            updateQuestionIndex(card, index);
            initQuestionTypeDropdown(card);
            updateQuestionOptionsVisibility(card);
            updateQuestionPreview(card);
            bindQuestionEvents(card);
        });
    }
    
    // Handle delegated events
    function handleDelegatedEvents(e) {
        // Toggle question expansion
        if (e.target.closest('.toggle-question-btn')) {
            const questionCard = e.target.closest('.question-card');
            const content = questionCard.querySelector('.question-content');
            const chevron = questionCard.querySelector('.chevron-icon');
            
            if (content.style.display === 'none') {
                content.style.display = 'block';
                chevron.innerHTML = '<polyline points="18 15 12 9 6 15"></polyline>';
            } else {
                content.style.display = 'none';
                chevron.innerHTML = '<polyline points="6 9 12 15 18 9"></polyline>';
            }
        }
        
        // Delete question
        if (e.target.closest('.delete-question-btn')) {
            const questionCard = e.target.closest('.question-card');
            removeQuestion(questionCard);
        }
        
        // Move question up
        if (e.target.closest('.move-up-btn')) {
            const questionCard = e.target.closest('.question-card');
            moveQuestion(questionCard, 'up');
        }
        
        // Move question down
        if (e.target.closest('.move-down-btn')) {
            const questionCard = e.target.closest('.question-card');
            moveQuestion(questionCard, 'down');
        }
        
        // Toggle question type dropdown
        if (e.target.closest('.question-type-toggle')) {
            const questionCard = e.target.closest('.question-card');
            toggleQuestionTypeDropdown(questionCard);
        }
        
        // Select question type
        if (e.target.closest('.question-type-option')) {
            const typeOption = e.target.closest('.question-type-option');
            const questionCard = e.target.closest('.question-card');
            selectQuestionType(questionCard, typeOption.dataset.type);
        }
        
        // Add option to question
        if (e.target.closest('.add-option-btn')) {
            const questionCard = e.target.closest('.question-card');
            addOption(questionCard);
        }
        
        // Remove option from question
        if (e.target.closest('.remove-option-btn')) {
            const optionItem = e.target.closest('.option-item');
            const questionCard = e.target.closest('.question-card');
            removeOption(questionCard, optionItem);
        }
    }
    
    // Handle delegated input events
    function handleDelegatedInputEvents(e) {
        // Update question title preview when question title changes
        if (e.target.classList.contains('question-title-input')) {
            const questionCard = e.target.closest('.question-card');
            const titlePreview = questionCard.querySelector('.question-title-preview');
            titlePreview.textContent = e.target.value || 'Untitled Question';
        }
        
        // Update required state when checkbox changes
        if (e.target.classList.contains('question-required-checkbox')) {
            const questionCard = e.target.closest('.question-card');
            const hiddenRequiredInput = questionCard.querySelector('input[name^="Questions"][name$="].Required"]');
            hiddenRequiredInput.value = e.target.checked;
        }
    }
    
    // Add a new question
    function addNewQuestion() {
        // Create new question with UUID
        const newQuestion = {
            id: 'new-' + generateUUID(),
            title: 'New Question',
            description: '',
            type: 'text',
            required: true,
            options: []
        };
        
        // Add question to UI
        appendQuestion(newQuestion);
        
        // Hide empty state if visible
        if (emptyQuestionsState) {
            emptyQuestionsState.style.display = 'none';
        }
    }
    
    // Add sample questions
    function addSampleQuestions() {
        const sampleQuestions = [
            {
                id: 'new-' + generateUUID(),
                title: 'How satisfied are you with our service?',
                description: '',
                type: 'rating',
                required: true,
                options: []
            },
            {
                id: 'new-' + generateUUID(),
                title: 'What features do you like most?',
                description: '',
                type: 'multiple-choice',
                required: true,
                options: ['User Interface', 'Performance', 'Customer Support', 'Price']
            },
            {
                id: 'new-' + generateUUID(),
                title: 'Please provide any additional feedback',
                description: '',
                type: 'text',
                required: false,
                options: []
            }
        ];
        
        sampleQuestions.forEach(question => {
            appendQuestion(question);
        });
        
        // Hide empty state if visible
        if (emptyQuestionsState) {
            emptyQuestionsState.style.display = 'none';
        }
    }
    
    // Append a question to the container
    function appendQuestion(question) {
        const index = questionCount++;
        
        // Prepare question type info
        const typeInfo = questionTypes.find(t => t.type === question.type) || questionTypes[0];
        
        // Replace template placeholders
        let questionHtml = questionBuilderTemplate
            .replace(/{{index}}/g, index)
            .replace(/{{id}}/g, question.id)
            .replace(/{{title}}/g, question.title || '')
            .replace(/{{description}}/g, question.description || '')
            .replace(/{{type}}/g, question.type)
            .replace(/{{typeName}}/g, typeInfo.name)
            .replace(/{{requiredChecked}}/g, question.required ? 'checked' : '');
        
        // Create temporary element to hold the HTML
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = questionHtml;
        const questionCard = tempDiv.firstElementChild;
        
        // Add to questions container
        if (questionsContainer) {
            questionsContainer.appendChild(questionCard);
        }
        
        // Initialize the new question
        initQuestionTypeDropdown(questionCard);
        updateQuestionOptionsVisibility(questionCard);
        
        // Add options if present
        if (question.options && question.options.length) {
            question.options.forEach(optionText => {
                addOption(questionCard, optionText);
            });
        } else if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(question.type)) {
            // Add default options for choice-based questions
            addOption(questionCard, 'Option 1');
            addOption(questionCard, 'Option 2');
        }
        
        // Update question preview based on type
        updateQuestionPreview(questionCard);
        
        // Bind events
        bindQuestionEvents(questionCard);
        
        // Update move buttons state for all questions
        updateMoveButtonsState();
    }
    
    // Remove a question
    function removeQuestion(questionCard) {
        // Ask for confirmation
        if (confirm('Are you sure you want to remove this question?')) {
            questionCard.remove();
            
            // Reindex remaining questions
            reindexQuestions();
            
            // Show empty state if no questions left
            if (questionsContainer.childElementCount === 0 && emptyQuestionsState) {
                emptyQuestionsState.style.display = 'block';
            }
            
            // Update move buttons state
            updateMoveButtonsState();
        }
    }
    
    // Move a question up or down
    function moveQuestion(questionCard, direction) {
        if (direction === 'up') {
            const previousSibling = questionCard.previousElementSibling;
            if (previousSibling) {
                questionsContainer.insertBefore(questionCard, previousSibling);
            }
        } else {
            const nextSibling = questionCard.nextElementSibling;
            if (nextSibling) {
                questionsContainer.insertBefore(nextSibling, questionCard);
            }
        }
        
        // Reindex questions
        reindexQuestions();
        
        // Update move buttons state
        updateMoveButtonsState();
    }
    
    // Update move buttons state (disable if at top/bottom)
    function updateMoveButtonsState() {
        const questionCards = questionsContainer.querySelectorAll('.question-card');
        
        questionCards.forEach((card, index) => {
            const isFirst = index === 0;
            const isLast = index === questionCards.length - 1;
            
            const moveUpBtn = card.querySelector('.move-up-btn');
            const moveDownBtn = card.querySelector('.move-down-btn');
            
            if (moveUpBtn) {
                moveUpBtn.disabled = isFirst;
                moveUpBtn.classList.toggle('opacity-50', isFirst);
            }
            
            if (moveDownBtn) {
                moveDownBtn.disabled = isLast;
                moveDownBtn.classList.toggle('opacity-50', isLast);
            }
        });
    }
    
    // Reindex questions after adding/removing/moving
    function reindexQuestions() {
        const questionCards = questionsContainer.querySelectorAll('.question-card');
        questionCards.forEach((card, index) => {
            updateQuestionIndex(card, index);
        });
    }
    
    // Update question index in all form fields
    function updateQuestionIndex(questionCard, newIndex) {
        questionCard.dataset.index = newIndex;
        
        // Update all input names that contain the index
        const inputs = questionCard.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            if (input.name) {
                input.name = input.name.replace(/Questions\[\d+\]/, `Questions[${newIndex}]`);
            }
        });
        
        // Update option indices
        const options = questionCard.querySelectorAll('.option-item');
        options.forEach((option, optIndex) => {
            const input = option.querySelector('input');
            if (input && input.name) {
                input.name = `Questions[${newIndex}].Options[${optIndex}]`;
            }
        });
    }
    
    // Initialize question type dropdown
    function initQuestionTypeDropdown(questionCard) {
        const dropdown = questionCard.querySelector('.question-types-dropdown');
        if (!dropdown) return;
        
        // Clear existing content
        dropdown.innerHTML = '';
        
        // Add question types
        questionTypes.forEach(type => {
            const typeButton = document.createElement('button');
            typeButton.type = 'button';
            typeButton.className = 'question-type-option';
            typeButton.dataset.type = type.type;
            
            // Get current question type
            const currentType = questionCard.querySelector('input[name$="].Type"]').value;
            if (currentType === type.type) {
                typeButton.classList.add('selected');
            }
            
            typeButton.innerHTML = `
                <div class="question-type-icon">${type.icon}</div>
                <div class="question-type-name">${type.name}</div>
                <div class="question-type-desc">${type.description}</div>
            `;
            
            dropdown.appendChild(typeButton);
        });
    }
    
    // Toggle question type dropdown visibility
    function toggleQuestionTypeDropdown(questionCard) {
        const dropdown = questionCard.querySelector('.question-types-dropdown');
        const chevron = questionCard.querySelector('.question-type-chevron');
        
        dropdown.classList.toggle('hidden');
        
        if (!dropdown.classList.contains('hidden')) {
            chevron.innerHTML = '<polyline points="18 15 12 9 6 15"></polyline>';
        } else {
            chevron.innerHTML = '<polyline points="6 9 12 15 18 9"></polyline>';
        }
    }
    
    // Select a question type
    function selectQuestionType(questionCard, type) {
        // Update hidden input
        const typeInput = questionCard.querySelector('input[name$="].Type"]');
        if (typeInput) {
            typeInput.value = type;
        }
        
        // Update type name display
        const typeNameElement = questionCard.querySelector('.question-type-name');
        const typeInfo = questionTypes.find(t => t.type === type);
        if (typeNameElement && typeInfo) {
            typeNameElement.textContent = typeInfo.name;
        }
        
        // Hide dropdown
        const dropdown = questionCard.querySelector('.question-types-dropdown');
        dropdown.classList.add('hidden');
        
        // Update options visibility based on type
        updateQuestionOptionsVisibility(questionCard);
        
        // Add default options if needed
        const optionsList = questionCard.querySelector('.options-list');
        if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(type) && (!optionsList || optionsList.children.length === 0)) {
            addOption(questionCard, 'Option 1');
            addOption(questionCard, 'Option 2');
        }
        
        // Update question preview
        updateQuestionPreview(questionCard);
    }
    
    // Update question options visibility based on type
    function updateQuestionOptionsVisibility(questionCard) {
        const type = questionCard.querySelector('input[name$="].Type"]').value;
        const optionsContainer = questionCard.querySelector('.question-options-container');
        
        // Show options for certain question types
        if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(type)) {
            if (optionsContainer) {
                optionsContainer.classList.remove('hidden');
            }
        } else {
            if (optionsContainer) {
                optionsContainer.classList.add('hidden');
            }
        }
    }
    
    // Update question preview based on type
    function updateQuestionPreview(questionCard) {
        const type = questionCard.querySelector('input[name$="].Type"]').value;
        const previewContainer = questionCard.querySelector('.question-preview-container');
        const previewContent = questionCard.querySelector('.preview-content');
        
        if (!previewContainer || !previewContent) return;
        
        // Show preview for specific question types
        if (['rating', 'nps'].includes(type)) {
            previewContainer.classList.remove('hidden');
            
            if (type === 'rating') {
                // Generate star rating preview
                const starRatingTemplate = document.getElementById('starRatingPreviewTemplate').innerHTML;
                const stars = [];
                for (let i = 1; i <= 5; i++) {
                    stars.push(`
                        <div class="star-rating-option">
                            <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-gray-300">
                                <polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"></polygon>
                            </svg>
                        </div>
                    `);
                }
                previewContent.innerHTML = starRatingTemplate.replace('{{starIcons}}', stars.join(''));
                
            } else if (type === 'nps') {
                // Generate NPS preview
                const npsTemplate = document.getElementById('npsRatingPreviewTemplate').innerHTML;
                const npsOptions = [];
                for (let i = 0; i <= 10; i++) {
                    npsOptions.push(`
                        <div class="nps-label">
                            ${i}
                        </div>
                    `);
                }
                previewContent.innerHTML = npsTemplate.replace('{{npsOptions}}', npsOptions.join(''));
            }
        } else {
            previewContainer.classList.add('hidden');
        }
    }
    
    // Add option to a question
    function addOption(questionCard, optionText = 'New option') {
        const index = parseInt(questionCard.dataset.index);
        const optionsList = questionCard.querySelector('.options-list');
        if (!optionsList) return;
        
        // Count existing options
        const optionCount = optionsList.children.length;
        
        // Replace template placeholders
        let optionHtml = optionTemplate
            .replace(/{{questionIndex}}/g, index)
            .replace(/{{optionIndex}}/g, optionCount)
            .replace(/{{optionValue}}/g, optionText)
            .replace(/{{disabledAttr}}/g, optionCount < 2 ? 'disabled' : '');
        
        // Create temporary element to hold the HTML
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = optionHtml;
        const optionElement = tempDiv.firstElementChild;
        
        // Add to options list
        optionsList.appendChild(optionElement);
        
        // Update disabled state on remove buttons (minimum 2 options)
        updateRemoveOptionButtonsState(questionCard);
    }
    
    // Remove option from a question
    function removeOption(questionCard, optionElement) {
        const optionsList = questionCard.querySelector('.options-list');
        if (!optionsList) return;
        
        optionElement.remove();
        
        // Reindex remaining options
        const options = optionsList.querySelectorAll('.option-item');
        const questionIndex = parseInt(questionCard.dataset.index);
        options.forEach((option, optionIndex) => {
            const input = option.querySelector('input');
            if (input) {
                input.name = `Questions[${questionIndex}].Options[${optionIndex}]`;
            }
        });
        
        // Update disabled state on remove buttons (minimum 2 options)
        updateRemoveOptionButtonsState(questionCard);
    }
    
    // Update disabled state on remove option buttons
    function updateRemoveOptionButtonsState(questionCard) {
        const optionsList = questionCard.querySelector('.options-list');
        if (!optionsList) return;
        
        const options = optionsList.querySelectorAll('.option-item');
        const removeButtons = optionsList.querySelectorAll('.remove-option-btn');
        
        removeButtons.forEach(button => {
            button.disabled = options.length <= 2;
            button.classList.toggle('opacity-50', options.length <= 2);
        });
    }
    
    // Bind events to question card
    function bindQuestionEvents(questionCard) {
        // Add any extra event binding here if needed
    }
    
    // Form validation and submission
    function validateAndSubmit(event) {
        let valid = true;
        
        // Basic validation
        const title = document.querySelector('input[name="Title"]');
        if (!title || !title.value.trim()) {
            alert('Please provide a title for your survey');
            event.preventDefault();
            valid = false;
        }
        
        // Check if there are questions
        const questionCards = questionsContainer.querySelectorAll('.question-card');
        if (questionCards.length === 0) {
            alert('Your survey must have at least one question');
            event.preventDefault();
            valid = false;
        }
        
        // Validate each question has a title
        questionCards.forEach((card, index) => {
            const questionTitle = card.querySelector('input[name^="Questions"][name$="].Text"]');
            if (!questionTitle || !questionTitle.value.trim()) {
                alert(`Question ${index + 1} must have a title`);
                event.preventDefault();
                valid = false;
            }
        });
        
        return valid;
    }
    
    // Utility function to generate UUID
    function generateUUID() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            var r = Math.random() * 16 | 0, 
                v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
    
    // Initialize everything
    initEventHandlers();
});

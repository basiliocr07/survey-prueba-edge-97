
/**
 * Survey Creation JavaScript
 * Handles the interactive functionality for the survey creation page
 */
document.addEventListener('DOMContentLoaded', function() {
    // Tab functionality
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabContents = document.querySelectorAll('.tab-content');
    
    tabButtons.forEach(button => {
        button.addEventListener('click', function() {
            const target = this.dataset.tab;
            
            // Update active tab button
            tabButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
            
            // Show the selected tab content
            tabContents.forEach(content => {
                if (content.id === target) {
                    content.classList.remove('hidden');
                } else {
                    content.classList.add('hidden');
                }
            });
        });
    });
    
    // Question type selection
    document.addEventListener('change', function(e) {
        if (e.target && e.target.name && e.target.name.includes('.Type')) {
            const questionType = e.target.value;
            const questionId = e.target.id.split('_')[1]; // Extraer el ID de la pregunta
            
            // Mostrar u ocultar contenedores de opciones basados en el tipo de pregunta
            const optionsContainer = document.getElementById(`options_${questionId}`);
            const ratingContainer = document.getElementById(`rating_${questionId}`);
            const npsContainer = document.getElementById(`nps_${questionId}`);
            
            if (optionsContainer) {
                if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(questionType)) {
                    optionsContainer.classList.remove('hidden');
                } else {
                    optionsContainer.classList.add('hidden');
                }
            }
            
            if (ratingContainer) {
                if (questionType === 'rating') {
                    ratingContainer.classList.remove('hidden');
                } else {
                    ratingContainer.classList.add('hidden');
                }
            }
            
            if (npsContainer) {
                if (questionType === 'nps') {
                    npsContainer.classList.remove('hidden');
                } else {
                    npsContainer.classList.add('hidden');
                }
            }
        }
    });
    
    // Add option buttons
    document.addEventListener('click', function(e) {
        if (e.target && e.target.classList.contains('add-option-btn')) {
            const questionId = e.target.dataset.questionId;
            const optionsContainer = document.getElementById(`optionsList_${questionId}`);
            const optionCount = optionsContainer.querySelectorAll('.option-item').length;
            
            const newOption = document.createElement('div');
            newOption.className = 'option-item flex items-center gap-2 mb-2';
            newOption.innerHTML = `
                <input type="text" name="Questions[${questionId}].Options[${optionCount}]" class="flex-1 px-3 py-2 border border-gray-300 rounded-md" value="New Option">
                <button type="button" class="remove-option-btn text-gray-500 hover:text-red-500 p-1 rounded-full" data-question-id="${questionId}" data-option-index="${optionCount}">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/></svg>
                </button>
            `;
            
            optionsContainer.appendChild(newOption);
            updateOptionIndices(questionId);
        }
    });
    
    // Remove option buttons
    document.addEventListener('click', function(e) {
        if (e.target && (e.target.classList.contains('remove-option-btn') || e.target.closest('.remove-option-btn'))) {
            const button = e.target.classList.contains('remove-option-btn') ? e.target : e.target.closest('.remove-option-btn');
            const questionId = button.dataset.questionId;
            const optionsContainer = document.getElementById(`optionsList_${questionId}`);
            
            // No eliminar si solo quedan 2 opciones
            if (optionsContainer.querySelectorAll('.option-item').length <= 2) {
                alert('Cannot remove option. At least 2 options are required.');
                return;
            }
            
            button.closest('.option-item').remove();
            updateOptionIndices(questionId);
        }
    });
    
    function updateOptionIndices(questionId) {
        const optionsContainer = document.getElementById(`optionsList_${questionId}`);
        const options = optionsContainer.querySelectorAll('.option-item');
        
        options.forEach((item, index) => {
            const input = item.querySelector('input');
            input.name = `Questions[${questionId}].Options[${index}]`;
            
            const removeBtn = item.querySelector('.remove-option-btn');
            removeBtn.dataset.optionIndex = index;
        });
    }
    
    // Delivery method selection
    const deliveryTypeRadios = document.querySelectorAll('input[name="DeliveryConfig.Type"]');
    const deliverySettings = {
        manual: document.getElementById('manualSettings'),
        scheduled: document.getElementById('scheduledSettings'),
        triggered: document.getElementById('triggerSettings')
    };
    
    deliveryTypeRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            const selectedType = this.value;
            
            // Show the selected delivery settings
            Object.keys(deliverySettings).forEach(type => {
                if (type === selectedType) {
                    deliverySettings[type].classList.remove('hidden');
                } else {
                    deliverySettings[type].classList.add('hidden');
                }
            });
        });
    });
    
    // Schedule frequency selection
    const frequencyRadios = document.querySelectorAll('input[name="DeliveryConfig.Schedule.Frequency"]');
    const dayOfMonthContainer = document.getElementById('dayOfMonthContainer');
    const dayOfWeekContainer = document.getElementById('dayOfWeekContainer');
    
    frequencyRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            const selectedFrequency = this.value;
            
            if (selectedFrequency === 'monthly') {
                dayOfMonthContainer.classList.remove('hidden');
                dayOfWeekContainer.classList.add('hidden');
            } else if (selectedFrequency === 'weekly') {
                dayOfMonthContainer.classList.add('hidden');
                dayOfWeekContainer.classList.remove('hidden');
            } else {
                dayOfMonthContainer.classList.add('hidden');
                dayOfWeekContainer.classList.add('hidden');
            }
        });
    });
    
    // Email address list management
    const emailInput = document.getElementById('emailInput');
    const addEmailBtn = document.getElementById('addEmailBtn');
    const emailListContainer = document.getElementById('emailList');
    
    if (addEmailBtn) {
        addEmailBtn.addEventListener('click', function() {
            if (!emailInput) return;
            
            const email = emailInput.value.trim();
            
            if (email && validateEmail(email)) {
                // Add email to the list
                addEmailToList(email);
                
                // Clear the input
                emailInput.value = '';
                emailInput.focus();
            } else {
                alert('Please enter a valid email address');
            }
        });
    }
    
    // Also handle pressing Enter in the email input
    if (emailInput) {
        emailInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                addEmailBtn.click();
            }
        });
    }
    
    function validateEmail(email) {
        const re = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        return re.test(email);
    }
    
    function addEmailToList(email) {
        const emailCount = emailListContainer.querySelectorAll('.email-item').length;
        
        // Create new email item
        const emailItem = document.createElement('div');
        emailItem.className = 'email-item flex items-center gap-2 mb-2';
        emailItem.innerHTML = `
            <input type="hidden" name="DeliveryConfig.EmailAddresses[${emailCount}]" value="${email}" />
            <div class="flex-1 px-3 py-2 bg-gray-100 border border-gray-300 rounded-md">
                ${email}
            </div>
            <button type="button" class="remove-email-btn text-gray-500 hover:text-red-500 p-1 rounded-full" title="Remove Email">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/></svg>
            </button>
        `;
        
        // Add to container
        emailListContainer.appendChild(emailItem);
        
        // Add event listener to the remove button
        const removeBtn = emailItem.querySelector('.remove-email-btn');
        removeBtn.addEventListener('click', function() {
            emailItem.remove();
            updateEmailIndices();
        });
    }
    
    function updateEmailIndices() {
        const emailItems = emailListContainer.querySelectorAll('.email-item');
        
        emailItems.forEach((item, index) => {
            const input = item.querySelector('input');
            input.name = `DeliveryConfig.EmailAddresses[${index}]`;
        });
    }
    
    // Load existing emails from hidden inputs
    const existingEmails = document.querySelectorAll('input[name^="DeliveryConfig.EmailAddresses["]');
    existingEmails.forEach(input => {
        const email = input.value;
        if (email && validateEmail(email)) {
            addEmailToList(email);
        }
    });
    
    // Add question button
    const addQuestionBtn = document.getElementById('addQuestionBtn');
    const addFirstQuestionBtn = document.getElementById('addFirstQuestionBtn');
    
    function addNewQuestion() {
        const questionCount = document.querySelectorAll('.question-card').length;
        
        // Use AJAX to add a new question
        fetch(`/Surveys/AddQuestion?index=${questionCount}&id=new-${Date.now()}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok: ' + response.statusText);
                }
                return response.text();
            })
            .then(html => {
                const questionsContainer = document.getElementById('questionsContainer');
                
                if (questionsContainer) {
                    // Si no hay preguntas y hay un mensaje "No questions yet", reemplazarlo
                    if (questionsContainer.querySelector('h3')?.textContent.includes('No questions yet')) {
                        questionsContainer.innerHTML = '';
                    }
                    
                    const tempDiv = document.createElement('div');
                    tempDiv.innerHTML = html.trim();
                    
                    // Append the new question to the container
                    questionsContainer.appendChild(tempDiv.firstChild);
                }
            })
            .catch(error => {
                console.error('Error adding question:', error);
                alert('Error adding question: ' + error.message);
            });
    }
    
    if (addQuestionBtn) {
        addQuestionBtn.addEventListener('click', addNewQuestion);
    }
    
    if (addFirstQuestionBtn) {
        addFirstQuestionBtn.addEventListener('click', addNewQuestion);
    }
    
    // Add sample questions button
    const addSampleQuestionsBtn = document.getElementById('addSampleQuestionsBtn');
    if (addSampleQuestionsBtn) {
        addSampleQuestionsBtn.addEventListener('click', function() {
            const questionCount = document.querySelectorAll('.question-card').length;
            
            // Use AJAX to add sample questions
            fetch(`/Surveys/AddSampleQuestions?startIndex=${questionCount}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok: ' + response.statusText);
                    }
                    return response.text();
                })
                .then(html => {
                    const questionsContainer = document.getElementById('questionsContainer');
                    
                    if (questionsContainer) {
                        // Si no hay preguntas y hay un mensaje "No questions yet", reemplazarlo
                        if (questionsContainer.querySelector('h3')?.textContent.includes('No questions yet')) {
                            questionsContainer.innerHTML = '';
                        }
                        
                        const tempDiv = document.createElement('div');
                        tempDiv.innerHTML = html.trim();
                        
                        // Append each sample question to the container
                        while (tempDiv.firstChild) {
                            questionsContainer.appendChild(tempDiv.firstChild);
                        }
                    }
                })
                .catch(error => {
                    console.error('Error adding sample questions:', error);
                    alert('Error adding sample questions: ' + error.message);
                });
        });
    }
    
    // Question deletion
    document.addEventListener('click', function(e) {
        if (e.target && (e.target.classList.contains('delete-question-btn') || e.target.closest('.delete-question-btn'))) {
            const button = e.target.classList.contains('delete-question-btn') ? e.target : e.target.closest('.delete-question-btn');
            const questionCard = button.closest('.question-card');
            
            if (document.querySelectorAll('.question-card').length <= 1) {
                alert('Cannot delete the last question. At least one question is required.');
                return;
            }
            
            if (confirm('Are you sure you want to delete this question?')) {
                questionCard.remove();
                updateQuestionIndices();
            }
        }
    });
    
    // Question movement (up/down)
    document.addEventListener('click', function(e) {
        if (e.target && (e.target.classList.contains('move-question-up') || e.target.closest('.move-question-up'))) {
            const button = e.target.classList.contains('move-question-up') ? e.target : e.target.closest('.move-question-up');
            const questionCard = button.closest('.question-card');
            const prevQuestion = questionCard.previousElementSibling;
            
            if (prevQuestion && prevQuestion.classList.contains('question-card')) {
                questionCard.parentNode.insertBefore(questionCard, prevQuestion);
                updateQuestionIndices();
            }
        }
        
        if (e.target && (e.target.classList.contains('move-question-down') || e.target.closest('.move-question-down'))) {
            const button = e.target.classList.contains('move-question-down') ? e.target : e.target.closest('.move-question-down');
            const questionCard = button.closest('.question-card');
            const nextQuestion = questionCard.nextElementSibling;
            
            if (nextQuestion && nextQuestion.classList.contains('question-card')) {
                questionCard.parentNode.insertBefore(nextQuestion, questionCard);
                updateQuestionIndices();
            }
        }
    });
    
    function updateQuestionIndices() {
        const questionCards = document.querySelectorAll('.question-card');
        
        questionCards.forEach((card, index) => {
            const inputs = card.querySelectorAll('input, textarea, select');
            
            inputs.forEach(input => {
                const name = input.name;
                if (name && name.startsWith('Questions[')) {
                    const newName = name.replace(/Questions\[\d+\]/, `Questions[${index}]`);
                    input.name = newName;
                }
            });
            
            // Update question number display if it exists
            const questionNumber = card.querySelector('.question-number');
            if (questionNumber) {
                questionNumber.textContent = `Question ${index + 1}`;
            }
        });
    }
    
    // Form validation
    const createSurveyForm = document.getElementById('createSurveyForm');
    
    if (createSurveyForm) {
        createSurveyForm.addEventListener('submit', function(e) {
            const title = document.getElementById('Title').value.trim();
            
            if (!title) {
                e.preventDefault();
                alert('Please provide a title for your survey');
                return false;
            }
            
            // Ensure at least one question exists
            const questions = document.querySelectorAll('.question-card');
            if (questions.length === 0) {
                e.preventDefault();
                alert('Please add at least one question to your survey');
                return false;
            }
            
            // Validate required fields for each question
            let isValid = true;
            questions.forEach((questionCard, index) => {
                const questionText = questionCard.querySelector(`[name="Questions[${index}].Text"]`).value.trim();
                if (!questionText) {
                    isValid = false;
                    const questionNumber = index + 1;
                    alert(`Question ${questionNumber} text is required`);
                }
            });
            
            if (!isValid) {
                e.preventDefault();
                return false;
            }
            
            return true;
        });
    }
});


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
    
    // Question type toggle dropdown
    document.addEventListener('click', function(e) {
        if (e.target && (e.target.classList.contains('question-type-toggle') || e.target.closest('.question-type-toggle'))) {
            const button = e.target.classList.contains('question-type-toggle') ? e.target : e.target.closest('.question-type-toggle');
            const dropdown = button.nextElementSibling;
            
            // Toggle this dropdown
            if (dropdown.classList.contains('hidden')) {
                dropdown.classList.remove('hidden');
                
                // Close all other dropdowns
                document.querySelectorAll('.question-types-dropdown:not(.hidden)').forEach(otherDropdown => {
                    if (otherDropdown !== dropdown) {
                        otherDropdown.classList.add('hidden');
                    }
                });
            } else {
                dropdown.classList.add('hidden');
            }
        } else if (e.target && e.target.classList.contains('question-type-option')) {
            // Handle question type selection
            const option = e.target;
            const type = option.dataset.type;
            const questionCard = option.closest('.question-card');
            const questionIndex = questionCard.dataset.questionIndex;
            
            // Update the hidden type input
            const typeInput = questionCard.querySelector(`[name="Questions[${questionIndex}].Type"]`);
            if (typeInput) {
                typeInput.value = type;
                
                // Update the display text
                const typeToggle = questionCard.querySelector('.question-type-toggle');
                if (typeToggle) {
                    const typeText = typeToggle.querySelector('span');
                    if (typeText) {
                        typeText.innerHTML = `Question Type: <span class="font-medium">${type.replace('-', ' ')}</span>`;
                    }
                }
                
                // Update UI based on question type
                updateQuestionTypeUI(questionCard, type);
                
                // Hide dropdown
                option.closest('.question-types-dropdown').classList.add('hidden');
                
                // Mark this option as selected
                option.closest('.question-types-dropdown').querySelectorAll('.question-type-option').forEach(opt => {
                    opt.classList.remove('selected');
                });
                option.classList.add('selected');
            }
        } else if (!e.target.closest('.question-types-dropdown')) {
            // Close all dropdowns when clicking outside
            document.querySelectorAll('.question-types-dropdown:not(.hidden)').forEach(dropdown => {
                dropdown.classList.add('hidden');
            });
        }
    });
    
    // Toggle question card expand/collapse
    document.addEventListener('click', function(e) {
        if (e.target && (e.target.classList.contains('toggle-question-btn') || e.target.closest('.toggle-question-btn'))) {
            const button = e.target.classList.contains('toggle-question-btn') ? e.target : e.target.closest('.toggle-question-btn');
            const questionCard = button.closest('.question-card');
            const content = questionCard.querySelector('.question-content');
            
            if (content.classList.contains('hidden')) {
                content.classList.remove('hidden');
                button.querySelector('svg').innerHTML = '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 15l7-7 7 7"></path>';
            } else {
                content.classList.add('hidden');
                button.querySelector('svg').innerHTML = '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>';
            }
        }
    });
    
    function updateQuestionTypeUI(questionCard, type) {
        const index = questionCard.dataset.questionIndex;
        const optionsContainer = questionCard.querySelector(`#options_${index}`);
        const ratingContainer = questionCard.querySelector(`#rating_${index}`);
        const npsContainer = questionCard.querySelector(`#nps_${index}`);
        
        // Hide all containers first
        if (optionsContainer) optionsContainer.classList.add('hidden');
        if (ratingContainer) ratingContainer.classList.add('hidden');
        if (npsContainer) npsContainer.classList.add('hidden');
        
        // Show container based on question type
        if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(type)) {
            if (optionsContainer) {
                optionsContainer.classList.remove('hidden');
                
                // Ensure at least 2 options exist
                const optionsList = questionCard.querySelector(`#optionsList_${index}`);
                if (optionsList && optionsList.children.length < 2) {
                    for (let i = optionsList.children.length; i < 2; i++) {
                        addOptionToQuestion(index);
                    }
                }
            }
        } else if (type === 'rating') {
            if (ratingContainer) {
                ratingContainer.classList.remove('hidden');
            }
        } else if (type === 'nps') {
            if (npsContainer) {
                npsContainer.classList.remove('hidden');
            }
        }
    }
    
    // Add option buttons
    document.addEventListener('click', function(e) {
        if (e.target && (e.target.classList.contains('add-option-btn') || e.target.closest('.add-option-btn'))) {
            const button = e.target.classList.contains('add-option-btn') ? e.target : e.target.closest('.add-option-btn');
            const questionIndex = button.dataset.questionId;
            addOptionToQuestion(questionIndex);
        }
    });
    
    function addOptionToQuestion(questionIndex) {
        const optionsContainer = document.getElementById(`optionsList_${questionIndex}`);
        if (!optionsContainer) return;
        
        const optionCount = optionsContainer.querySelectorAll('.option-item').length;
        
        const newOption = document.createElement('div');
        newOption.className = 'option-item flex items-center gap-2 mb-2';
        newOption.innerHTML = `
            <input type="text" name="Questions[${questionIndex}].Options[${optionCount}]" 
                  class="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary" 
                  value="New Option">
            <button type="button" class="remove-option-btn text-gray-500 hover:text-red-500 p-1 rounded-full" 
                   data-question-id="${questionIndex}" data-option-index="${optionCount}">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
            </button>
        `;
        
        optionsContainer.appendChild(newOption);
        updateOptionIndices(questionIndex);
    }
    
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
                    // If there are no questions and there's a "No questions yet" message, replace it
                    if (questionsContainer.querySelector('h3') && questionsContainer.querySelector('h3').textContent.includes('No questions yet')) {
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
                        // If there are no questions and there's a "No questions yet" message, replace it
                        if (questionsContainer.querySelector('h3') && questionsContainer.querySelector('h3').textContent.includes('No questions yet')) {
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
            
            if (confirm('Are you sure you want to delete this question?')) {
                questionCard.remove();
                
                // If no questions remain, show the empty state
                const questionsContainer = document.getElementById('questionsContainer');
                if (questionsContainer && !questionsContainer.querySelector('.question-card')) {
                    questionsContainer.innerHTML = `
                        <div class="bg-white rounded-lg shadow-sm p-8 text-center">
                            <div class="mx-auto rounded-full bg-primary/10 p-4 w-16 h-16 flex items-center justify-center mb-4">
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-primary"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>
                            </div>
                            <h3 class="text-lg font-semibold mb-1">No questions yet</h3>
                            <p class="text-gray-500 mb-4">Add questions to your survey</p>
                            <button type="button" id="addFirstQuestionBtn" class="px-4 py-2 bg-primary text-white rounded-md hover:bg-primary-dark">
                                Add First Question
                            </button>
                        </div>
                    `;
                    
                    // Re-attach event listener
                    const newAddFirstBtn = document.getElementById('addFirstQuestionBtn');
                    if (newAddFirstBtn) {
                        newAddFirstBtn.addEventListener('click', addNewQuestion);
                    }
                } else {
                    updateQuestionIndices();
                }
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
            // Update data attribute
            card.dataset.questionIndex = index;
            
            // Update question number in header
            const questionNumber = card.querySelector('.question-number');
            if (questionNumber) {
                questionNumber.textContent = `Question ${index + 1}`;
            }
            
            // Update input names
            const inputs = card.querySelectorAll('input, textarea, select');
            inputs.forEach(input => {
                const name = input.name;
                if (name && name.startsWith('Questions[')) {
                    const newName = name.replace(/Questions\[\d+\]/, `Questions[${index}]`);
                    input.name = newName;
                }
            });
            
            // Update button data attributes
            const buttons = card.querySelectorAll('button[data-question-id]');
            buttons.forEach(button => {
                button.dataset.questionId = index;
            });
            
            // Update container IDs
            const containersToUpdate = [
                card.querySelector(`#options_${card.dataset.questionIndex}`),
                card.querySelector(`#optionsList_${card.dataset.questionIndex}`),
                card.querySelector(`#rating_${card.dataset.questionIndex}`),
                card.querySelector(`#nps_${card.dataset.questionIndex}`)
            ];
            
            containersToUpdate.forEach(container => {
                if (container) {
                    const oldId = container.id;
                    const newId = oldId.replace(/\d+$/, index);
                    container.id = newId;
                }
            });
            
            // Update move buttons state based on position
            const moveUpBtn = card.querySelector('.move-question-up');
            const moveDownBtn = card.querySelector('.move-question-down');
            
            if (moveUpBtn) {
                if (index === 0) {
                    moveUpBtn.classList.add('opacity-50', 'cursor-not-allowed');
                    moveUpBtn.disabled = true;
                } else {
                    moveUpBtn.classList.remove('opacity-50', 'cursor-not-allowed');
                    moveUpBtn.disabled = false;
                }
            }
            
            if (moveDownBtn) {
                if (index === questionCards.length - 1) {
                    moveDownBtn.classList.add('opacity-50', 'cursor-not-allowed');
                    moveDownBtn.disabled = true;
                } else {
                    moveDownBtn.classList.remove('opacity-50', 'cursor-not-allowed');
                    moveDownBtn.disabled = false;
                }
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
    
    // Initialize UI state for existing questions
    document.querySelectorAll('.question-card').forEach(questionCard => {
        const questionIndex = questionCard.dataset.questionIndex;
        const typeSelect = questionCard.querySelector(`[name="Questions[${questionIndex}].Type"]`);
        
        if (typeSelect) {
            updateQuestionTypeUI(questionCard, typeSelect.value);
        }
    });
});

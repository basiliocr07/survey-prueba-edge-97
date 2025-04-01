
/**
 * Question Builder JavaScript
 * Handles the interactive functionality for survey question editing
 */
document.addEventListener('DOMContentLoaded', function() {
    // Question expand/collapse functionality
    window.toggleQuestionExpand = function(button) {
        const questionCard = button.closest('.question-card');
        const content = questionCard.querySelector('.question-content');
        const icon = button.querySelector('svg path');
        
        if (content.classList.contains('hidden')) {
            content.classList.remove('hidden');
            icon.setAttribute('d', 'M5 15l7-7 7 7');
        } else {
            content.classList.add('hidden');
            icon.setAttribute('d', 'M19 9l-7 7-7-7');
        }
    };
    
    // Question type selector toggle
    document.querySelectorAll('.question-type-toggle').forEach(toggle => {
        toggle.addEventListener('click', function() {
            const dropdown = this.nextElementSibling;
            dropdown.classList.toggle('hidden');
        });
    });
    
    // Question type selection
    document.querySelectorAll('.question-type-option').forEach(option => {
        option.addEventListener('click', function() {
            const type = this.dataset.type || this.querySelector('input[type="radio"]').dataset.type;
            const questionCard = this.closest('.question-card');
            const typeDisplay = questionCard.querySelector('.question-type-display');
            const typeInput = questionCard.querySelector('.current-question-type');
            const optionsContainer = questionCard.querySelector('.question-options-container');
            const previewContainer = questionCard.querySelector('.question-preview-container');
            
            // Update visible type
            typeDisplay.textContent = type.replace('-', ' ');
            
            // Update hidden input
            typeInput.value = type;
            
            // Toggle options container visibility
            if (['multiple-choice', 'single-choice', 'dropdown', 'ranking'].includes(type)) {
                optionsContainer.classList.remove('hidden');
                
                // Add default options if none exist
                if (optionsContainer.querySelectorAll('.option-item').length === 0) {
                    addOption(questionCard, 'Option 1');
                    addOption(questionCard, 'Option 2');
                }
            } else {
                optionsContainer.classList.add('hidden');
            }
            
            // Toggle preview container visibility
            if (['rating', 'nps'].includes(type)) {
                previewContainer.classList.remove('hidden');
                updatePreview(questionCard, type);
            } else {
                previewContainer.classList.add('hidden');
            }
            
            // Hide the dropdown
            this.closest('.question-types-dropdown').classList.add('hidden');
            
            // Mark this option as selected
            document.querySelectorAll('.question-type-option').forEach(opt => {
                opt.classList.remove('selected');
            });
            this.classList.add('selected');
            
            // Update radio button
            this.querySelector('input[type="radio"]').checked = true;
        });
    });
    
    // Add option button
    document.querySelectorAll('.add-option-btn').forEach(button => {
        button.addEventListener('click', function() {
            const questionCard = this.closest('.question-card');
            const optionItems = questionCard.querySelectorAll('.option-item');
            addOption(questionCard, `Option ${optionItems.length + 1}`);
        });
    });
    
    // Function to add a new option
    function addOption(questionCard, optionText) {
        const optionsContainer = questionCard.querySelector('.question-options-container');
        const addButton = optionsContainer.querySelector('.add-option-btn');
        const questionIndex = questionCard.dataset.questionIndex;
        const optionItems = optionsContainer.querySelectorAll('.option-item');
        const newOptionIndex = optionItems.length;
        
        // Create new option element
        const optionItem = document.createElement('div');
        optionItem.className = 'option-item flex items-center gap-2 mb-2';
        optionItem.innerHTML = `
            <div class="flex-1">
                <input type="text" name="Questions[${questionIndex}].Options[${newOptionIndex}]" value="${optionText}" 
                       class="option-input w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary" 
                       placeholder="Option ${newOptionIndex + 1}" />
            </div>
            <button type="button" class="remove-option-btn text-gray-500 hover:text-red-500 p-1 rounded-full" title="Remove Option">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 6h18M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/></svg>
            </button>
        `;
        
        // Add before the add button
        optionsContainer.insertBefore(optionItem, addButton);
        
        // Add event listener to the remove button
        const removeButton = optionItem.querySelector('.remove-option-btn');
        removeButton.addEventListener('click', function() {
            removeOption(questionCard, this);
        });
        
        // Update all remove buttons' disabled state
        updateRemoveButtonsState(questionCard);
    }
    
    // Function to remove an option
    function removeOption(questionCard, button) {
        const optionsContainer = questionCard.querySelector('.question-options-container');
        const optionItems = optionsContainer.querySelectorAll('.option-item');
        
        // Don't allow removing if only 2 options remain
        if (optionItems.length <= 2) {
            return;
        }
        
        // Remove the option item
        const optionItem = button.closest('.option-item');
        optionItem.remove();
        
        // Re-index remaining options
        optionsContainer.querySelectorAll('.option-item').forEach((item, index) => {
            const input = item.querySelector('input');
            const questionIndex = questionCard.dataset.questionIndex;
            input.name = `Questions[${questionIndex}].Options[${index}]`;
            input.placeholder = `Option ${index + 1}`;
        });
        
        // Update all remove buttons' disabled state
        updateRemoveButtonsState(questionCard);
    }
    
    // Function to update preview based on question type
    function updatePreview(questionCard, type) {
        const previewContainer = questionCard.querySelector('.question-preview-container');
        previewContainer.innerHTML = '';
        
        const label = document.createElement('label');
        label.className = 'block text-sm font-medium mb-2';
        label.textContent = 'Preview';
        previewContainer.appendChild(label);
        
        if (type === 'rating') {
            const ratingPreview = document.createElement('div');
            ratingPreview.className = 'rating-preview flex gap-1';
            
            for (let i = 1; i <= 5; i++) {
                const star = document.createElement('div');
                star.className = `star ${i <= 3 ? 'text-yellow-400' : 'text-gray-300'}`;
                star.innerHTML = `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="${i <= 3 ? 'currentColor' : 'none'}" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>`;
                ratingPreview.appendChild(star);
            }
            
            previewContainer.appendChild(ratingPreview);
            
            // Add hidden settings
            const questionIndex = questionCard.dataset.questionIndex;
            const settingsHtml = `
                <input type="hidden" name="Questions[${questionIndex}].Settings.Min" value="1" />
                <input type="hidden" name="Questions[${questionIndex}].Settings.Max" value="5" />
            `;
            previewContainer.insertAdjacentHTML('beforeend', settingsHtml);
        } else if (type === 'nps') {
            const npsPreview = document.createElement('div');
            npsPreview.className = 'nps-preview';
            
            const grid = document.createElement('div');
            grid.className = 'grid grid-cols-11 gap-1';
            
            for (let i = 0; i <= 10; i++) {
                const option = document.createElement('div');
                option.className = `nps-option text-center py-2 border rounded-md cursor-pointer ${i === 7 ? 'bg-primary/10 border-primary' : ''}`;
                option.textContent = i;
                grid.appendChild(option);
            }
            
            npsPreview.appendChild(grid);
            previewContainer.appendChild(npsPreview);
            
            // Add hidden settings
            const questionIndex = questionCard.dataset.questionIndex;
            const settingsHtml = `
                <input type="hidden" name="Questions[${questionIndex}].Settings.Min" value="0" />
                <input type="hidden" name="Questions[${questionIndex}].Settings.Max" value="10" />
            `;
            previewContainer.insertAdjacentHTML('beforeend', settingsHtml);
        }
    }
    
    // Function to update remove buttons state
    function updateRemoveButtonsState(questionCard) {
        const optionsContainer = questionCard.querySelector('.question-options-container');
        const optionItems = optionsContainer.querySelectorAll('.option-item');
        const removeButtons = optionsContainer.querySelectorAll('.remove-option-btn');
        
        removeButtons.forEach(button => {
            if (optionItems.length <= 2) {
                button.classList.add('opacity-50', 'cursor-not-allowed');
                button.setAttribute('disabled', '');
            } else {
                button.classList.remove('opacity-50', 'cursor-not-allowed');
                button.removeAttribute('disabled');
            }
        });
    }
    
    // Initialize option remove buttons
    document.querySelectorAll('.remove-option-btn').forEach(button => {
        button.addEventListener('click', function() {
            const questionCard = this.closest('.question-card');
            removeOption(questionCard, this);
        });
    });
    
    // Initialize the remove buttons state
    document.querySelectorAll('.question-card').forEach(card => {
        updateRemoveButtonsState(card);
    });
    
    // Initialize the title display update
    document.querySelectorAll('.question-title-input').forEach(input => {
        input.addEventListener('input', function() {
            const card = this.closest('.question-card');
            const titleDisplay = card.querySelector('.question-title-display');
            titleDisplay.textContent = this.value || 'Untitled Question';
        });
    });
});

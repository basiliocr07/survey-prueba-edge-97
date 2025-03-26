
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
            
            return true;
        });
    }
});

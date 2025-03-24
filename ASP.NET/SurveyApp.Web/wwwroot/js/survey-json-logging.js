
/**
 * Utility functions for logging survey JSON data to the browser console
 */

// Function to log survey data
function logSurveyData(surveyData) {
    console.log('Survey Data (JSON):', JSON.stringify(surveyData, null, 2));
    return surveyData;
}

// Function to log survey response data
function logSurveyResponseData(responseData) {
    console.log('Survey Response Data (JSON):', JSON.stringify(responseData, null, 2));
    return responseData;
}

// Add event listeners to forms when document is ready
document.addEventListener('DOMContentLoaded', function() {
    // Find survey creation form
    const surveyForm = document.querySelector('form[action*="Create"]');
    if (surveyForm) {
        surveyForm.addEventListener('submit', function() {
            // Collect form data
            const formData = new FormData(surveyForm);
            const surveyData = {};
            
            // Convert form data to object
            for (let [key, value] of formData.entries()) {
                surveyData[key] = value;
            }
            
            // Log the data
            console.log('Survey Form Submit (JSON):', JSON.stringify(surveyData, null, 2));
        });
        
        console.log('Survey form detected and listener attached');
    }
    
    // Find survey response form
    const responseForm = document.querySelector('form[action*="Submit"]');
    if (responseForm) {
        responseForm.addEventListener('submit', function() {
            // Collect form data
            const formData = new FormData(responseForm);
            const responseData = {};
            
            // Convert form data to object
            for (let [key, value] of formData.entries()) {
                responseData[key] = value;
            }
            
            // Log the data
            console.log('Survey Response Submit (JSON):', JSON.stringify(responseData, null, 2));
        });
        
        console.log('Response form detected and listener attached');
    }
});

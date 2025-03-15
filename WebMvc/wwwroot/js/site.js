
// Site JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Toast notification function
    window.showToast = function(message, type = 'info', duration = 5000) {
        const toast = document.createElement('div');
        toast.className = `toast ${type}`;
        toast.innerHTML = `
            <div class="flex items-center">
                <div class="flex-shrink-0">
                    ${type === 'success' ? '<i class="fas fa-check-circle"></i>' : ''}
                    ${type === 'error' ? '<i class="fas fa-exclamation-circle"></i>' : ''}
                    ${type === 'info' ? '<i class="fas fa-info-circle"></i>' : ''}
                </div>
                <div class="ml-3">
                    <p class="text-sm font-medium">${message}</p>
                </div>
                <div class="ml-auto pl-3">
                    <div class="-mx-1.5 -my-1.5">
                        <button class="close-toast inline-flex rounded-md p-1.5 text-gray-500 hover:text-gray-700 focus:outline-none">
                            <span class="sr-only">Dismiss</span>
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;
        
        document.body.appendChild(toast);
        
        // Animate in
        setTimeout(() => {
            toast.classList.add('active');
        }, 10);
        
        // Auto dismiss
        const timeout = setTimeout(() => {
            dismissToast(toast);
        }, duration);
        
        // Close button
        toast.querySelector('.close-toast').addEventListener('click', () => {
            clearTimeout(timeout);
            dismissToast(toast);
        });
        
        function dismissToast(toast) {
            toast.classList.remove('active');
            setTimeout(() => {
                toast.remove();
            }, 300);
        }
    };
    
    // Initialize any components that need JavaScript
    initializeTabsComponents();
});

function initializeTabsComponents() {
    const tabContainers = document.querySelectorAll('.tabs-container');
    
    tabContainers.forEach(container => {
        const tabButtons = container.querySelectorAll('.tab-button');
        const tabPanels = container.querySelectorAll('.tab-panel');
        
        tabButtons.forEach(button => {
            button.addEventListener('click', () => {
                const tabId = button.getAttribute('data-tab');
                
                // Update active states
                tabButtons.forEach(btn => btn.classList.remove('active'));
                tabPanels.forEach(panel => panel.classList.remove('active'));
                
                button.classList.add('active');
                container.querySelector(`.tab-panel[data-tab="${tabId}"]`).classList.add('active');
            });
        });
    });
}

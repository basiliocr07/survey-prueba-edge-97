
// Site JavaScript

// Function to handle navbar scrolling effect
document.addEventListener('DOMContentLoaded', function() {
    const header = document.querySelector('header');
    const mobileMenuToggle = document.getElementById('mobile-menu-toggle');
    const mobileMenu = document.getElementById('mobile-menu');
    
    // Handle scroll events for navbar transparency
    window.addEventListener('scroll', function() {
        if (window.scrollY > 10) {
            header.classList.add('glass', 'shadow-sm');
            header.classList.remove('bg-transparent');
        } else {
            header.classList.remove('glass', 'shadow-sm');
            header.classList.add('bg-transparent');
        }
    });
    
    // Toggle mobile menu
    if (mobileMenuToggle && mobileMenu) {
        mobileMenuToggle.addEventListener('click', function() {
            if (mobileMenu.classList.contains('opacity-0')) {
                // Open menu
                mobileMenu.classList.remove('opacity-0', 'translate-x-full', 'pointer-events-none');
                mobileMenu.classList.add('opacity-100', 'translate-x-0');
            } else {
                // Close menu
                mobileMenu.classList.add('opacity-0', 'translate-x-full', 'pointer-events-none');
                mobileMenu.classList.remove('opacity-100', 'translate-x-0');
            }
        });
    }
});

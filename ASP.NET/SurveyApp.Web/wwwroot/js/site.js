
// Navbar scroll effect
document.addEventListener('DOMContentLoaded', function() {
    const navbar = document.getElementById('navbar');
    const mobileMenu = document.getElementById('mobile-menu');
    const mobileMenuToggle = document.getElementById('mobile-menu-toggle');
    const mobileMenuClose = document.getElementById('mobile-menu-close');

    // Navbar scroll effect
    function handleScroll() {
        if (window.scrollY > 10) {
            navbar.classList.add('glass', 'shadow-sm');
            navbar.classList.remove('bg-transparent');
        } else {
            navbar.classList.remove('glass', 'shadow-sm');
            navbar.classList.add('bg-transparent');
        }
    }

    // Mobile menu toggle
    function toggleMobileMenu() {
        if (mobileMenu.classList.contains('opacity-0')) {
            // Open menu
            mobileMenu.classList.remove('opacity-0', 'translate-x-full', 'pointer-events-none');
            mobileMenu.classList.add('opacity-100', 'translate-x-0');
            document.body.style.overflow = 'hidden'; // Prevent scrolling when menu is open
        } else {
            // Close menu
            mobileMenu.classList.add('opacity-0', 'translate-x-full', 'pointer-events-none');
            mobileMenu.classList.remove('opacity-100', 'translate-x-0');
            document.body.style.overflow = ''; // Restore scrolling
        }
    }

    // Add event listeners
    window.addEventListener('scroll', handleScroll);
    
    if (mobileMenuToggle) {
        mobileMenuToggle.addEventListener('click', toggleMobileMenu);
    }
    
    if (mobileMenuClose) {
        mobileMenuClose.addEventListener('click', toggleMobileMenu);
    }

    // Initialize navbar state
    handleScroll();
});

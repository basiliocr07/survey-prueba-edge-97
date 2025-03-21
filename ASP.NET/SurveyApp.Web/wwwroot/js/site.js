
// Navbar Functionality
document.addEventListener('DOMContentLoaded', function() {
    // Variables for navbar elements
    const navbar = document.getElementById('navbar');
    const mobileMenuToggle = document.getElementById('mobile-menu-toggle');
    const mobileMenuClose = document.getElementById('mobile-menu-close');
    const mobileMenu = document.getElementById('mobile-menu');
    
    // Handle scroll event for navbar background change
    function handleScroll() {
        if (window.scrollY > 10) {
            navbar.classList.add('glass', 'shadow-sm');
        } else {
            navbar.classList.remove('glass', 'shadow-sm');
        }
    }
    
    // Initial check for scroll position
    handleScroll();
    
    // Add scroll event listener
    window.addEventListener('scroll', handleScroll);
    
    // Mobile menu toggle functionality
    if (mobileMenuToggle && mobileMenu && mobileMenuClose) {
        mobileMenuToggle.addEventListener('click', function() {
            mobileMenu.classList.remove('opacity-0', 'translate-x-full', 'pointer-events-none');
            mobileMenu.classList.add('opacity-100', 'translate-x-0');
        });
        
        mobileMenuClose.addEventListener('click', function() {
            mobileMenu.classList.remove('opacity-100', 'translate-x-0');
            mobileMenu.classList.add('opacity-0', 'translate-x-full', 'pointer-events-none');
        });
    }
    
    // Dropdown menu hover functionality for desktop
    const dropdownMenus = document.querySelectorAll('.group');
    
    dropdownMenus.forEach(menu => {
        const content = menu.querySelector('div[class*="hidden group-hover:block"]');
        if (content) {
            // Desktop behavior uses CSS :hover
            // We don't need JS for this part, it's handled by Tailwind's group-hover
        }
    });
});

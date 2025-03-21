
// Mobile menu functionality
document.addEventListener('DOMContentLoaded', function() {
    const menuToggle = document.getElementById('menu-toggle');
    const closeMenu = document.getElementById('close-menu');
    const mobileMenu = document.getElementById('mobile-menu');
    
    if (menuToggle && closeMenu && mobileMenu) {
        menuToggle.addEventListener('click', function() {
            mobileMenu.classList.remove('hidden');
            mobileMenu.classList.add('flex');
            document.body.style.overflow = 'hidden';
        });
        
        closeMenu.addEventListener('click', function() {
            mobileMenu.classList.add('hidden');
            mobileMenu.classList.remove('flex');
            document.body.style.overflow = '';
        });
    }
    
    // Navbar scroll effect - similar to React version
    const header = document.querySelector('header');
    
    if (header) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 10) {
                header.classList.add('border-b', 'bg-background/80', 'backdrop-blur-sm');
            } else {
                header.classList.remove('border-b', 'bg-background/80', 'backdrop-blur-sm');
            }
        });
    }
});


// Este archivo contiene la configuración de Tailwind CSS para su uso en archivos Razor
// Las variables CSS personalizadas se definen aquí para mantener la coherencia con la configuración de Tailwind

document.addEventListener('DOMContentLoaded', function() {
    // Variables CSS globales
    const root = document.documentElement;
    
    // Colores principales
    root.style.setProperty('--primary', '215 100% 50%'); // Azul
    root.style.setProperty('--primary-foreground', '210 40% 98%');
    
    // Colores secundarios
    root.style.setProperty('--secondary', '215 20% 65%');
    root.style.setProperty('--secondary-foreground', '210 40% 98%');
    
    // Colores de fondo y texto
    root.style.setProperty('--background', '0 0% 100%');
    root.style.setProperty('--foreground', '215 35% 15%');
    
    // Colores de acento
    root.style.setProperty('--accent', '215 20% 95%');
    root.style.setProperty('--accent-foreground', '215 35% 15%');
    
    // Colores de borde
    root.style.setProperty('--border', '215 10% 90%');
    root.style.setProperty('--input', '215 10% 90%');
    
    // Colores de tarjeta
    root.style.setProperty('--card', '0 0% 100%');
    root.style.setProperty('--card-foreground', '215 35% 15%');
    
    // Colores de elementos mutados
    root.style.setProperty('--muted', '215 10% 95%');
    root.style.setProperty('--muted-foreground', '215 10% 55%');
    
    // Colores destructivos
    root.style.setProperty('--destructive', '0 100% 50%');
    root.style.setProperty('--destructive-foreground', '210 40% 98%');
    
    // Otros valores
    root.style.setProperty('--radius', '0.5rem');
    
    // Colores de la barra lateral
    root.style.setProperty('--sidebar-background', '215 35% 15%');
    root.style.setProperty('--sidebar-foreground', '210 40% 98%');
    root.style.setProperty('--sidebar-primary', '215 100% 50%');
    root.style.setProperty('--sidebar-primary-foreground', '210 40% 98%');
    root.style.setProperty('--sidebar-accent', '215 20% 25%');
    root.style.setProperty('--sidebar-accent-foreground', '210 40% 98%');
    root.style.setProperty('--sidebar-border', '215 20% 30%');
    root.style.setProperty('--sidebar-ring', '215 100% 50%');
});

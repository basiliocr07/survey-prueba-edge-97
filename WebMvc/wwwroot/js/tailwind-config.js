
// Configuración de Tailwind para la aplicación
tailwind.config = {
    theme: {
        extend: {
            colors: {
                primary: {
                    DEFAULT: 'hsl(215, 25%, 27%)',
                    foreground: 'hsl(210, 40%, 98%)'
                },
                secondary: {
                    DEFAULT: 'hsl(210, 40%, 96.1%)',
                    foreground: 'hsl(222.2, 47.4%, 11.2%)'
                },
                accent: {
                    DEFAULT: 'hsl(210, 30%, 92%)',
                    foreground: 'hsl(222.2, 47.4%, 11.2%)'
                },
                muted: {
                    DEFAULT: 'hsl(210, 40%, 96.1%)',
                    foreground: 'hsl(215.4, 16.3%, 46.9%)'
                },
                card: {
                    DEFAULT: 'hsl(0, 0%, 100%)',
                    foreground: 'hsl(222.2, 84%, 4.9%)'
                },
                background: 'hsl(210, 40%, 98%)',
                foreground: 'hsl(222.2, 84%, 4.9%)',
                border: 'hsl(214.3, 31.8%, 91.4%)',
            },
            fontFamily: {
                sans: ['Lato', 'Inter', 'sans-serif'],
            },
            animation: {
                'fade-in': 'fadeIn 0.5s ease-out',
                'slide-up': 'slideUp 0.4s ease-out',
                'slide-down': 'slideDown 0.4s ease-out',
                'pulse-slow': 'pulseSlow 3s infinite',
            },
            keyframes: {
                fadeIn: {
                    '0%': { opacity: 0 },
                    '100%': { opacity: 1 },
                },
                slideUp: {
                    '0%': { transform: 'translateY(10px)', opacity: 0 },
                    '100%': { transform: 'translateY(0)', opacity: 1 },
                },
                slideDown: {
                    '0%': { transform: 'translateY(-10px)', opacity: 0 },
                    '100%': { transform: 'translateY(0)', opacity: 1 },
                },
                pulseSlow: {
                    '0%, 100%': { opacity: 1 },
                    '50%': { opacity: 0.8 },
                }
            }
        },
    },
    safelist: [
        'bg-primary', 'text-primary', 'border-primary',
        'bg-secondary', 'text-secondary', 'border-secondary',
        'bg-accent', 'text-accent', 'border-accent',
        'bg-muted', 'text-muted-foreground',
        'hover:bg-primary', 'hover:text-primary', 'hover:border-primary'
    ]
}

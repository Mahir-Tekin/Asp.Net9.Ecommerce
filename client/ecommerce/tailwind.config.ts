import type { Config } from 'tailwindcss'

const config: Config = {
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      colors: {
        brand: {
          primary: '#8b5cf6', // purple-600
          secondary: '#3b82f6', // blue-600
          accent: '#06b6d4', // cyan-400
          danger: '#ef4444', // red-500
        },
        surface: {
          light: '#fafafa', // gray-50 - main background
          card: '#ffffff', // white - card backgrounds  
          border: '#f3f4f6', // gray-100 - subtle borders
          muted: '#6b7280', // gray-500 - muted text
        }
      },
      backgroundImage: {
        'brand-gradient': 'linear-gradient(135deg, rgb(139, 92, 246), rgb(59, 130, 246), rgb(147, 51, 234))',
        'accent-gradient': 'linear-gradient(45deg, rgb(59, 130, 246), rgb(6, 182, 212))',
        'danger-gradient': 'linear-gradient(45deg, rgb(239, 68, 68), rgb(220, 38, 38))',
      },
      animation: {
        'float': 'float 3s ease-in-out infinite',
        'glow': 'glow 2s ease-in-out infinite alternate',
      },
      keyframes: {
        float: {
          '0%, 100%': { transform: 'translateY(0px)' },
          '50%': { transform: 'translateY(-10px)' },
        },
        glow: {
          'from': { boxShadow: '0 0 20px rgba(139, 92, 246, 0.5)' },
          'to': { boxShadow: '0 0 30px rgba(59, 130, 246, 0.8)' },
        },
      },
      backdropBlur: {
        xs: '2px',
      }
    },
  },
  plugins: [],
}
export default config

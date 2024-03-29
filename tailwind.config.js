/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme');

module.exports = {
	content: ['./Areas/**/*.{cshtml,cs}', './Views/**/*.{cshtml,cs}'],
	theme: {
		container: {
			padding: {
				DEFAULT: '1rem',
				sm: '2rem',
				lg: '4rem',
				xl: '5rem',
				'2xl': '6rem',
			},
		},
		extend: {
			colors: {
				primary: '#7148fc',
				secondary: '#6633CC',
				darkest: '#0c121c',
				dark: '#1E293B',
				mid: '#e0e0e0',
				light: '#fffdf8',
				lightest: '#d6dee7',
			},
			fontFamily: {
				montserrat: ['montserrat', ...defaultTheme.fontFamily.sans],
			},
		},
	},
	plugins: [require('@tailwindcss/forms'), require('@tailwindcss/typography')],
};

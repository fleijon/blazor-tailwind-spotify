/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./**/*.{razor,html,cshtml}"],
  theme: {
    extend: {
      colors: {
        'dark1': '#101010',
        'dark2': '#282828',
        'dark3': '#3E3D3D',
        'dark4': '#1C1C1C',
      }
    },
  },
  plugins: [],
}

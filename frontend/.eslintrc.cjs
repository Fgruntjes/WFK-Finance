/* eslint-disable no-undef */
module.exports = {
  env: { browser: true, es2020: true },
  extends: [
    'eslint:recommended',
    'plugin:@typescript-eslint/recommended',
    'plugin:react/recommended',
    'plugin:react-hooks/recommended',
  ],
  parser: '@typescript-eslint/parser',
  parserOptions: { ecmaVersion: 'latest', sourceType: 'module' },
  plugins: [
    '@typescript-eslint',
    'react-refresh',
    'react-hooks',
    'formatjs',
  ],
  rules: {
    'react/react-in-jsx-scope': 'off',

    'react-refresh/only-export-components': 'warn',

    "react-hooks/rules-of-hooks": "error",
    "react-hooks/exhaustive-deps": "error",

    "formatjs/no-offset": "error",
    "formatjs/no-literal-string-in-jsx": "error",
    "formatjs/no-multiple-whitespaces": "error",
    "formatjs/no-multiple-plurals": "error",
    "formatjs/enforce-default-message": "error",
    "formatjs/enforce-id": "error",
  },
}
